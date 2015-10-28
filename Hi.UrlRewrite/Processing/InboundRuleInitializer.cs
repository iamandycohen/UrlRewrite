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

                    // cache all of the rules

                    foreach (var db in Factory.GetDatabases().Where(e => e.HasContentItem))
                    {
                        var rulesEngine = new RulesEngine(db);
                        rulesEngine.GetCachedInboundRules();
                    }

                    // make sure that the page event has been deployed
                    DeployEventIfNecessary();
                }
            }
            catch (Exception ex)
            {
                Hi.UrlRewrite.Log.Error(this, ex, "Exception during initialization.");
            }
        }

        private void DeployEventIfNecessary()
        {
            var database = Sitecore.Data.Database.GetDatabase("master");
            if (database == null)
            {
                return;
            }

            var eventItem = database.GetItem(new ID(Constants.RedirectEventItemId));
            if (eventItem == null)
            {
                return;
            }

            var workflow = database.WorkflowProvider.GetWorkflow(eventItem);
            if (workflow == null)
            {
                return;
            }

            var workflowState = workflow.GetState(eventItem);
            if (workflowState == null)
            {
                return;
            }

            const string analyticsDraftStateWorkflowId = "{39156DC0-21C6-4F64-B641-31E85C8F5DFE}";

            if (!workflowState.StateID.Equals(analyticsDraftStateWorkflowId))
            {
                return;
            }

            const string analyticsDeployCommandId = "{4044A9C4-B583-4B57-B5FF-2791CB0351DF}";
            var workflowResult = workflow.Execute(analyticsDeployCommandId, eventItem, "Deploying UrlRewrite Redirect event during initialization", false, new object[0]);

        }



    }



}