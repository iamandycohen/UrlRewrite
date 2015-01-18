using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Hi.UrlRewrite.Processing
{
    public class InboundRewriteProcessor : HttpRequestProcessor
    {

        public override void Process(HttpRequestArgs args)
        {
            var db = Sitecore.Context.Database;

            try
            {

                if (args.Context == null || db == null) return;

                var httpContext = new HttpContextWrapper(args.Context);
                var requestUri = httpContext.Request.Url;

                if (requestUri == null || Configuration.IgnoreUrlPrefixes.Length > 0 && Configuration.IgnoreUrlPrefixes.Any(prefix => requestUri.PathAndQuery.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return;
                }

                var urlRewriter = new InboundRewriter(httpContext.Request.ServerVariables, httpContext.Request.Headers);
                var requestResult = ProcessUri(requestUri, db, urlRewriter);

                if (requestResult == null || !requestResult.MatchedAtLeastOneRule) return;

                //TODO: Curently this only reflects the result of Redirect Actions - make this call to logging reflect all action types
                Log.Info(this, db, "Redirecting {0} to {1} [{2}]", requestResult.OriginalUri, requestResult.RewrittenUri,
                    requestResult.StatusCode);

                urlRewriter.ExecuteResult(httpContext, requestResult);
            }
            catch (ThreadAbortException)
            {
                // swallow this exception because we may have called Response.End
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException) return;

                Log.Error(this, ex, db, "Exception occured");
            }
        }

        internal ProcessInboundRulesResult ProcessUri(Uri requestUri, Database db, InboundRewriter urlRewriter)
        {
            var inboundRules = GetInboundRules(db);

            if (inboundRules == null)
            {
                return null;
            }

            return urlRewriter.ProcessRequestUrl(requestUri, inboundRules);
        }

        private List<InboundRule> GetInboundRules(Database db)
        {
            var cache = RulesCacheManager.GetCache(db);
            var inboundRules = cache.GetInboundRules();

            if (inboundRules != null) return inboundRules;

            Log.Info(this, db, "Initializing Inbound Rules.");

            using (new SecurityDisabler())
            {
                var rulesEngine = new RulesEngine(db);
                var setup = new Setup();
                setup.InstallItems();
                inboundRules = rulesEngine.GetCachedInboundRules();
            }

            return inboundRules;
        }
    }
}
