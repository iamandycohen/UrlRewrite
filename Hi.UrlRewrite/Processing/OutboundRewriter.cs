﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Module;
using Hi.UrlRewrite.Processing.Results;
using Hi.UrlRewrite.Entities.Match;

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
            var isRuleMatch = TestRuleMatches(ruleResult.RewrittenResponseString, outboundRule, out outboundRuleMatch);
            ConditionMatchResult conditionMatchResult = null;

            // test conditions matches
            if (isRuleMatch && outboundRule.Conditions != null && outboundRule.Conditions.Count > 0)
            {
                var replacements = new RewriteHelper.Replacements
                {
                    RequestHeaders = RequestHeaders,
                    RequestServerVariables = RequestServerVariables
                };

                conditionMatchResult = RewriteHelper.TestConditionMatches(outboundRule, replacements, out lastConditionMatch);
                isRuleMatch = conditionMatchResult.Matched;
            }

            if (isRuleMatch)
            {
                ruleResult.RuleMatched = true;
            }

            return ruleResult;
        }

        private bool TestRuleMatches(string responseString, OutboundRule outboundRule, out Match outboundRuleMatch)
        {
            // TODO: test against all of the "match the content within"


            outboundRuleMatch = new Regex(@"").Match(@"");
            return true;
        }

        internal PreconditionResult CheckPreconditions(HttpContextBase httpContext, List<OutboundRule> outboundRules)
        {

            var originalUri = httpContext.Request.Url;
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