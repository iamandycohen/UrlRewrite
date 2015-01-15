using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Entities.Reporting
{
    public class RewriteReport
    {
        public string OriginalUrl { get; set; }
        public string RewrittenUrl { get; set; }
        public DateTime RewriteDate { get; set; }
        public string DatabaseName { get; set; }
        //public InboundRule Rule { get; set; }
        public string RulePath { get; set; }
    }
}