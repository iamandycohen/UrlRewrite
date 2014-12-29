using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Processing.Results
{
    public class ProcessOutboundRulesResult
    {

        private readonly List<OutboundRuleResult> _processedResults;
        private readonly string _rewrittenResponseString;

        public ProcessOutboundRulesResult(List<OutboundRuleResult> processedResults)
        {
            _processedResults = processedResults;
            var lastMatchedResult = _processedResults.FirstOrDefault(r => r.RuleMatched);

            if (lastMatchedResult != null)
            {
                _rewrittenResponseString = lastMatchedResult.RewrittenResponseString;
                //_finalAction = lastMatchedResult.ResultAction;
            }

        }

        public string ResponseString
        {
            get
            {
                return _rewrittenResponseString;
            }
        }

        public bool MatchedAtLeastOneRule
        {
            get
            {
                return ProcessedResults != null && ProcessedResults.Any(e => e.RuleMatched);
            }
        }

        public List<OutboundRuleResult> ProcessedResults
        {
            get
            {
                return _processedResults;
            }
        }

    }
}