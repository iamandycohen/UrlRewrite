using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Processing.Results;
using Sitecore.Data;
using Sitecore.Sites;

namespace Hi.UrlRewrite.Processing
{
    public class InboundRulesHelper
    {

        public ProcessInboundRulesResult GetUrlResults(string url, Database database, NameValueCollection serverVariables = null)
        {
            var rewriter = new InboundRewriter();

            var rulesEngine = new RulesEngine();
            var inboundRules = rulesEngine.GetInboundRules(database);

            var requestUri = new Uri(url);
            var siteContext = SiteContextFactory.GetSiteContext(requestUri.Host, requestUri.AbsolutePath,
                requestUri.Port);

            rewriter.RequestServerVariables = serverVariables ?? new NameValueCollection();

            if (!rewriter.RequestServerVariables.AllKeys.Contains("HTTP_HOST"))
            {
                rewriter.RequestServerVariables.Add("HTTP_HOST", requestUri.Host);
            }

            if (!rewriter.RequestServerVariables.AllKeys.Contains("HTTPS"))
            {
                rewriter.RequestServerVariables.Add("HTTPS", requestUri.Scheme.Equals(Uri.UriSchemeHttps) ? "on" : "off");
            }

            if (!rewriter.RequestServerVariables.AllKeys.Contains("QUERY_STRING") && requestUri.Query.Length > 0)
            {
                rewriter.RequestServerVariables.Add("QUERY_STRING", requestUri.Query.Remove(0, 1));
            }

            var results = rewriter.ProcessRequestUrl(requestUri, inboundRules, siteContext, database);

            return results;
        }

    }
}