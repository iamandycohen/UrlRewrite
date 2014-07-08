using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Templates;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Hi.UrlRewrite
{
    public class UrlRewriteProcessor : HttpRequestProcessor
    {
        private static object thisType = typeof(UrlRewriteProcessor);
        private static readonly HttpCache cache = new HttpCache();
        public const string rulesCacheKey = "Hi.UrlRewrite:Rules";

        public override void Process(HttpRequestArgs args)
        {
            try
            {
                List<InboundRule> inboundRules = null;

                if (!cache.Get(rulesCacheKey, out inboundRules))
                {
                    Log.Info("UrlRewrite - Initializing", this);
                    using (new SecurityDisabler())
                    {
                        inboundRules = RefreshInboundRulesCache(inboundRules);
                    }
                }

                if (args.Context != null && args.Context.Request != null)
                {
                    if (Configuration.IgnoreUrlPrefixes.Length > 0 && Configuration.IgnoreUrlPrefixes.Any(prefix => args.Context.Request.FilePath.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return;
                    }

                    if (inboundRules != null)
                    {
                        UrlRewriter.ProcessRequestUrl(inboundRules, args.Context.Request.Url);
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // swallow this exception because we may have called Response.End
            }
            catch (Exception ex)
            {
                Log.Error("UrlRewrite - Exception occured", ex, this);
            }
        }

        internal static List<InboundRule> RefreshInboundRulesCache(List<InboundRule> inboundRules = null)
        {
            inboundRules = RulesEngine.GetInboundRules();

            if (inboundRules != null)
            {
                Log.Info(string.Format("UrlRewrite - Adding {0} rules to Cache", inboundRules.Count()), thisType);
                cache.Set(rulesCacheKey, inboundRules, 86400);
            }
            else
            {
                Log.Info("UrlRewrite - Found no rules", thisType);
            }

            return inboundRules;
        }

        internal static void RefreshSimpleRedirect(Item item)
        {
            UpdateRulesCache(item, AddSimpleRedirect);
        }

        internal static void RefreshInboundRule(Item item)
        {
            UpdateRulesCache(item, AddInboundRule);
        }

        internal static void DeleteInboundRule(Item item)
        {
            UpdateRulesCache(item, RemoveInboundRule);
        }

        private static void UpdateRulesCache(Item item, Action<Item, List<InboundRule>> action)
        {
            List<InboundRule> inboundRules = null;
            if (!cache.Get(rulesCacheKey, out inboundRules))
            {
                inboundRules = RulesEngine.GetInboundRules();
            }

            if (inboundRules != null)
            {
                action(item, inboundRules);

                Log.Debug(string.Format("UrlRewrite - Updating Rules Cache - count: {0}", inboundRules.Count), thisType);

                // update the cache
                cache.Set(rulesCacheKey, inboundRules);
            }
        }

        private static void AddSimpleRedirect(Item item, List<InboundRule> inboundRules)
        {

            Log.Debug(string.Format("UrlRewrite - Adding Simple Redirect - item: [{0}]", item.Paths.FullPath), thisType);

            var simpleRedirectItem = new SimpleRedirectItem(item);
            var redirectFolderItem = new RedirectFolderItem(simpleRedirectItem.InnerItem.Parent);
            var siteCondition = RulesEngine.GetSiteCondition(redirectFolderItem);
            var newInboundRule = RulesEngine.CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem, siteCondition);

            AddOrRemoveRule(item, inboundRules, newInboundRule);
        }

        private static void AddInboundRule(Item item, List<InboundRule> inboundRules)
        {

            Log.Debug(string.Format("UrlRewrite - Adding Inbound Rule - item: [{0}]", item.Paths.FullPath), thisType);

            var inboundRuleItem = new InboundRuleItem(item);
            var redirectFolderItem = new RedirectFolderItem(inboundRuleItem.InnerItem.Parent);
            var siteCondition = RulesEngine.GetSiteCondition(redirectFolderItem);
            var newInboundRule = RulesEngine.CreateInboundRuleFromInboundRuleItem(inboundRuleItem, siteCondition);

            AddOrRemoveRule(item, inboundRules, newInboundRule);
        }

        private static void AddOrRemoveRule(Item item, List<InboundRule> inboundRules, InboundRule newInboundRule)
        {
            if (newInboundRule.Enabled)
            {
                var existingInboundRule = inboundRules.FirstOrDefault(e => e.ItemId == item.ID.Guid);
                if (existingInboundRule != null)
                {

                    Log.Debug(string.Format("UrlRewrite - Replacing Inbound Rule - item: [{0}]", item.Paths.FullPath), thisType);

                    var index = inboundRules.FindIndex(e => e.ItemId == existingInboundRule.ItemId);
                    inboundRules.RemoveAt(index);
                    inboundRules.Insert(index, newInboundRule);
                }
                else
                {

                    Log.Debug(string.Format("UrlRewrite - Adding Inbound Rule - item: [{0}]", item.Paths.FullPath), thisType);
                    
                    inboundRules.Add(newInboundRule);
                }
            }
            else
            {

                RemoveInboundRule(item, inboundRules);
            }
        }

        private static void RemoveInboundRule(Item item, List<InboundRule> inboundRules)
        {
            Log.Debug(string.Format("UrlRewrite - Removing Inbound Rule - item: [{0}]", item.Paths.FullPath), thisType);

            var existingInboundRule = inboundRules.FirstOrDefault(e => e.ItemId == item.ID.Guid);
            if (existingInboundRule != null)
            {
                inboundRules.Remove(existingInboundRule);
            }
        }
    }
}
