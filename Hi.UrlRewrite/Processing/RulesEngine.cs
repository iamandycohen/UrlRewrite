using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Templates.Folders;
using Hi.UrlRewrite.Templates.Inbound;
using Hi.UrlRewrite.Templates.Outbound;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Hi.UrlRewrite.Processing
{
	public class RulesEngine
    {
        private readonly Database db;

        public Database Database
        {
            get
            {
                return db;
            }
        }

        public RulesEngine(Database db)
        {
            this.db = db;
        }

        public List<InboundRule> GetInboundRules()
        {
            if (db == null)
            {
                return null;
            }

            var redirectFolderItems = GetRedirectFolderItems();

            if (redirectFolderItems == null)
            {
                return null;
            }

            var inboundRules = new List<InboundRule>();

            foreach (var redirectFolderItem in redirectFolderItems)
            {
                Log.Debug(this, db, "Loading Inbound Rules from RedirectFolder: {0}", redirectFolderItem.Name);

                AssembleRulesRecursive(redirectFolderItem, ref inboundRules, new RedirectFolderItem(redirectFolderItem));
            }

            return inboundRules;
        }

	    private void AssembleRulesRecursive(Item ruleOrFolderItem, ref List<InboundRule> rules, RedirectFolderItem redirectFolderItem)
	    {
			if (ruleOrFolderItem.TemplateID == new ID(new Guid(SimpleRedirectItem.TemplateId)))
			{
				var simpleRedirectItem = new SimpleRedirectItem(ruleOrFolderItem);

				Log.Debug(this, db, "Loading SimpleRedirect: {0}", simpleRedirectItem.Name);

				var inboundRule = CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem, redirectFolderItem);

				if (inboundRule != null && inboundRule.Enabled)
				{
					rules.Add(inboundRule);
				}
			}
			else if (ruleOrFolderItem.TemplateID == new ID(new Guid(InboundRuleItem.TemplateId)))
			{
				var inboundRuleItem = new InboundRuleItem(ruleOrFolderItem);

				Log.Debug(this, db, "Loading InboundRule: {0}", inboundRuleItem.Name);

				var inboundRule = CreateInboundRuleFromInboundRuleItem(inboundRuleItem, redirectFolderItem);

				if (inboundRule != null && inboundRule.Enabled)
				{
					rules.Add(inboundRule);
				}
			}
			else if (ruleOrFolderItem.TemplateID == new ID(new Guid(RedirectSubFolderItem.TemplateId))
			         || ruleOrFolderItem.TemplateID == new ID(new Guid(RedirectFolderItem.TemplateId)))
			{
				ChildList childRules = ruleOrFolderItem.GetChildren();
				foreach (Item childRule in childRules)
				{
					AssembleRulesRecursive(childRule, ref rules, redirectFolderItem);
				}
			}
		}

        public List<OutboundRule> GetOutboundRules()
        {
            if (db == null)
            {
                return null;
            }

            var redirectFolderItems = GetRedirectFolderItems();

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

        private IEnumerable<Item> GetRedirectFolderItems()
        {
            var redirectFolderItems = db.GetItem(RedirectFolderItem.TemplateId)
                .GetReferrers()
                .Where(e => e.TemplateID == new ID(RedirectFolderItem.TemplateId));

            return redirectFolderItems;
        }

        #region Serialization 
        internal InboundRule CreateInboundRuleFromSimpleRedirectItem(SimpleRedirectItem simpleRedirectItem, RedirectFolderItem redirectFolderItem)
        {
            var inboundRulePattern = string.Format("^{0}/?$", simpleRedirectItem.Path.Value);

            var siteNameRestriction = GetSiteNameRestriction(redirectFolderItem);

            var redirectTo = simpleRedirectItem.Target;
            string actionRewriteUrl;
            Guid? redirectItem;
            string redirectItemQueryString;
            string redirectItemAnchor;

            GetRedirectUrlOrItemId(redirectTo, out actionRewriteUrl, out redirectItem, out redirectItemQueryString, out redirectItemAnchor);

            Log.Debug(this, simpleRedirectItem.Database, "Creating Inbound Rule From Simple Redirect Item - {0} - id: {1} actionRewriteUrl: {2} redirectItem: {3}", simpleRedirectItem.Name, simpleRedirectItem.ID.Guid, actionRewriteUrl, redirectItem);

            var inboundRule = new InboundRule
            {
                Action = new Redirect
                {
                    AppendQueryString = true,
                    Name = "Redirect",
                    StatusCode = RedirectStatusCode.Permanent,
                    RewriteUrl = actionRewriteUrl,
                    RewriteItemId = redirectItem,
                    RewriteItemQueryString = redirectItemQueryString,
                    RewriteItemAnchor = redirectItemAnchor,
                    StopProcessingOfSubsequentRules = false,
                    HttpCacheability = HttpCacheability.NoCache
                },
                SiteNameRestriction = siteNameRestriction,
                Enabled = simpleRedirectItem.BaseEnabledItem.Enabled.Checked,
                IgnoreCase = true,
                ItemId = simpleRedirectItem.ID.Guid,
                ConditionLogicalGrouping = LogicalGrouping.MatchAll,
                Name = simpleRedirectItem.Name,
                Pattern = inboundRulePattern,
                MatchType = MatchType.MatchesThePattern,
                Using = Using.RegularExpressions,
				SortOrder = simpleRedirectItem.SortOrder
            };

            return inboundRule;
        }

        internal InboundRule CreateInboundRuleFromInboundRuleItem(InboundRuleItem inboundRuleItem, RedirectFolderItem redirectFolderItem)
        {
            var siteNameRestriction = GetSiteNameRestriction(redirectFolderItem);
            var inboundRule = inboundRuleItem.ToInboundRule(siteNameRestriction);

            return inboundRule;
        }

        internal OutboundRule CreateOutboundRuleFromOutboundRuleItem(OutboundRuleItem outboundRuleItem,
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

        internal static void GetRedirectUrlOrItemId(LinkField redirectTo, out string actionRewriteUrl, out Guid? redirectItemId, out string redirectItemQueryString, out string redirectItemAnchor)
        {
            actionRewriteUrl = null;
            redirectItemId = null;
            redirectItemQueryString = null;
            redirectItemAnchor = null;

            if (redirectTo.TargetItem != null)
            {
                redirectItemId = redirectTo.TargetItem.ID.Guid;
                redirectItemQueryString = redirectTo.QueryString;
                redirectItemAnchor = redirectTo.Anchor;
            }
            else
            {
                actionRewriteUrl = redirectTo.Url;
            }
        }

        #endregion

        #region Caching

        private RulesCache GetRulesCache()
        {
            return RulesCacheManager.GetCache(db);
        }

        internal List<InboundRule> GetCachedInboundRules()
        {
            var inboundRules = GetInboundRules();

            if (inboundRules != null)
            {
                Log.Info(this, db, "Adding {0} rules to Cache [{1}]", inboundRules.Count, db.Name);

                var cache = GetRulesCache();
                cache.SetInboundRules(inboundRules);
            }
            else
            {
                Log.Info(this, db, "Found no rules");
            }

            return inboundRules;
        }

        internal List<OutboundRule> GetCachedOutboundRules()
        {
            var outboundRules = GetOutboundRules();

            if (outboundRules != null)
            {
                Log.Info(this, db, "Adding {0} rules to Cache [{1}]", outboundRules.Count, db.Name);

                var cache = GetRulesCache();
                cache.SetOutboundRules(outboundRules);
            }
            else
            {
                Log.Info(this, db, "Found no rules");
            }

            return outboundRules;
        }

		/// <summary>
		/// Checks to see whether this individual inbound rule can be updated without refreshing all rules
		/// </summary>
	    internal bool CanRefreshInboundRule(Item item, Item redirectFolderItem)
	    {
		    bool result = false;

			var cache = GetRulesCache();
		    var inboundRules = cache.GetInboundRules();

		    if (inboundRules != null)
		    {
				// It's only possible to update this individual rule if its position in the tree has not changed. Sort order is an imperfect test, but it will catch most cases.
			    result = inboundRules.Any(r => r.ItemId == item.ID.Guid && r.SortOrder == item.Appearance.Sortorder);
			}

		    return result;
	    }


		internal void RefreshRule(Item item, Item redirectFolderItem)
        {
            UpdateRulesCache(item, redirectFolderItem, AddRule);
        }

        internal void DeleteRule(Item item, Item redirectFolderItem)
        {
            UpdateRulesCache(item, redirectFolderItem, RemoveRule);
        }

        private void UpdateRulesCache(Item item, Item redirectFolderItem, Action<Item, Item, List<IBaseRule>> action)
        {
            var cache = GetRulesCache();
            IEnumerable<IBaseRule> baseRules = null;
            if (item.IsSimpleRedirectItem() || item.IsInboundRuleItem())
            {
                baseRules = cache.GetInboundRules();
                if (baseRules == null)
                {
                    baseRules = GetInboundRules();
                }
            }
            else if (item.IsOutboundRuleItem())
            {
                baseRules = cache.GetOutboundRules();
                if (baseRules == null)
                {
                    baseRules = GetOutboundRules();
                }
            }

            if (baseRules != null)
            {
                var rules = baseRules.ToList();

                action(item, redirectFolderItem, rules);

                Log.Debug(this, item.Database, "Updating Rules Cache - count: {0}", rules.Count());

                // update the cache
                if (item.IsSimpleRedirectItem() || item.IsInboundRuleItem())
                {
                    cache.SetInboundRules(rules.OfType<InboundRule>());
                }
                else if (item.IsOutboundRuleItem())
                {
                    cache.SetOutboundRules(rules.OfType<OutboundRule>());
                }

            }
        }

        private void AddRule(Item item, Item redirectFolderItem, List<IBaseRule> inboundRules)
        {
            IBaseRule newRule = null;

            Log.Debug(this, item.Database, "Adding Rule - item: [{0}]", item.Paths.FullPath);

            if (item.IsInboundRuleItem())
            {
                newRule = CreateInboundRuleFromInboundRuleItem(item, redirectFolderItem);
            }
            else if (item.IsSimpleRedirectItem())
            {
                newRule = CreateInboundRuleFromSimpleRedirectItem(item, redirectFolderItem);
            }
            else if (item.IsOutboundRuleItem())
            {
                newRule = CreateOutboundRuleFromOutboundRuleItem(item, redirectFolderItem);
            }

            if (newRule != null)
            {
                AddOrRemoveRule(item, redirectFolderItem, inboundRules, newRule);
            }
        }

        private void AddOrRemoveRule(Item item, Item redirectFolderItem, List<IBaseRule> rules, IBaseRule newRule)
        {
            if (newRule.Enabled)
            {
                var existingRule = rules.FirstOrDefault(e => e.ItemId == item.ID.Guid);
                if (existingRule != null)
                {

                    Log.Debug(this, item.Database, "Replacing Rule - item: [{0}]", item.Paths.FullPath);

                    var index = rules.FindIndex(e => e.ItemId == existingRule.ItemId);
                    rules.RemoveAt(index);
                    rules.Insert(index, newRule);
                }
                else
                {

                    Log.Debug(this, item.Database, "Adding Rule - item: [{0}]", item.Paths.FullPath);

                    rules.Add(newRule);
                }
            }
            else
            {
                RemoveRule(item, redirectFolderItem, rules);
            }
        }

        private void RemoveRule(Item item, Item redirectFolderItem, List<IBaseRule> inboundRules)
        {
            Log.Debug(this, item.Database, "Removing Rule - item: [{0}]", item.Paths.FullPath);

            var existingInboundRule = inboundRules.FirstOrDefault(e => e.ItemId == item.ID.Guid);
            if (existingInboundRule != null)
            {
                inboundRules.Remove(existingInboundRule);
            }
		}

		/// <summary>
		/// Clears the cache of inbound rules
		/// </summary>
		internal void ClearInboundRuleCache()
		{
			var cache = GetRulesCache();
			cache.ClearInboundRules();
		}

		/// <summary>
		/// Clears the cache of outbound rules
		/// </summary>
		internal void ClearOutboundRuleCache()
		{
			var cache = GetRulesCache();
			cache.ClearOutboundRules();
		}
		#endregion

	}
}
