using Hi.UrlRewrite.Processing.Results;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Jobs
{
    public class ReportingService
    {

        public List<Job> StartJob(ProcessInboundRulesResult ruleResult)
        {
            var jobs = new List<Job>();
            var rewrites = ruleResult.ProcessedResults.Where(e => e.RuleMatched);

            foreach (var rewrite in rewrites)
            {
                var jobName = string.Format("UrlRewrite::Reporting - Saving rewrite info for item '{0}'", rewrite.ItemId);


                JobOptions options = new JobOptions(jobName,
                                                    "UrlRewrite",
                                                    Context.Site.Name,
                                                    this,
                                                    "SaveRewriteInfo",
                                                    new object[] { rewrite });

                var job = JobManager.Start(options);

                jobs.Add(job);
            }

            return jobs;
        }

        public void SaveRewriteInfo(InboundRuleResult ruleResult)
        {
            Log.Info(this, "Logging Reporting rewrite {0}", ruleResult.ItemId);
        }
    }
}