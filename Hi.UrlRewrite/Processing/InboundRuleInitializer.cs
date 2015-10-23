using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Pipelines;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Processing
{
    public class InboundRuleInitializer
    {
        public void Process(PipelineArgs args)
        {
            Log.Info(this, "Initializing URL Rewrite.");

            try
            {
                using (new SecurityDisabler())
                {
                    foreach (var db in Factory.GetDatabases().Where(e => e.HasContentItem))
                    {
                        var rulesEngine = new RulesEngine(db);
                        rulesEngine.GetCachedInboundRules();
                    }
                }
            }
            catch (Exception ex)
            {
                Hi.UrlRewrite.Log.Error(this, ex, "Exception during initialization.");
            }
        }
    }
}