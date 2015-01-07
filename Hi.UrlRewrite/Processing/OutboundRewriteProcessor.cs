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

            var transformer = new Tranformer(httpContext, rewriter, outboundRules);
            transformer.SetupResponseFilter();
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

        private class Tranformer
        {

            private readonly HttpContextBase _httpContext;
            private readonly List<OutboundRule> _outboundRules;
            private readonly OutboundRewriter _rewriter;

            private ResponseFilterStream _responseFilterStream;

            public Tranformer(HttpContextBase httpContext, OutboundRewriter rewriter, List<OutboundRule> outboundRules)
            {
                _outboundRules = outboundRules;
                _httpContext = httpContext;
                _rewriter = rewriter;
            }

            public void SetupResponseFilter()
            {
                _responseFilterStream = new ResponseFilterStream(_httpContext.Response.Filter);
                _responseFilterStream.HeadersWritten += HeadersWritten;
                 _httpContext.Response.Filter = _responseFilterStream;
           }

            void HeadersWritten(HttpContextBase httpContext)
            {
                var preconditionResult = _rewriter.CheckPreconditions(httpContext, _outboundRules);
                if (!preconditionResult.Passed) return;

                _responseFilterStream.TransformString += TransformString;

            }

            string TransformString(string responseString)
            {
                _rewriter.SetupReplacements(_httpContext.Request.ServerVariables, _httpContext.Request.Headers, _httpContext.Response.Headers);
                var result = _rewriter.ProcessContext(_httpContext, responseString, _outboundRules);
                
                if (result == null || !result.MatchedAtLeastOneRule)
                    return responseString;

                return result.ResponseString;
            }

        }

    }
}