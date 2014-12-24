using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Templates.Conditions;
using Hi.UrlRewrite.Templates.Folders;
using Hi.UrlRewrite.Templates.Inbound;
using Hi.UrlRewrite.Templates.Settings;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
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

        public List<InboundRule> GetInboundRules(Database db)
        {
            if (db == null)
            {
                return null;
            }

            var query = string.Format(Constants.RedirectFolderItemsQuery, Configuration.RewriteFolderSearchRoot, RedirectFolderItem.TemplateId);
            var redirectFolderItems = db.SelectItems(query);

            if (redirectFolderItems == null)
            {
                return null;
            }

            var inboundRules = new List<InboundRule>();

            foreach (var redirectFolderItem in redirectFolderItems)
            {
                Log.Debug(string.Format("UrlRewrite - Processing RedirectFolder: {0}", redirectFolderItem.Name), this);

                var folderDescendants = redirectFolderItem.Axes.GetDescendants()
                    .Where(x => x.TemplateID == new ID(new Guid(SimpleRedirectItem.TemplateId)) ||
                                x.TemplateID == new ID(new Guid(InboundRuleItem.TemplateId)));

                foreach (var descendantItem in folderDescendants)
                {
                    if (descendantItem.TemplateID == new ID(new Guid(SimpleRedirectItem.TemplateId)))
                    {
                        var simpleRedirectItem = new SimpleRedirectItem(descendantItem);

                        Log.Debug(string.Format("UrlRewrite - Processing Simple Redirect: {0}", simpleRedirectItem.Name), this);

                        var inboundRule = CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem, redirectFolderItem);

                        inboundRules.Add(inboundRule);
                    }
                    else if (descendantItem.TemplateID == new ID(new Guid(InboundRuleItem.TemplateId)))
                    {
                        var inboundRuleItem = new InboundRuleItem(descendantItem);
                      
                        Log.Debug(string.Format("UrlRewrite - Processing InboundRule: {0}", inboundRuleItem.Name), this);

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

        public void InstallTemplates()
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
                var urlRewriteModuleItem = settingsDb.GetItem(new ID(Constants.UrlRewriteModuleFolder_ItemId));

                PublishManager.PublishItem(urlRewriteModuleItem, dbArray, urlRewriteModuleItem.Languages, true,
                    true);
            }
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

            Log.Debug(string.Format("UrlRewrite - Creating Inbound Rule From Simple Redirect Item - {0} - id: {1} actionRewriteUrl: {2} redirectItem: {3}", simpleRedirectInternalItem.Name, simpleRedirectInternalItem.ID.Guid, actionRewriteUrl, redirectItem), this);

            var inboundRule = new InboundRule
            {
                Action = new RedirectAction
                {
                    AppendQueryString = true,
                    Name = "Redirect",
                    StatusCode = RedirectActionStatusCode.Permanent,
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
            IEnumerable<BaseConditionItem> conditionItems = null;

            var siteNameRestriction = GetSiteNameRestriction(redirectFolderItem);

            Item[] items = inboundRuleItem.InnerItem.Axes.SelectItems(string.Format(Constants.RedirectFolderConditionItemsQuery, ConditionItem.TemplateId, ConditionAdvancedItem.TemplateId));

            if (items != null)
            {
                conditionItems = items.Select(e => new BaseConditionItem(e));
            }

            var inboundRule = inboundRuleItem.ToInboundRule(conditionItems, siteNameRestriction);

            return inboundRule;
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
                Log.Info(string.Format("UrlRewrite - Adding {0} rules to Cache [{1}]", inboundRules.Count(), db.Name), this);

                var cache = GetRulesCache(db);
                cache.SetInboundRules(inboundRules);
            }
            else
            {
                Log.Info(string.Format("UrlRewrite - Found no rules [{0}]", db.Name), this);
            }

            return inboundRules;
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

                Log.Debug(string.Format("UrlRewrite - Updating Rules Cache - count: {0}", inboundRules.Count), this);

                // update the cache
                cache.SetInboundRules(inboundRules);
            }
        }

        private void AddSimpleRedirect(Item item, Item redirectFolderItem, List<InboundRule> inboundRules)
        {

            Log.Debug(string.Format("UrlRewrite - Adding Simple Redirect - item: [{0}]", item.Paths.FullPath), this);

            var simpleRedirectItem = new SimpleRedirectItem(item);
            var newInboundRule = CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem, redirectFolderItem);

            AddOrRemoveRule(item, redirectFolderItem, inboundRules, newInboundRule);
        }

        private void AddInboundRule(Item item, Item redirectFolderItem, List<InboundRule> inboundRules)
        {

            Log.Debug(string.Format("UrlRewrite - Adding Inbound Rule - item: [{0}]", item.Paths.FullPath), this);

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

                    Log.Debug(string.Format("UrlRewrite - Replacing Inbound Rule - item: [{0}]", item.Paths.FullPath), this);

                    var index = inboundRules.FindIndex(e => e.ItemId == existingInboundRule.ItemId);
                    inboundRules.RemoveAt(index);
                    inboundRules.Insert(index, newInboundRule);
                }
                else
                {

                    Log.Debug(string.Format("UrlRewrite - Adding Inbound Rule - item: [{0}]", item.Paths.FullPath), this);

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
            Log.Debug(string.Format("UrlRewrite - Removing Inbound Rule - item: [{0}]", item.Paths.FullPath), this);

            var existingInboundRule = inboundRules.FirstOrDefault(e => e.ItemId == item.ID.Guid);
            if (existingInboundRule != null)
            {
                inboundRules.Remove(existingInboundRule);
            }
        }

        #endregion

    }
}
