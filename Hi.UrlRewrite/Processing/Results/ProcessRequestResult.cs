using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hi.UrlRewrite.Entities.Actions;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ProcessRequestResult
    {

        public ProcessRequestResult(Uri originalUri, RuleResult finalRuleResult, List<RuleResult> processedResults)
        {
            OriginalUri = originalUri;
            ProcessedResults = processedResults;

            if (finalRuleResult != null)
            {
                RewrittenUri = finalRuleResult.RewrittenUri;
                FinalAction = finalRuleResult.ResultAction;
            }
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

        public IBaseAction FinalAction { get; set; }

        public bool MatchedAtLeastOneRule
        {
            get
            {
                return ProcessedResults != null && ProcessedResults.Any(e => e.RuleMatched);
            }
        }

        public List<RuleResult> ProcessedResults { get; set; }

    }
}
