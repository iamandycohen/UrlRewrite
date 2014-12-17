using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
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
            try
            {
                var urlRewriteProcessor = new UrlRewriteProcessor();
                var requestArgs = new HttpRequestArgs(context, HttpRequestType.Begin);
                var requestUri = context.Request.Url;

                var siteContext = SiteContextFactory.GetSiteContext(requestUri.Host, requestUri.AbsolutePath, requestUri.Port);

                if (siteContext != null)
                {
                    using (new SiteContextSwitcher(siteContext))
                    {
                        urlRewriteProcessor.Process(requestArgs);
                    }
                }
            
                // if we have come this far, the url rewrite processor didn't match on anything so the request is passed to the static request handler

                // Serve static content:
                var type = typeof(HttpApplication).Assembly.GetType("System.Web.StaticFileHandler", true);
                var handler = Activator.CreateInstance(type, true) as IHttpHandler;

                if (handler != null)
                {
                    handler.ProcessRequest(context);
                }
            }
            catch (ThreadAbortException)
            {
                // swallow this exception because we may have called Response.End
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0}::Error in UrlRewriteHandler", this), ex, this);
                
                // don't throw the error, but instead let it fall through
            }

        }
    }
}
