using Hi.UrlRewrite.Processing.Results;
using Sitecore;
using Sitecore.Jobs;
using System.Collections.Generic;
using System.Linq;

namespace Hi.UrlRewrite.Jobs
{
    public class ReportingService
    {

        public List<Job> ReportRewrites(IEnumerable<InboundRuleResult> ruleResults)
        {
            var jobs = new List<Job>();

            ruleResults
                .AsParallel()
                .ForAll(result =>
            {
                var jobName = string.Format("UrlRewrite::Reporting - Saving rewrite info for item '{0}'", result.ItemId);


                JobOptions options = new JobOptions(jobName,
                                                    "UrlRewrite",
                                                    Context.Site.Name,
                                                    this,
                                                    "SaveRewriteInfo",
                                                    new object[] { result });

                var job = JobManager.Start(options);

                jobs.Add(job);
            });

            return jobs;
        }

        public void SaveRewriteInfo(InboundRuleResult ruleResult)
        {
            Log.Info(this, "Logging Reporting rewrite {0}", ruleResult.ItemId);
        }
    }
}