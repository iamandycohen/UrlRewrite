using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Module;
using Hi.UrlRewrite.Processing.Results;

namespace Hi.UrlRewrite.Processing
{
    public class OutboundRewriter
    {

        public ProcessOutboundRulesResult ProcessContext(HttpContextBase httpContext, string responseString, List<OutboundRule> outboundRules)
        {

            if (httpContext == null) throw new ArgumentNullException("httpContext");
            if (outboundRules == null) throw new ArgumentNullException("outboundRules");

            // process outbound rules here... only set up event if it matches rules and preconditions

            // check rule matches

            // check conditions

            var result = new ProcessOutboundRulesResult
            {
                ResponseString = responseString
            };

            return result;
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