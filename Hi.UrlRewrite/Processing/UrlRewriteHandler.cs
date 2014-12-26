using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using System.Reflection;

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
                var urlRewriteProcessor = new InboundRewriteProcessor();
                var requestArgs = new HttpRequestArgs(context, HttpRequestType.Begin);
                var requestUri = context.Request.Url;

                var siteContext = SiteContextFactory.GetSiteContext(requestUri.Host, requestUri.AbsolutePath,
                    requestUri.Port);

                if (siteContext != null)
                {
                    using (new SiteContextSwitcher(siteContext))
                    {
                        urlRewriteProcessor.Process(requestArgs);
                    }
                }

                // if we have come this far, the url rewrite processor didn't match on anything so the request is passed to the static request handler

                // Serve static content:
                var systemWebAssemblyName =
                    GetType()
                        .Assembly.GetReferencedAssemblies()
                        .First(assembly => assembly.FullName.StartsWith("System.Web, "));
                var systemWeb = AppDomain.CurrentDomain.Load(systemWebAssemblyName);

                var staticFileHandlerType = systemWeb.GetType("System.Web.StaticFileHandler", true);
                var staticFileHanlder = Activator.CreateInstance(staticFileHandlerType, true) as IHttpHandler;

                if (staticFileHanlder != null)
                {
                    staticFileHanlder.ProcessRequest(context);
                }
            }
            catch (ThreadAbortException)
            {
                // swallow this exception because we may have called Response.End
            }
            catch (HttpException)
            {
                // we want to throw http exceptions, but we don't care about logging them in Sitecore
                throw;
            }
            catch (Exception ex)
            {

                if (ex is HttpException || ex is ThreadAbortException) return;

                // log it in sitecore
                Log.Error(this, ex, "Error in UrlRewriteHandler.");

                throw;
                // throw the error, but instead let it fall through
            }

        }
    }
}
