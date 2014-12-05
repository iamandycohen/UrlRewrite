using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hi.UrlRewrite
{
    public class ProcessRequestResult
    {

        public ProcessRequestResult(RuleResult ruleResult, bool hasOneRedirect, List<RuleResult> processedResults)
        {
            this.OriginalUri = ruleResult.OriginalUri;
            this.HttpCacheability = ruleResult.HttpCacheability;
            this.RewrittenUri = ruleResult.RewrittenUri;
            this.StatusCode = ruleResult.StatusCode;
            this.HasOneRedirect = hasOneRedirect;
            this.ProcessedResults = processedResults;
        }


        public Uri OriginalUri { get; set; }

        public HttpCacheability? HttpCacheability { get; set; }

        public Uri RewrittenUri { get; set; }

        public int? StatusCode { get; set; }

        public bool HasOneRedirect { get; set; }

        public List<RuleResult> ProcessedResults { get; set; }
    }
}
