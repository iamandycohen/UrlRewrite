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

        public Job StartJob()
        {
            var jobName = string.Format("UrlRewrite::Reporting - Saving rewrite info");

            JobOptions options = new JobOptions(jobName,
                                                "UrlRewrite",
                                                Context.Site.Name,
                                                this,
                                                "SaveRewriteInfo",
                                                new object[] {  });
            return JobManager.Start(options);
        }

        public void SaveRewriteInfo(Item root)
        {
           
        }
    }
}