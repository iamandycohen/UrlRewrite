using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Conditions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Hi.UrlRewrite.Processing
{
    public class RulesEngine
    {
        private static object thisType = typeof(RulesEngine);

        public static List<InboundRule> GetInboundRules(Database db)
        {
            if (db == null)
            {
                return null;
            }

            var query = string.Format(Constants.RedirectFolderItemsQuery, Configuration.RewriteFolderSearchRoot, RedirectFolderItem.TemplateId);
            Item[] redirectFolderItems = db.SelectItems(query);

            if (redirectFolderItems == null)
            {
                return null;
            }

            var inboundRules = new List<InboundRule>();

            foreach (var redirectFolderItem in redirectFolderItems)
            {
                Log.Debug(string.Format("UrlRewrite - Processing RedirectFolder: {0}", redirectFolderItem.Name), thisType);

                var folderDescendants = redirectFolderItem.Axes.GetDescendants()
                    .Where(x => x.TemplateID == new ID(new Guid(SimpleRedirectItem.TemplateId)) ||
                                x.TemplateID == new ID(new Guid(InboundRuleItem.TemplateId)));

                foreach (var descendantItem in folderDescendants)
                {
                    if (descendantItem.TemplateID == new ID(new Guid(SimpleRedirectItem.TemplateId)))
                    {
                        var simpleRedirectItem = new SimpleRedirectItem(descendantItem);

                        Log.Debug(string.Format("UrlRewrite - Processing Simple Redirect: {0}", simpleRedirectItem.Name), thisType);

                        var inboundRule = CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem, redirectFolderItem);

                        inboundRules.Add(inboundRule);
                    }
                    else if (descendantItem.TemplateID == new ID(new Guid(InboundRuleItem.TemplateId)))
                    {
                        var inboundRuleItem = new InboundRuleItem(descendantItem);
                      
                        Log.Debug(string.Format("UrlRewrite - Processing InboundRule: {0}", inboundRuleItem.Name), thisType);

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

        internal static InboundRule CreateInboundRuleFromSimpleRedirectItem(SimpleRedirectItem simpleRedirectInternalItem, RedirectFolderItem redirectFolderItem)
        {
            string inboundRulePattern = string.Format("^{0}/?$", simpleRedirectInternalItem.Path.Value);

            var siteNameRestriction = GetSiteNameRestriction(redirectFolderItem);

            var redirectTo = simpleRedirectInternalItem.Target;
            string actionRewriteUrl = null;
            Guid? redirectItem = null;
            string redirectItemAnchor;

            GetRedirectUrlOrItemId(redirectTo, out actionRewriteUrl, out redirectItem, out redirectItemAnchor);

            Log.Debug(string.Format("UrlRewrite - Creating Inbound Rule From Simple Redirect Item - {0} - id: {1} actionRewriteUrl: {2} redirectItem: {3}", simpleRedirectInternalItem.Name, simpleRedirectInternalItem.ID.Guid, actionRewriteUrl, redirectItem), thisType);

            var inboundRule = new InboundRule
            {
                Action = new RedirectAction
                {
                    AppendQueryString = true,
                    Name = "Redirect",
                    RedirectType = RedirectType.Permanent,
                    RewriteUrl = actionRewriteUrl,
                    RewriteItemId = redirectItem,
                    RewriteItemAnchor = redirectItemAnchor,
                    StopProcessingOfSubsequentRules = false,
                    HttpCacheability = HttpCacheability.NoCache
                },
                Conditions = new List<Condition>(),
                SiteNameRestriction = siteNameRestriction,
                Enabled = true,
                IgnoreCase = true,
                ItemId = simpleRedirectInternalItem.ID.Guid,
                ConditionLogicalGrouping = LogicalGrouping.MatchAll,
                Name = simpleRedirectInternalItem.Name,
                Pattern = inboundRulePattern,
                RequestedUrl = RequestedUrl.MatchesThePattern,
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

    }
}
