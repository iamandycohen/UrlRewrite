using Hi.UrlRewrite.Processing.Results;
using Hi.UrlRewrite.Templates;
using Sitecore;
using Sitecore.Data;
using Sitecore.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hi.UrlRewrite.Jobs
{
    public class ReportingService
    {

        public List<Job> ReportRewrites(IEnumerable<InboundRuleResult> ruleResults, Database database)
        {
            var jobs = new List<Job>();
            var siteName = Context.Site.Name;

            ruleResults
                .AsParallel()
                .ForAll(result =>
            {
                var jobName = string.Format("UrlRewrite::Reporting - Saving rewrite info for item '{0}'", result.ItemId);


                var jobOptions = new JobOptions(jobName, "UrlRewrite", siteName, this, "SaveRewriteInfo", new object[] { result, database })
                                                {
                                                    WriteToLog = false
                                                };

                var job = JobManager.Start(jobOptions);

                jobs.Add(job);
            });

            return jobs;
        }

        public void SaveRewriteInfo(InboundRuleResult ruleResult, Database database)
        {
            var reportingFolder = database.GetItem(new ID(Constants.ReportingFolder_ItemId));
            var uniqueId = new ID(Guid.NewGuid()).ToShortID().ToString();
            var reportingItem = reportingFolder.Add(uniqueId, new TemplateID(new ID(RewriteReportItem.TemplateId)));
            var report = new RewriteReportItem(reportingItem);
            report.BeginEdit();

            report.OriginalUrl.Value = ruleResult.OriginalUri.ToString();
            report.RewrittenUrl.Value = ruleResult.RewrittenUri.ToString();
            report.RewriteDate.InnerField.Value = DateUtil.ToIsoDate(DateTime.Now);
            report.DatabaseName.Value = database.Name;
            var ruleItem = database.GetItem(new ID(ruleResult.ItemId));
            report.Rule.InnerField.Value = ruleItem.Paths.Path;

            report.EndEdit();
        }
    }
}