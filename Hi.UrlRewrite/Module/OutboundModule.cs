using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Caching;
using Hi.UrlRewrite.Processing;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using Sitecore.Sites;

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
                var requestUri = context.Request.Url;

                if (Configuration.IgnoreUrlPrefixes.Length > 0 && Configuration.IgnoreUrlPrefixes.Any(prefix => requestUri.PathAndQuery.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return;
                }

                var siteContext = Context.Site;
                if (siteContext == null) return;

                var db = siteContext.Database;
                if (db == null) return;

                var cache = RulesCacheManager.GetCache(db);
                var outboundRules = cache.GetOutboundRules();

                if (outboundRules == null)
                {
                    Log.Info(this, db, "Initializing Outbound Rules.");

                    using (new SecurityDisabler())
                    {
                        var rulesEngine = new RulesEngine();
                        rulesEngine.InstallTemplates();
                        outboundRules = rulesEngine.GetCachedOutboundRules(db);
                    }
                }

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