using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Conditions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.SecurityModel;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite
{
    public class RulesEngine
    {
        private static object thisType = typeof(RulesEngine);

        internal static List<InboundRule> GetInboundRules()
        {
            var db = Sitecore.Data.Database.GetDatabase(Configuration.Database);
            if (db == null)
            {
                return null;
            }

            var query = string.Format(Constants.RedirectFolderItemsQuery, Configuration.RewriteFolderSearchRoot, Constants.RedirectFolderTemplateId);
            Item[] redirectFolderItems = db.SelectItems(query);


            if (redirectFolderItems == null)
            {
                return null;
            }

            var inboundRules = new List<InboundRule>();

            foreach (var redirectFolderItem in redirectFolderItems)
            {
                Log.Debug(string.Format("UrlRewrite - Processing RedirectFolder: {0}", redirectFolderItem.Name), thisType);

                var redirectFolder = new RedirectFolderItem(redirectFolderItem);

                var folderDescendants = redirectFolderItem.Axes.GetDescendants();

                foreach (var simpleRedirectInternalItem in folderDescendants
                    .Where(x => x.TemplateID == new ID(new Guid(Constants.SimpleRedirectInternalTemplateId)))
                    .Select(x => new SimpleRedirectItem(x)))
                {
                    Log.Debug(string.Format("UrlRewrite - Processing Simple Redirect: {0}", simpleRedirectInternalItem.Name), thisType);

                    var inboundRule = CreateInboundRuleFromSimpleRedirectItem(simpleRedirectInternalItem);

                    inboundRules.Add(inboundRule);
                }

                foreach (var inboundRuleItem in folderDescendants
                    .Where(x => x.TemplateID == new ID(new Guid(Constants.InboundRuleTemplateId)))
                    .Select(x => new InboundRuleItem(x)))
                {
                    Log.Debug(string.Format("UrlRewrite - Processing InboundRule: {0}", inboundRuleItem.Name), thisType);

                    var inboundRule = CreateInboundRuleFromInboundRuleItem(inboundRuleItem);

                    if (inboundRule.Enabled)
                    {
                        inboundRules.Add(inboundRule);
                    }
                }
            }

            return inboundRules;
        }

        internal static InboundRule CreateInboundRuleFromSimpleRedirectItem(SimpleRedirectItem simpleRedirectInternalItem)
        {
            string inboundRulePattern = string.Format("^{0}/?$", simpleRedirectInternalItem.Path.Value);

            var redirectFolderItem = new RedirectFolderItem(simpleRedirectInternalItem.InnerItem.Parent);
            var siteCondition = GetSiteCondition(redirectFolderItem);

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
                    StopProcessingOfSubsequentRules = true,
                    HttpCacheability = HttpCacheability.NoCache
                },
                Conditions = siteCondition != null ? new List<Condition> { siteCondition } : new List<Condition>(),
                Enabled = true,
                IgnoreCase = true,
                ItemId = simpleRedirectInternalItem.ID.Guid,
                LogicalGrouping = LogicalGrouping.MatchAll,
                Name = simpleRedirectInternalItem.Name,
                Pattern = inboundRulePattern,
                RequestedUrl = RequestedUrl.MatchesThePattern,
                Using = Using.RegularExpressions
            };

            return inboundRule;
        }

        internal static InboundRule CreateInboundRuleFromInboundRuleItem(InboundRuleItem inboundRuleItem)
        {
            IEnumerable<ConditionItem> conditionItems = null;

            var redirectFolderItem = new RedirectFolderItem(inboundRuleItem.InnerItem.Parent);
            var siteCondition = GetSiteCondition(redirectFolderItem);

            Item[] items = inboundRuleItem.InnerItem.Axes.SelectItems(string.Format(Constants.RedirectFolderConditionItemsQuery, Constants.ConditionItemTemplateId));

            if (items != null)
            {
                conditionItems = items.Select(e => new ConditionItem(e));
            }

            var inboundRule = inboundRuleItem.ToInboundRule(conditionItems, siteCondition);

            return inboundRule;
        }

        internal static Condition GetSiteCondition(RedirectFolderItem redirectFolder)
        {
            var sitePattern = redirectFolder.SitePattern.Value;
            Condition siteCondition = null;

            if (!string.IsNullOrEmpty(sitePattern))
            {
                siteCondition = new Condition
                {
                    CheckIfInputString = CheckIfInputStringType.MatchesThePattern,
                    ConditionInput = Hi.UrlRewrite.Entities.ConditionInputType.HTTP_HOST,
                    IgnoreCase = true,
                    Name = "Site",
                    Pattern = sitePattern
                };
            }

            return siteCondition;
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
