using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;

namespace Hi.UrlRewrite.Processing
{
    public class UrlRewriteHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var urlRewriteProcessor = new UrlRewriteProcessor();
            var requestArgs = new HttpRequestArgs(context, HttpRequestType.Begin);
            var requestUri = context.Request.Url;

            var siteContext = SiteContextFactory.GetSiteContext(requestUri.Host, requestUri.AbsolutePath, requestUri.Port);

            using (new SiteContextSwitcher(siteContext))
            {
                urlRewriteProcessor.Process(requestArgs);
            }

            // Serve static content:
            var type = typeof(HttpApplication).Assembly.GetType("System.Web.StaticFileHandler", true);
            var handler = (IHttpHandler)Activator.CreateInstance(type, true);
            handler.ProcessRequest(context);

        }
    }
}
