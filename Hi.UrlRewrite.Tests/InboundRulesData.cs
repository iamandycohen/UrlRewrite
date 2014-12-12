using System.Web;
using Hi.UrlRewrite.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Tests
{
    public class InboundRulesData
    {
        public static List<InboundRule> GetInboundRulesData()
        {
            return new List<InboundRule>()
            {
                new InboundRule()
                {
                    Action = new RedirectAction()
                    {
                        AppendQueryString = true,
                        HttpCacheability = HttpCacheability.NoCache,
                        Name = "RedirectAction 1",
                        RedirectType = RedirectActionStatusCode.Permanent,
                        RewriteUrl = "http://www.google.com",
                        StopProcessingOfSubsequentRules = false
                    },
                    Enabled = true,
                    IgnoreCase = true,
                    Name = "Inbound Rule 1",
                    Pattern = "john",
                    Using = Using.ExactMatch,
                    RequestedUrl = RequestedUrl.MatchesThePattern,
                    ConditionLogicalGrouping = LogicalGrouping.MatchAll
                },
                new InboundRule()
                {
                    Action = new RedirectAction()
                    {
                        AppendQueryString = true,
                        HttpCacheability = HttpCacheability.NoCache,
                        Name = "RedirectAction 1",
                        RedirectType = RedirectActionStatusCode.Permanent,
                        RewriteUrl = "http://{HTTP_HOST}/article.aspx?id={R:1}&amp;title={R:2}",
                        StopProcessingOfSubsequentRules = false
                    },
                    Enabled = true,
                    IgnoreCase = true,
                    Name = "Inbound Rule 2",
                    Pattern = "^article/([0-9]+)/([_0-9a-z-]+)",
                    Using = Using.RegularExpressions,
                    RequestedUrl = RequestedUrl.MatchesThePattern,
                    ConditionLogicalGrouping = LogicalGrouping.MatchAll
                },
                new InboundRule()
                {
                    Action = new RedirectAction()
                    {
                        AppendQueryString = true,
                        HttpCacheability = HttpCacheability.NoCache,
                        Name = "RedirectAction 1",
                        RedirectType = RedirectActionStatusCode.Permanent,
                        RewriteUrl = "http://{HTTP_HOST}/hostreplaced",
                        StopProcessingOfSubsequentRules = false
                    },
                    Enabled = true,
                    IgnoreCase = true,
                    Name = "Inbound Rule 3",
                    Pattern = "^hostreplacement$",
                    Using = Using.RegularExpressions,
                    RequestedUrl = RequestedUrl.MatchesThePattern,
                    ConditionLogicalGrouping = LogicalGrouping.MatchAll
                }
            };
        }
    }
}
