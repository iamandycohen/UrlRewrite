using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite
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
    }
}
