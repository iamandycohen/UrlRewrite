using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Templates;
using Sitecore;
using Sitecore.Data;
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
        private static readonly object thisType = typeof(UrlRewriteProcessor);
        private static readonly HttpCache cache = new HttpCache();
        public const string rulesCacheKey = "Hi.UrlRewrite:Rules";

        public override void Process(HttpRequestArgs args)
        {
            try
            {
                var db = Sitecore.Context.Database;

                if (args.Context == null || db == null) return;

                var httpContext = new HttpContextWrapper(args.Context);
                var urlRewriter = new UrlRewriter();

                var requestResult = ProcessUri(httpContext.Request.Url, db, urlRewriter);

                if (requestResult == null || !requestResult.HasOneRedirect || requestResult.RewrittenUri == null || requestResult.RewrittenUri.ToString().Equals(requestResult.OriginalUri.ToString(), StringComparison.InvariantCultureIgnoreCase)) return;

                Log.Info(string.Format("UrlRewrite - Redirecting {0} to {1} [{2}]", requestResult.OriginalUri, requestResult.RewrittenUri, requestResult.StatusCode), thisType);

                urlRewriter.ExecuteRedirect(httpContext.Response, requestResult);
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

        internal ProcessRequestResult ProcessUri(Uri requestUri, Database db, UrlRewriter urlRewriter)
        {
            List<InboundRule> inboundRules;

            if (!cache.Get(rulesCacheKey + ":" + db.Name, out inboundRules))
            {
                Log.Info(string.Format("UrlRewrite - Initializing [{0}]", db.Name), this);

                using (new SecurityDisabler())
                {
                    inboundRules = RefreshInboundRulesCache(db);
                }
            }

            if (inboundRules == null || Configuration.IgnoreUrlPrefixes.Length > 0 && Configuration.IgnoreUrlPrefixes.Any(prefix => requestUri.PathAndQuery.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
            {
                return null;
            }

            return urlRewriter.ProcessRequestUrl(requestUri, inboundRules);
        }

        internal static List<InboundRule> RefreshInboundRulesCache(Database db)
        {
            var inboundRules = RulesEngine.GetInboundRules(db);

            if (inboundRules != null)
            {
                Log.Info(string.Format("UrlRewrite - Adding {0} rules to Cache [{0}]", db.Name, inboundRules.Count()), thisType);
                cache.Set(rulesCacheKey + ":" + db.Name, inboundRules, 86400);
            }
            else
            {
                Log.Info(string.Format("UrlRewrite - Found no rules [{0}]", db.Name), thisType);
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
            if (!cache.Get(rulesCacheKey + ":" + item.Database.Name, out inboundRules))
            {
                inboundRules = RulesEngine.GetInboundRules(item.Database);
            }

            if (inboundRules != null)
            {
                action(item, inboundRules);

                Log.Debug(string.Format("UrlRewrite - Updating Rules Cache - count: {0}", inboundRules.Count), thisType);

                // update the cache
                cache.Set(rulesCacheKey + ":" + item.Database.Name, inboundRules);
            }
        }

        private static void AddSimpleRedirect(Item item, List<InboundRule> inboundRules)
        {

            Log.Debug(string.Format("UrlRewrite - Adding Simple Redirect - item: [{0}]", item.Paths.FullPath), thisType);

            var simpleRedirectItem = new SimpleRedirectItem(item);
            var newInboundRule = RulesEngine.CreateInboundRuleFromSimpleRedirectItem(simpleRedirectItem);

            AddOrRemoveRule(item, inboundRules, newInboundRule);
        }

        private static void AddInboundRule(Item item, List<InboundRule> inboundRules)
        {

            Log.Debug(string.Format("UrlRewrite - Adding Inbound Rule - item: [{0}]", item.Paths.FullPath), thisType);

            var inboundRuleItem = new InboundRuleItem(item);
            var newInboundRule = RulesEngine.CreateInboundRuleFromInboundRuleItem(inboundRuleItem);

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
