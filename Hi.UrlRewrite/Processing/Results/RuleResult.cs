using System;
using System.Web;
using Hi.UrlRewrite.Entities;

namespace Hi.UrlRewrite.Processing.Results
{

    public class RuleResult
    {
        public int? StatusCode { get; set; }
        public HttpCacheability? HttpCacheability { get; set; }
        public bool Abort { get; set; }
        public bool StopProcessing { get; set; }
        public bool StoppedProcessing { get; set; }

        public Uri OriginalUri { get; set; }
        public Uri RewrittenUri { get; set; }

        public Guid ItemId { get; set; }
        public bool RuleMatched { get; set; }

        public CustomResponseAction CustomResponse { get; set; }
    }
}
