using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Module;
using Hi.UrlRewrite.Processing.Results;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Actions;

namespace Hi.UrlRewrite.Processing
{
    public class OutboundRewriter
    {

        public NameValueCollection RequestServerVariables { get; set; }
        public NameValueCollection RequestHeaders { get; set; }
        public NameValueCollection ResponseHeaders { get; set; }

        public OutboundRewriter()
        {
            RequestServerVariables = new NameValueCollection();
            RequestHeaders = new NameValueCollection();
            ResponseHeaders = new NameValueCollection();
        }

        public void SetupReplacements(NameValueCollection requestServerVariables, NameValueCollection requestHeaders,
            NameValueCollection responseHeaders)
        {
            RequestServerVariables = requestServerVariables;
            RequestHeaders = requestHeaders;
            ResponseHeaders = responseHeaders;
        }

        public ProcessOutboundRulesResult ProcessContext(HttpContextBase httpContext, string responseString, List<OutboundRule> outboundRules)
        {

            if (httpContext == null) throw new ArgumentNullException("httpContext");
            if (outboundRules == null) throw new ArgumentNullException("outboundRules");

            // process outbound rules here... only set up event if it matches rules and preconditions

            var processedResults = new List<OutboundRuleResult>();
            var ruleResult = new OutboundRuleResult()
            {
                RewrittenResponseString = responseString
            };

            foreach (var outboundRule in outboundRules)
            {
                ruleResult = ProcessOutboundRule(httpContext, ruleResult.RewrittenResponseString, outboundRule);
                processedResults.Add(ruleResult);

                if (!ruleResult.RuleMatched)
                    continue;

                if (ruleResult.RuleMatched && ruleResult.StopProcessing)
                {
                    break;
                }
            }


            // check rule matches

            // check conditions

            var result = new ProcessOutboundRulesResult(processedResults);

            return result;
        }

        private OutboundRuleResult ProcessOutboundRule(HttpContextBase httpContext, string responseString, OutboundRule outboundRule)
        {
            //Log.Debug(this, "Processing inbound rule - requestUri: {0} inboundRule: {1}", originalUri, inboundRule.Name);

            var ruleResult = new OutboundRuleResult()
            {
                OriginalResponseString = responseString,
                RewrittenResponseString = responseString
            };

            switch (outboundRule.Using)
            {
                case Using.ExactMatch:
                case Using.RegularExpressions:
                case Using.Wildcards:
                    ruleResult = ProcessRegularExpressionOutboundRule(ruleResult, outboundRule);

                    break;
                //case Using.Wildcards:
                //    //TODO: Implement Wildcards
                //    throw new NotImplementedException("Using Wildcards has not been implemented");
                //    break;
            }

            //Log.Debug(this, "Processing inbound rule - requestUri: {0} inboundRule: {1} rewrittenUrl: {2}", ruleResult.OriginalUri, inboundRule.Name, ruleResult.RewrittenUri);

            //ruleResult.ItemId = inboundRule.ItemId;

            return ruleResult;
        }

        private OutboundRuleResult ProcessRegularExpressionOutboundRule(OutboundRuleResult ruleResult, OutboundRule outboundRule)
        {
            Match outboundRuleMatch,
                lastConditionMatch = null;

            // test rule match
            var isRuleMatch = true;
            ConditionMatchResult conditionMatchResult = null;

            // test conditions matches
            if (outboundRule.Conditions != null && outboundRule.Conditions.Count > 0)
            {
                var replacements = new RewriteHelper.Replacements
                {
                    RequestHeaders = RequestHeaders,
                    RequestServerVariables = RequestServerVariables,
                    ResponseHeaders = ResponseHeaders
                };

                conditionMatchResult = RewriteHelper.TestConditionMatches(outboundRule, replacements, out lastConditionMatch);
                isRuleMatch = conditionMatchResult.Matched;
            }

            if (isRuleMatch)
            {
                ruleResult.RewrittenResponseString = ProcessRuleReplacements(ruleResult.OriginalResponseString, outboundRule);
                ruleResult.RuleMatched = true;
            }

            return ruleResult;
        }

        public static string ProcessRuleReplacements(string responseString, OutboundRule outboundRule)
        {
            var output = responseString;
            var rewritePattern = outboundRule.Pattern;
            var rewriteValue = ((OutboundRewriteAction)outboundRule.Action).Value;
            var matchTags = outboundRule.MatchTheContentWithin ?? new List<MatchTag>();

            // if we are not matching on match tags, then we are doing matching on the entire response
            if (!matchTags.Any())
            {
                if (outboundRule.Using == Using.ExactMatch)
                {
                    output = responseString.Replace(rewritePattern, rewriteValue);
                }
                else
                {
                    var responseRegex = new Regex(rewritePattern);

                    output = responseRegex.Replace(responseString, match => RewriteHelper.ReplaceRuleBackReferences(match, rewriteValue));
                }
            }
            else
            {

                output = ProcessRuleReplacementsWithMatchTags(responseString, outboundRule.Using, matchTags, rewritePattern, rewriteValue);
            }

            return output;
        }

