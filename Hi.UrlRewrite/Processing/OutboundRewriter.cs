using System;
using System.Collections.Generic;
using System.Linq;
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

            // TODO: Check preconditions

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
            var result = new PreconditionResult { Passed = true };
            return result;
        }
    }
}