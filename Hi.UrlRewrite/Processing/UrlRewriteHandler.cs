using System;
using System.Web;
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

            if (requestUri.AbsolutePath.StartsWith("/~/media"))
            {
                // Serve static content:
                var type = typeof(SiteContextFactory).Assembly.GetType("Sitecore.Resources.Media.MediaRequestHandler", true);
                var handler = (IHttpHandler)Activator.CreateInstance(type, true);
                handler.ProcessRequest(context);
            }
            else
            {
                // Serve static content:
                var type = typeof(HttpApplication).Assembly.GetType("System.Web.StaticFileHandler", true);
                var handler = (IHttpHandler)Activator.CreateInstance(type, true);
                handler.ProcessRequest(context);
            }
            
        }
    }
}
