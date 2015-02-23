using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Hi.UrlRewrite.Processing.Results;
using Hi.UrlRewrite.Templates;
using Hi.UrlRewrite.Templates.Reporting;
using Sitecore;
using Sitecore.Data;
using Sitecore.Jobs;

namespace Hi.UrlRewrite.Reporting
{
    public class ReportingService
    {

        public List<Job> QueueReport(IEnumerable<InboundRuleResult> ruleResults, Database database)
        {
            var jobs = new List<Job>();
            var siteName = Context.Site.Name;

            ruleResults
                .AsParallel()
                .ForAll(result =>
            {
                var jobName = string.Format("UrlRewrite::Reporting - Saving rewrite info for item '{0}'", result.ItemId);
                var jobOptions = new JobOptions(jobName, "UrlRewrite", siteName, this, "SaveRewriteReport", new object[] { result, database })
                                                {
                                                    WriteToLog = false
                                                };

                var job = JobManager.Start(jobOptions);

                jobs.Add(job);
            });

            return jobs;
        }

        public void SaveRewriteReport(InboundRuleResult ruleResult, Database database)
        {
            var reportingFolder = database.GetItem(new ID(Constants.ReportingFolder_ItemId));
            var uniqueId = new ID(Guid.NewGuid()).ToShortID().ToString();
            var rule = database.GetItem(new ID(ruleResult.ItemId));

            var values = new NameValueCollection
            {
                {"Rule", rule.Paths.Path},
                {"Original Url", ruleResult.OriginalUri.ToString()},
                {"Rewritten Url", ruleResult.RewrittenUri.ToString()},
                {"Database Name", database.Name}
            };

            var url = string.Format("{0}/-/item/v1/{1}?name={2}&template={3}&sc_database=master", Configuration.ItemWebApiHost, reportingFolder.Paths.Path, uniqueId, RewriteReportItem.TemplateId).ToLower();

            using (var client = new WebClient())
            {
                client.Headers.Add("X-Scitemwebapi-Username", Configuration.ItemWebApiUser);
                client.Headers.Add("X-Scitemwebapi-Password", Configuration.ItemWebApiPassword);
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.UploadValues(url, "POST", values);
            }

        }

    }
}