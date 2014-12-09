using Hi.UrlRewrite.Entities;
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

        public ProcessRequestResult(Uri originalUri, RuleResult finalRuleResult, bool matchedAtLeastOneRule, List<RuleResult> processedResults)
        {
            this.OriginalUri = originalUri;
            this.RewrittenUri = finalRuleResult.RewrittenUri;

            this.HttpCacheability = finalRuleResult.HttpCacheability;
            this.StatusCode = finalRuleResult.StatusCode;
            this.Abort = finalRuleResult.Abort;
            this.CustomResponse = finalRuleResult.CustomResponse;

            this.MatchedAtLeastOneRule = matchedAtLeastOneRule;
            this.ProcessedResults = processedResults;
        }

        public Uri OriginalUri { get; set; }
        public Uri RewrittenUri { get; set; }

        public HttpCacheability? HttpCacheability { get; set; }
        public int? StatusCode { get; set; }
        public bool Abort { get; set; }
        public CustomResponseAction CustomResponse { get; set; }

        public bool MatchedAtLeastOneRule { get; set; }
        public List<RuleResult> ProcessedResults { get; set; }


    }
}
