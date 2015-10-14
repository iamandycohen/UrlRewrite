using log4net.Filter;
using log4net.spi;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Hi.UrlRewrite
{
    public class MyFilter : StringMatchFilter
    {

        public override FilterDecision Decide(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
                throw new ArgumentNullException("loggingEvent");

            var exceptionMessage = loggingEvent.GetExceptionStrRep();
            if (string.IsNullOrEmpty(exceptionMessage))
            {
                return FilterDecision.NEUTRAL;
            }

            if (RegexToMatch != null)
            {
                var regex = new Regex(RegexToMatch, RegexOptions.Multiline);

                if (!regex.Match(exceptionMessage).Success)
                    return FilterDecision.NEUTRAL;

                return AcceptOnMatch ? FilterDecision.ACCEPT : FilterDecision.DENY;
            }

            if (StringToMatch == null || exceptionMessage.IndexOf(StringToMatch) == -1)
            {
                return FilterDecision.NEUTRAL;
            }

            return AcceptOnMatch ? FilterDecision.ACCEPT : FilterDecision.DENY;
        }

        public override void ActivateOptions()
        {
 	         base.ActivateOptions();
        }
    }
}