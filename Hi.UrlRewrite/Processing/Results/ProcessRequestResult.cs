using System;
using System.Collections.Generic;
using System.Web;
using Hi.UrlRewrite.Entities.Actions;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ProcessRequestResult
    {

        public ProcessRequestResult(Uri originalUri, RuleResult finalRuleResult, bool matchedAtLeastOneRule, List<RuleResult> processedResults)
        {
            this.OriginalUri = originalUri;
            this.RewrittenUri = finalRuleResult.RewrittenUri;
            this.MatchedAtLeastOneRule = matchedAtLeastOneRule;
            this.ProcessedResults = processedResults;
            this.FinalAction = finalRuleResult.ResultAction;
        }

        public Uri OriginalUri { get; set; }
        public Uri RewrittenUri { get; set; }

        public int? StatusCode 
        { 
            get 
            {
                if (FinalAction is RedirectAction)
                {
                    var redirectAction = FinalAction as RedirectAction;
                    if (redirectAction.StatusCode.HasValue)
                    {
                        return (int) (redirectAction.StatusCode.Value);
                    }
                }
                else if (FinalAction is CustomResponseAction)
                {
                    var customResponse = FinalAction as CustomResponseAction;
                    return customResponse.StatusCode;
                }

                return null;
            }
        }

        public BaseAction FinalAction { get; set; }

        public bool MatchedAtLeastOneRule { get; set; }
        public List<RuleResult> ProcessedResults { get; set; }

    }
}
