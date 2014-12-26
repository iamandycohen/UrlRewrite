using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Module;
using Sitecore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.SecurityModel;

namespace Hi.UrlRewrite.Processing
{
    public class OutboundRewriteProcessor
    {

        public void Process(HttpContextBase httpContext)
        {
            var requestUri = httpContext.Request.Url;
            if (requestUri == null) return;

            if (Configuration.IgnoreUrlPrefixes.Length > 0 &&
                Configuration.IgnoreUrlPrefixes.Any(
                    prefix => requestUri.PathAndQuery.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            var siteContext = Context.Site;
            if (siteContext == null) return;

            var db = siteContext.Database;
            if (db == null) return;

            var outboundRules = GetOutboundRules(db);
            var rewriter = new OutboundRewriter();
            var result = rewriter.ProcessContext(httpContext, outboundRules);

            if (result == null || !result.MatchedAtLeastOneRule) return;

            rewriter.ExecuteResponse(httpContext);
        }

        private List<OutboundRule> GetOutboundRules(Database db)
        {
            var cache = RulesCacheManager.GetCache(db);
            var outboundRules = cache.GetOutboundRules();

            if (outboundRules != null) return outboundRules;

            Log.Info(this, db, "Initializing Outbound Rules.");

            using (new SecurityDisabler())
            {
                var rulesEngine = new RulesEngine();
                rulesEngine.InstallTemplates();
                outboundRules = rulesEngine.GetCachedOutboundRules(db);
            }

            return outboundRules;
        }

    }
}