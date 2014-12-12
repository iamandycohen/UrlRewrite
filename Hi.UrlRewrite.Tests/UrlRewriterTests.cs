using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Hi.UrlRewrite.Entities;
using Hi.UrlRewrite.Processing;
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
            InboundRules = InboundRulesData.GetInboundRulesData();
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
        public void ProcessRequestUrlWithCaptureGroups()
        {
            var rewriter = new UrlRewriter();
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/article/1/2"), InboundRules);

            var expectedUri = new Uri("http://fictitioussite.com/article.aspx?id=1&amp;title=2");

            Assert.AreEqual(expectedUri, rewriteResult.RewrittenUri);
        }

        [TestMethod]
        public void ProcessRequestUrlWithHttpHostReplacement()
        {
            var rewriter = new UrlRewriter();
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/hostreplacement"), InboundRules);

            var expectedUri = new Uri("http://fictitioussite.com/hostreplaced");

            Assert.AreEqual(expectedUri, rewriteResult.RewrittenUri);
        }

        [TestMethod]
        public void ProcessRequestUrlWithAbort()
        {
            var rewriter = new UrlRewriter();
            var newInboundRule = new InboundRule()
            {
                Name = "Abort Rule",
                Pattern = "^abort$",
                Using = Using.RegularExpressions,
                Action = new AbortRequestAction() { Name = "Abort Action" },
                RequestedUrl = RequestedUrl.MatchesThePattern
            };

            InboundRules.Insert(1, newInboundRule);

            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/abort"), InboundRules);

            Assert.IsTrue(rewriteResult.Abort);
            Assert.IsTrue(rewriteResult.ProcessedResults.Count == 2);
        }

        [TestMethod]
        public void ProcessRequestUrlWithCustomResponse()
        {
            var rewriter = new UrlRewriter();
            var newInboundRule = new InboundRule()
            {
                Name = "Custom Response Rule",
                Pattern = "customresponse",
                Using = Using.ExactMatch,
                Action = new CustomResponseAction() { Name = "Custom Response Action", StatusCode = 550, SubStatusCode = 100, ErrorDescription = "Custom Response Because I Said So", Reason = "Custom Response 550"},
                RequestedUrl = RequestedUrl.MatchesThePattern
            };

            InboundRules.Insert(0, newInboundRule);

            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/customresponse"), InboundRules);

            Assert.IsTrue(rewriteResult.CustomResponse != null);
            Assert.AreEqual(rewriteResult.CustomResponse.StatusCode, 550);
            Assert.AreEqual(rewriteResult.CustomResponse.SubStatusCode, 100);
            Assert.IsTrue(rewriteResult.ProcessedResults.Count == 1);
        }

        [TestMethod]
        public void ProcessRequestUrlWithMultipleConditionMatchBackReferences()
        {
            var rewriter = new UrlRewriter();

            InboundRules = new List<InboundRule>()
            {
                new InboundRule()
                {
                    Name = "Multiple Variable Rules",
                    Pattern = "^(.*)$",
                    Using = Using.RegularExpressions,
                    Action = new RedirectAction()
                    {
                        Name = "Redirect to C1 and C2",
                        AppendQueryString = false,
                        HttpCacheability = HttpCacheability.NoCache,
                        RedirectType = RedirectType.Permanent,
                        RewriteUrl = "http://{HTTP_HOST}/newpage/{C:1}/{C:2}"
                    },
                    RequestedUrl = RequestedUrl.MatchesThePattern,
                    ConditionLogicalGrouping = LogicalGrouping.MatchAll,
                    Conditions = new List<Condition>()
                    {
                        new Condition()
                        {
                            Name = "C1",
                            CheckIfInputString = CheckIfInputStringType.MatchesThePattern,
                            InputString = "{QUERY_STRING}",
                            Pattern = @"(?:^|&)var1=(\d+)(?:&|$)",
                            IgnoreCase = true
                        },
                        new Condition()
                        {
                            Name = "C2",
                            CheckIfInputString = CheckIfInputStringType.MatchesThePattern,
                            InputString = "%{C:1}%_{QUERY_STRING}",
                            Pattern = @"%(.+)%_.*var2=(\d+)(?:&|$)",
                            IgnoreCase = true
                        }
                    }
                }

            };

            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/?var1=1&var2=2"), InboundRules);

            var expectedUrl = new Uri("http://fictitioussite.com/newpage/1/2");

            Assert.AreEqual(expectedUrl, rewriteResult.RewrittenUri);
        }

    }
}
