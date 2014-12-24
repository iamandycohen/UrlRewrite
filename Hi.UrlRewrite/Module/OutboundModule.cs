using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;

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

            if (app != null)
            {
                var context = app.Context;

                // process outbound rules here... only set up event if it matches rules and preconditions
                var responseFilterStream = new ResponseFilterStream(context.Response.Filter);
                responseFilterStream.TransformString += responseFilterStream_TransformString;

                context.Response.Filter = responseFilterStream;
            }
        }

        string responseFilterStream_TransformString(string arg)
        {
            return arg;
        }
    }
}