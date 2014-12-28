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

            // check preconditions

            var preconditionResult = rewriter.CheckPreconditions(httpContext, outboundRules);
            if (!preconditionResult.Passed) return;

            var transformer = new Tranformer(httpContext, outboundRules);
        }

        private List<OutboundRule> GetOutboundRules(Database db)
        {
            var cache = RulesCacheManager.GetCache(db);
            var outboundRules = cache.GetOutboundRules();

#if !DEBUG
            if (outboundRules != null) return outboundRules;
#endif
            Log.Info(this, db, "Initializing Outbound Rules.");

            using (new SecurityDisabler())
            {
                var rulesEngine = new RulesEngine();
                rulesEngine.InstallTemplates();
                outboundRules = rulesEngine.GetCachedOutboundRules(db);
            }

            return outboundRules;
        }

        private class Tranformer
        {
            public Tranformer(HttpContextBase httpContext, List<OutboundRule> outboundRules)
            {
                _outboundRules = outboundRules;
                _responseFilterStream = new ResponseFilterStream(httpContext.Response.Filter);
                _responseFilterStream.TransformString += TransformString;
                httpContext.Response.Filter = _responseFilterStream;
                _httpContext = httpContext;

            }

            string TransformString(string responseString)
            {
                Rewriter = new OutboundRewriter();
                var result = Rewriter.ProcessContext(_httpContext, responseString, _outboundRules);
                
                if (result == null || !result.MatchedAtLeastOneRule)
                    return responseString;

                return result.ResponseString;
            }

            private readonly HttpContextBase _httpContext;

            private readonly List<OutboundRule> _outboundRules;

            private readonly ResponseFilterStream _responseFilterStream;

            public OutboundRewriter Rewriter { get; set; }
        }

    }
}