using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing;
using Sitecore;
using Sitecore.Data;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Module
{
    public class OutboundModule : IHttpModule
    {
        public void Dispose()
        {
        }
         
        public void Init(HttpApplication context)
        {
            context.ReleaseRequestState += context_ReleaseRequestState;
        }

        void context_ReleaseRequestState(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app == null) return;

            var context = new HttpContextWrapper(app.Context);

            var outboundRewriteProcessor = new OutboundRewriteProcessor();
            outboundRewriteProcessor.Process(context);
        }

    }
}