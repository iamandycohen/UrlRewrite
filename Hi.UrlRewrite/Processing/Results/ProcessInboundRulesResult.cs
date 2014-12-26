using System;
using System.Collections.Generic;
using System.Linq;
using Hi.UrlRewrite.Entities.Actions;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ProcessInboundRulesResult
    {

        public ProcessInboundRulesResult(Uri originalUri, List<RuleResult> processedResults)
        {
            _originalUri = originalUri;
            _processedResults = processedResults;
            var lastMatchedResult = _processedResults.FirstOrDefault(r => r.RuleMatched);

            if (lastMatchedResult != null)
            {
                _rewrittenUri = lastMatchedResult.RewrittenUri;
                _finalAction = lastMatchedResult.ResultAction;
            }
        }

        private readonly Uri _originalUri;
        private readonly Uri _rewrittenUri;
        private readonly IBaseAction _finalAction;
        private readonly List<RuleResult> _processedResults;

        public Uri OriginalUri
        {
            get
            {
                return _originalUri;
            }
        }

        public Uri RewrittenUri
        {
            get
            {
                return _rewrittenUri;
            }
        }

        public IBaseAction FinalAction
        {
            get
            {
                return _finalAction;
            }
        }

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

        public bool MatchedAtLeastOneRule
        {
            get
            {
                return ProcessedResults != null && ProcessedResults.Any(e => e.RuleMatched);
            }
        }

        public List<RuleResult> ProcessedResults
        {
            get
            {
                return _processedResults;
            }
        }

    }
}
