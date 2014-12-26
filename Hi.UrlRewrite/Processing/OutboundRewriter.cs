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

        public ProcessOutboundRulesResult ProcessContext(HttpContextBase httpContext, List<OutboundRule> outboundRules)
        {

            if (httpContext == null) throw new ArgumentNullException("httpContext");
            if (outboundRules == null) throw new ArgumentNullException("outboundRules");

            var result = new ProcessOutboundRulesResult();

            return result;
        }

        public void ExecuteResponse(HttpContextBase httpContext)
        {
            // process outbound rules here... only set up event if it matches rules and preconditions

            // TODO: Check preconditions

            // check rule matches

            // check conditions

            // perform rewrites

            var responseFilterStream = new ResponseFilterStream(httpContext.Response.Filter);
            responseFilterStream.TransformString += responseFilterStream_TransformString;

            httpContext.Response.Filter = responseFilterStream;

        }

        private string responseFilterStream_TransformString(string arg)
        {
            return arg;
        }
    }
}