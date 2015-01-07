using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Actions.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ProcessInboundRulesResult
    {

        private readonly Uri _originalUri;
        private readonly Uri _rewrittenUri;
        private readonly IBaseAction _finalAction;
        private readonly List<InboundRuleResult> _processedResults;
        private readonly Guid? _itemId;

        public ProcessInboundRulesResult(Uri originalUri, List<InboundRuleResult> processedResults)
        {
            _originalUri = originalUri;
            _processedResults = processedResults;
            var lastMatchedResult = _processedResults.FirstOrDefault(r => r.RuleMatched);

            if (lastMatchedResult != null)
            {
                _rewrittenUri = lastMatchedResult.RewrittenUri;
                _finalAction = lastMatchedResult.ResultAction;
                _itemId = lastMatchedResult.ItemId;
            }
        }

        public Guid? ItemId
        {
            get
            {
                return _itemId;
            }
        }

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
                if (FinalAction is Redirect)
                {
                    var redirectAction = FinalAction as Redirect;
                    if (redirectAction.StatusCode.HasValue)
                    {
                        return (int) (redirectAction.StatusCode.Value);
                    }
                }
                else if (FinalAction is CustomResponse)
                {
                    var customResponse = FinalAction as CustomResponse;
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

        public List<InboundRuleResult> ProcessedResults
        {
            get
            {
                return _processedResults;
            }
        }

    }
}