        private static string ProcessRuleReplacementsWithMatchTags(string responseString, Using? outboundRuleUsing,
            IEnumerable<MatchTag> matchTags, string rewritePattern, string rewriteValue)
        {
            const string startKey = "start";
            const string innerKey = "inner";
            const string endKey = "end";
            const string nameKey = "name";
            const string startquoteKey = "startquote";
            const string valueKey = "value";
            const string endquoteKey = "endquote";
            string output = responseString;

            const string tagPatternFormat =
                @"(?<" + startKey + @"><{0}\s+)(?<" + innerKey + @">.*?{1}=(?:""|').*?)(?<" + endKey + @">\s*/?>)";

            const string attributePatternFormat =
                @"(?<" + nameKey + @">{0}=)(?<" + startquoteKey + @">""|')(?<" + valueKey + @">.*?)(?<" +
                endquoteKey + @">""|')";

            foreach (var matchTag in matchTags)
            {
                var tag = matchTag.Tag;
                var attribute = matchTag.Attribute;
                var tagPattern = string.Format(tagPatternFormat, tag, attribute);
                var tagRegex = new Regex(tagPattern);

                output = tagRegex.Replace(responseString, tagMatch =>
                {
                    var tagMatchGroups = tagMatch.Groups;
                    var tagStart = tagMatchGroups[startKey].Value;
                    var tagInnards = tagMatchGroups[innerKey].Value;
                    var tagEnd = tagMatchGroups[endKey].Value;
                    var attributePattern = string.Format(attributePatternFormat, attribute);
                    var attributeRegex = new Regex(attributePattern);

                    var newTagInnards = attributeRegex.Replace(tagInnards, attributeMatch =>
                    {
                        var attributeMatchGroups = attributeMatch.Groups;
                        var attributeValue = attributeMatchGroups[valueKey].Value;

                        var attributeValueRegex = new Regex(rewritePattern);
                        var attributeValueMatch = attributeValueRegex.Match(attributeValue);

                        if (attributeValueMatch.Success)
                        {
                            var attributeName = attributeMatchGroups[nameKey].Value;
                            var attributeStartQuote = attributeMatchGroups[startquoteKey].Value;
                            var attributeEndQuote = attributeMatchGroups[endquoteKey].Value;

                            // need to determine where the match occurs within the original string
                            var attributeValueMatchIndex = attributeValueMatch.Index;
                            var attributeValueMatchLength = attributeValueMatch.Length;
                            string attributeValueReplaced;

                            if (outboundRuleUsing == Using.ExactMatch)
                            {
                                attributeValueReplaced = attributeValueMatch.Value.Replace(
                                    attributeValueMatch.Value, rewriteValue);
                            }
                            else
                            {
                                attributeValueReplaced = RewriteHelper.ReplaceRuleBackReferences(attributeValueMatch,
                                    rewriteValue);
                            }

                            var newAttributeValue = attributeValue.Substring(0, attributeValueMatchIndex) +
                                                       attributeValueReplaced +
                                                       attributeValue.Substring(attributeValueMatchIndex +
                                                                                attributeValueMatchLength);

                            var attributeOutput = attributeName + attributeStartQuote + newAttributeValue +
                                                  attributeEndQuote;

                            return attributeOutput;
                        }

                        return attributeMatch.Value;
                    });

                    var tagOutput = tagStart + newTagInnards + tagEnd;

                    return tagOutput;
                });
            }
            return output;
        }

        internal PreconditionResult CheckPreconditions(HttpContextBase httpContext, List<OutboundRule> outboundRules)
        {

            Match lastConditionMatch = null;
            bool isPreconditionMatch = false;

            var preconditions = outboundRules.Select(p => p.Precondition)
                .Where(p => p != null);

            foreach (var precondition in preconditions)
            {
                var conditions = precondition.Conditions;

                // test conditions matches
                if (conditions != null && conditions.Count > 0)
                {
                    var replacements = new RewriteHelper.Replacements
                    {
                        RequestServerVariables = httpContext.Request.ServerVariables,
                        RequestHeaders = httpContext.Request.Headers,
                        ResponseHeaders = httpContext.Response.Headers
                    };
                    var conditionMatchResult = RewriteHelper.TestConditionMatches(precondition, replacements, out lastConditionMatch);
                    isPreconditionMatch = conditionMatchResult.Matched;
                }
            }

            var result = new PreconditionResult { Passed = isPreconditionMatch };

            return result;
        }
    }
}