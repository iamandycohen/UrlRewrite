using Sitecore;
using Sitecore.IO;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite
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
            Sitecore.Context.Site = siteContext;

            urlRewriteProcessor.Process(requestArgs);
        }
    }
}
