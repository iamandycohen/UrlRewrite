using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Hi.UrlRewrite.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;

namespace Hi.UrlRewrite.Tests
{
    [TestClass]
    public class UrlRewriterTests
    {

        List<InboundRule> InboundRules { get; set; } 

        [TestInitialize]
        public void Initialize()
        {
            InboundRules = new List<InboundRule>()
            {
                new InboundRule()
                {
                    Action = new RedirectAction()
                    {
                        AppendQueryString = true,
                        HttpCacheability = HttpCacheability.NoCache,
                        Name = "RedirectAction 1",
                        RedirectType = RedirectType.Permanent,
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
                        RedirectType = RedirectType.Permanent,
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
                        RedirectType = RedirectType.Permanent,
                        RewriteUrl = "http://{HTTP_HOST}/tested",
                        StopProcessingOfSubsequentRules = false
                    },
                    Enabled = true,
                    IgnoreCase = true,
                    Name = "Inbound Rule 3",
                    Pattern = "^test$",
                    Using = Using.RegularExpressions,
                    RequestedUrl = RequestedUrl.MatchesThePattern,
                    ConditionLogicalGrouping = LogicalGrouping.MatchAll
                }
            };
        }

        [TestMethod]
        public void ProcessRequestUrlTest()
        {
            var rewriter = new UrlRewriter();
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/john"), InboundRules);

            var firstInboundRule = InboundRules.First();
            var actionToCompare = firstInboundRule.Action as RedirectAction;
            var actionRewriteUrl = actionToCompare.RewriteUrl;
            var actionRewriteUri = new Uri(actionRewriteUrl);

            Assert.AreEqual(actionRewriteUri, rewriteResult.RewrittenUri);

        }

        [TestMethod]
        public void ProcessRequestUrlWithCaptureGroupsTest()
        {
            var rewriter = new UrlRewriter();
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/article/1/2"), InboundRules);

            var expectedUri = new Uri("http://fictitioussite.com/article.aspx?id=1&amp;title=2");

            Assert.AreEqual(expectedUri, rewriteResult.RewrittenUri);
        }

        [TestMethod]
        public void ProcessRequestUrlWithHttpHostReplacementTest()
        {
            var rewriter = new UrlRewriter();
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/test"), InboundRules);

            var expectedUri = new Uri("http://fictitioussite.com/tested");

            Assert.AreEqual(expectedUri, rewriteResult.RewrittenUri);
        }
    }
}
