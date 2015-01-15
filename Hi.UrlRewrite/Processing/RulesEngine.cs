using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Templates.Folders;
using Hi.UrlRewrite.Templates.Inbound;
using Hi.UrlRewrite.Templates.Outbound;
using Hi.UrlRewrite.Templates.Settings;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Processing
{
    public class RulesEngine
    {

        public void InstallItems()
        {
            var settingsDb = Database.GetDatabase("master");
            if (settingsDb == null) return;

            using (new SecurityDisabler())
            {
                var settingsItem = settingsDb.GetItem(new ID(Constants.UrlRewriteSettings_ItemId));
                if (settingsItem == null) return;

                var settings = new SettingsItem(settingsItem);
                var publishingTargets = settings.InstallationPublishingTargets.GetItems();
                var dbArray = publishingTargets.Select(x => Factory.GetDatabase(x.Fields["Target database"].Value)).ToArray();

                // install templates
                var urlRewriteTemplatesFolderItem = settingsDb.GetItem(new ID(Constants.UrlRewriteTemplatesFolder_ItemId));
                if (urlRewriteTemplatesFolderItem != null)
                {
                    PublishManager.PublishItem(urlRewriteTemplatesFolderItem, dbArray, urlRewriteTemplatesFolderItem.Languages, true,
                        true);
                }

                // install module folder
                var urlRewriteModuleFolderItem = settingsDb.GetItem(new ID
                    (Constants.UrlRewriteModuleFolder_ItemId));

                if (urlRewriteModuleFolderItem != null)
                {
                    PublishManager.PublishItem(urlRewriteModuleFolderItem, dbArray, urlRewriteModuleFolderItem.Languages, false,
                        true);
                }

                // install reporting folder
                var reportingFolderItem = settingsDb.GetItem(new ID
        (Constants.ReportingFolder_ItemId));
                if (reportingFolderItem != null)
                {
                    PublishManager.PublishItem(reportingFolderItem, dbArray, urlRewriteModuleFolderItem.Languages, false,
                        true);
                }
            }
        }

        public List<InboundRule> GetInboundRules(Database db)
        {
            if (db == null)
            {
                return null;
            }

            var redirectFolderItems = GetRedirectFolderItems(db);

            if (redirectFolderItems == null)
            {
                return null;
            }

            var inboundRules = new List<InboundRule>();

            foreach (var redirectFolderItem in redirectFolderItems)
            {
                Log.Debug(this, db, "Loading Inbound Rules from RedirectFolder: {0}", redirectFolderItem.Name);

                var folderDescendants = redirectFolderItem.Axes.GetDescendants()
                    .Where(x => x.TemplateID == new ID(new Guid(SimpleRedirectItem.TemplateId)) ||
                                x.TemplateID == new ID(new Guid(InboundRuleItem.TemplateId)));

                foreach (var descendantItem in folderDescendants)
                {
                    if (descendantItem.TemplateID == new ID(new Guid(SimpleRedirectItem.TemplateId)))
                    {
                        var simpleRedirectItem = new SimpleRedirectItem(descendantItem);

                        Log.Debug(this, db, "Loading SimpleRedirect: {0}", simpleRedirectItem.Name);

                        var inboundRule = CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem, redirectFolderItem);

                        inboundRules.Add(inboundRule);
                    }
                    else if (descendantItem.TemplateID == new ID(new Guid(InboundRuleItem.TemplateId)))
                    {
                        var inboundRuleItem = new InboundRuleItem(descendantItem);

                        Log.Debug(this, db, "Loading InboundRule: {0}", inboundRuleItem.Name);

                        var inboundRule = CreateInboundRuleFromInboundRuleItem(inboundRuleItem, redirectFolderItem);

                        if (inboundRule != null && inboundRule.Enabled)
                        {
                            inboundRules.Add(inboundRule);
                        }
                    }
                }
            }

            return inboundRules;
        }

        public List<OutboundRule> GetOutboundRules(Database db)
        {
            if (db == null)
            {
                return null;
            }

            var redirectFolderItems = GetRedirectFolderItems(db);

            if (redirectFolderItems == null)
            {
                return null;
            }

            var outboundRules = new List<OutboundRule>();

            foreach (var redirectFolderItem in redirectFolderItems)
            {
                Log.Debug(this, db, "Loading Outbound Rules from RedirectFolder: {0}", redirectFolderItem.Name);

                var folderDescendants = redirectFolderItem.Axes.GetDescendants()
                    .Where(x => x.TemplateID == new ID(new Guid(OutboundRuleItem.TemplateId)));

                foreach (var descendantItem in folderDescendants)
                {
                    if (descendantItem.TemplateID == new ID(new Guid(OutboundRuleItem.TemplateId)))
                    {
                        var outboundRuleItem = new OutboundRuleItem(descendantItem);

                        Log.Debug(this, db, "Loading OutboundRule: {0}", outboundRuleItem.Name);

                        var outboundRule = CreateOutboundRuleFromOutboundRuleItem(outboundRuleItem, redirectFolderItem);

                        if (outboundRule != null && outboundRule.Enabled)
                        {
                            outboundRules.Add(outboundRule);
                        }
                    }
                }
            }

            return outboundRules;
        }

        private static IEnumerable<Item> GetRedirectFolderItems(Database db)
        {
            var query = string.Format(Constants.RedirectFolderItemsQuery, Configuration.RewriteFolderSearchRoot,
                RedirectFolderItem.TemplateId);
            var redirectFolderItems = db.SelectItems(query);

            return redirectFolderItems;
        }

        internal InboundRule CreateInboundRuleFromSimpleRedirectItem(SimpleRedirectItem simpleRedirectInternalItem, RedirectFolderItem redirectFolderItem)
        {
            string inboundRulePattern = string.Format("^{0}/?$", simpleRedirectInternalItem.Path.Value);

            var siteNameRestriction = GetSiteNameRestriction(redirectFolderItem);

            var redirectTo = simpleRedirectInternalItem.Target;
            string actionRewriteUrl;
            Guid? redirectItem;
            string redirectItemAnchor;

            GetRedirectUrlOrItemId(redirectTo, out actionRewriteUrl, out redirectItem, out redirectItemAnchor);

            Log.Debug(this, simpleRedirectInternalItem.Database, "Creating Inbound Rule From Simple Redirect Item - {0} - id: {1} actionRewriteUrl: {2} redirectItem: {3}", simpleRedirectInternalItem.Name, simpleRedirectInternalItem.ID.Guid, actionRewriteUrl, redirectItem);

            var inboundRule = new InboundRule
            {
                Action = new Redirect
                {
                    AppendQueryString = true,
                    Name = "Redirect",
                    StatusCode = RedirectStatusCode.Permanent,
                    RewriteUrl = actionRewriteUrl,
                    RewriteItemId = redirectItem,
                    RewriteItemAnchor = redirectItemAnchor,
                    StopProcessingOfSubsequentRules = false,
                    HttpCacheability = HttpCacheability.NoCache
                },
                SiteNameRestriction = siteNameRestriction,
                Enabled = true,
                IgnoreCase = true,
                ItemId = simpleRedirectInternalItem.ID.Guid,
                ConditionLogicalGrouping = LogicalGrouping.MatchAll,
                Name = simpleRedirectInternalItem.Name,
                Pattern = inboundRulePattern,
                MatchType = MatchType.MatchesThePattern,
                Using = Using.RegularExpressions
            };

            return inboundRule;
        }

        internal static InboundRule CreateInboundRuleFromInboundRuleItem(InboundRuleItem inboundRuleItem, RedirectFolderItem redirectFolderItem)
        {
            var siteNameRestriction = GetSiteNameRestriction(redirectFolderItem);
            var inboundRule = inboundRuleItem.ToInboundRule(siteNameRestriction);

            return inboundRule;
        }

        internal static OutboundRule CreateOutboundRuleFromOutboundRuleItem(OutboundRuleItem outboundRuleItem,
            RedirectFolderItem redirectFolderItem)
        {
            var outboundRule = outboundRuleItem.ToOutboundRule();

            return outboundRule;
        }

        internal static string GetSiteNameRestriction(RedirectFolderItem redirectFolder)
        {
            var site = redirectFolder.SiteNameRestriction.Value;

            return site;
        }

        internal static void GetRedirectUrlOrItemId(LinkField redirectTo, out string actionRewriteUrl, out Guid? redirectItemId, out string redirectItemAnchor)
        {
            actionRewriteUrl = null;
            redirectItemId = null;
            redirectItemAnchor = null;

            if (redirectTo.TargetItem != null)
            {
                redirectItemId = redirectTo.TargetItem.ID.Guid;
                redirectItemAnchor = redirectTo.Anchor;
            }
            else
            {
                actionRewriteUrl = redirectTo.Url;
            }
        }

        #region Caching

        private RulesCache GetRulesCache(Database db)
        {
            return RulesCacheManager.GetCache(db);
        }

        internal List<InboundRule> GetCachedInboundRules(Database db)
        {
            var inboundRules = GetInboundRules(db);

            if (inboundRules != null)
            {
                Log.Info(this, db, "Adding {0} rules to Cache [{1}]", inboundRules.Count(), db.Name);

                var cache = GetRulesCache(db);
                cache.SetInboundRules(inboundRules);
            }
            else
            {
                Log.Info(this, db, "Found no rules");
            }

            return inboundRules;
        }

        internal List<OutboundRule> GetCachedOutboundRules(Database db)
        {
            var outboundRules = GetOutboundRules(db);

            if (outboundRules != null)
            {
                Log.Info(this, db, "Adding {0} rules to Cache [{1}]", outboundRules.Count(), db.Name);

                var cache = GetRulesCache(db);
                cache.SetOutboundRules(outboundRules);
            }
            else
            {
                Log.Info(this, db, "Found no rules");
            }

            return outboundRules;
        }

        internal void RefreshSimpleRedirect(Item item, Item redirectFolderItem)
        {
            UpdateRulesCache(item, redirectFolderItem, AddSimpleRedirect);
        }

        internal void RefreshInboundRule(Item item, Item redirectFolderItem)
        {
            UpdateRulesCache(item, redirectFolderItem, AddInboundRule);
        }

        internal void DeleteInboundRule(Item item, Item redirectFolderItem)
        {
            UpdateRulesCache(item, redirectFolderItem, RemoveInboundRule);
        }

        private void UpdateRulesCache(Item item, Item redirectFolderItem, Action<Item, Item, List<InboundRule>> action)
        {
            var cache = GetRulesCache(item.Database);
            var inboundRules = cache.GetInboundRules();

            if (inboundRules == null)
            {
                inboundRules = GetInboundRules(item.Database);
            }

            if (inboundRules != null)
            {
                action(item, redirectFolderItem, inboundRules);

                Log.Debug(this, item.Database, "Updating Rules Cache - count: {0}", inboundRules.Count);

                // update the cache
                cache.SetInboundRules(inboundRules);
            }
        }

        private void AddSimpleRedirect(Item item, Item redirectFolderItem, List<InboundRule> inboundRules)
        {

            Log.Debug(this, item.Database, "Adding Simple Redirect - item: [{0}]", item.Paths.FullPath);

            var simpleRedirectItem = new SimpleRedirectItem(item);
            var newInboundRule = CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem, redirectFolderItem);

            AddOrRemoveRule(item, redirectFolderItem, inboundRules, newInboundRule);
        }

        private void AddInboundRule(Item item, Item redirectFolderItem, List<InboundRule> inboundRules)
        {

            Log.Debug(this, item.Database, "Adding Inbound Rule - item: [{0}]", item.Paths.FullPath);

            var inboundRuleItem = new InboundRuleItem(item);

            var newInboundRule = RulesEngine.CreateInboundRuleFromInboundRuleItem(inboundRuleItem, redirectFolderItem);

            if (newInboundRule != null)
            {
                AddOrRemoveRule(item, redirectFolderItem, inboundRules, newInboundRule);
            }
        }

        private void AddOrRemoveRule(Item item, Item redirectFolderItem, List<InboundRule> inboundRules, InboundRule newInboundRule)
        {
            if (newInboundRule.Enabled)
            {
                var existingInboundRule = inboundRules.FirstOrDefault(e => e.ItemId == item.ID.Guid);
                if (existingInboundRule != null)
                {

                    Log.Debug(this, item.Database, "Replacing Inbound Rule - item: [{0}]", item.Paths.FullPath);

                    var index = inboundRules.FindIndex(e => e.ItemId == existingInboundRule.ItemId);
                    inboundRules.RemoveAt(index);
                    inboundRules.Insert(index, newInboundRule);
                }
                else
                {

                    Log.Debug(this, item.Database, "Adding Inbound Rule - item: [{0}]", item.Paths.FullPath);

                    inboundRules.Add(newInboundRule);
                }
            }
            else
            {

                RemoveInboundRule(item, redirectFolderItem, inboundRules);
            }
        }

        private void RemoveInboundRule(Item item, Item redirectFolderItem, List<InboundRule> inboundRules)
        {
            Log.Debug(this, item.Database, "Removing Inbound Rule - item: [{0}]", item.Paths.FullPath);

            var existingInboundRule = inboundRules.FirstOrDefault(e => e.ItemId == item.ID.Guid);
            if (existingInboundRule != null)
            {
                inboundRules.Remove(existingInboundRule);
            }
        }

        #endregion

    }
}
