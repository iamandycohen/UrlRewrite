using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing.Results;
using Hi.UrlRewrite.Templates;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Publishing;
using Sitecore.SecurityModel;

namespace Hi.UrlRewrite.Processing
{
    public class UrlRewriteProcessor : HttpRequestProcessor
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

                var urlRewriter = new UrlRewriter(httpContext.Request.ServerVariables);
                var requestResult = ProcessUri(requestUri, db, urlRewriter);

                if (requestResult == null || !requestResult.MatchedAtLeastOneRule) return;

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

        internal ProcessRequestResult ProcessUri(Uri requestUri, Database db, UrlRewriter urlRewriter)
        {
            var cache = RulesCacheManager.GetCache(db);
            var inboundRules = cache.GetInboundRules();

            if (inboundRules == null)
            {
                Log.Info(this, db, "Initializing Inbound Rules.");

                using (new SecurityDisabler())
                {
                    var rulesEngine = new RulesEngine();
                    rulesEngine.InstallTemplates();
                    inboundRules = rulesEngine.GetCachedInboundRules(db);
                }
            }

            if (inboundRules == null)
            {
                return null;
            }

            return urlRewriter.ProcessRequestUrl(requestUri, inboundRules);
        }

    }
}
