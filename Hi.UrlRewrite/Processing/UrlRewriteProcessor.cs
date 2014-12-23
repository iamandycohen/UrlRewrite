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
            try
            {
                var db = Sitecore.Context.Database;

                if (args.Context == null || db == null) return;

                var httpContext = new HttpContextWrapper(args.Context);
                var urlRewriter = new UrlRewriter(httpContext.Request.ServerVariables);

                var requestResult = ProcessUri(httpContext.Request.Url, db, urlRewriter);

                if (requestResult == null || !requestResult.MatchedAtLeastOneRule) return;

                Log.Info(string.Format("UrlRewrite - Redirecting {0} to {1} [{2}]", requestResult.OriginalUri, requestResult.RewrittenUri, requestResult.StatusCode), this);

                urlRewriter.ExecuteResult(httpContext, requestResult);
            }
            catch (ThreadAbortException)
            {
                // swallow this exception because we may have called Response.End
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException) return;

                Log.Error("UrlRewrite - Exception occured", ex, this);
            }
        }

        internal ProcessRequestResult ProcessUri(Uri requestUri, Database db, UrlRewriter urlRewriter)
        {
            var cache = RulesCacheManager.GetCache(db);
            var inboundRules = cache.GetInboundRules();

            if (inboundRules == null)
            {
                Log.Info(string.Format("UrlRewrite - Initializing [{0}]", db.Name), this);

                using (new SecurityDisabler())
                {
                    var rulesEngine = new RulesEngine();
                    rulesEngine.InstallTemplates();
                    inboundRules = rulesEngine.RefreshInboundRulesCache(db);
                }
            }

            if (inboundRules == null || Configuration.IgnoreUrlPrefixes.Length > 0 && Configuration.IgnoreUrlPrefixes.Any(prefix => requestUri.PathAndQuery.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
            {
                return null;
            }

            return urlRewriter.ProcessRequestUrl(requestUri, inboundRules);
        }

    }
}
