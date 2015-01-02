using System.Collections.Specialized;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;
using Hi.UrlRewrite.Processing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var serverVariables = new NameValueCollection
            {
                {"HTTP_HOST", "fictitioussite.com"}
            };
            var rewriter = new InboundRewriter(serverVariables, null);
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/john"), InboundRules);

            var firstInboundRule = InboundRules.First();
            var actionToCompare = firstInboundRule.Action as Redirect;
            var actionRewriteUrl = actionToCompare.RewriteUrl;
            var actionRewriteUri = new Uri(actionRewriteUrl);

            Assert.AreEqual(actionRewriteUri, rewriteResult.RewrittenUri);
        }

        [TestMethod]
        public void ProcessRequestUrlWithCaptureGroups()
        {
            var serverVariables = new NameValueCollection
            {
                {"HTTP_HOST", "fictitioussite.com"}
            };
            var rewriter = new InboundRewriter(serverVariables, null);
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/article/1/2"), InboundRules);

            var expectedUri = new Uri("http://fictitioussite.com/article.aspx?id=1&amp;title=2");

            Assert.AreEqual(expectedUri, rewriteResult.RewrittenUri);
        }

        [TestMethod]
        public void ProcessRequestUrlWithHttpHostReplacement()
        {
            var serverVariables = new NameValueCollection
            {
                {"HTTP_HOST", "fictitioussite.com"}
            };
            var rewriter = new InboundRewriter(serverVariables, null);
            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/hostreplacement"), InboundRules);

            var expectedUri = new Uri("http://fictitioussite.com/hostreplaced");

            Assert.AreEqual(expectedUri, rewriteResult.RewrittenUri);
        }

        [TestMethod]
        public void ProcessRequestUrlWithAbort()
        {
            var rewriter = new InboundRewriter();
            var newInboundRule = new InboundRule()
            {
                Name = "Abort Rule",
                Pattern = "^abort$",
                Using = Using.RegularExpressions,
                Action = new AbortRequest() { Name = "Abort Action" },
                MatchType = MatchType.MatchesThePattern
            };

            InboundRules.Insert(1, newInboundRule);

            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/abort"), InboundRules);

            Assert.IsInstanceOfType(rewriteResult.FinalAction, typeof(AbortRequest));
            Assert.IsTrue(rewriteResult.ProcessedResults.Count == 2);
        }

        [TestMethod]
        public void ProcessRequestUrlWithCustomResponse()
        {
            var rewriter = new InboundRewriter();
            var newInboundRule = new InboundRule()
            {
                Name = "Custom Response Rule",
                Pattern = "customresponse",
                Using = Using.ExactMatch,
                Action = new CustomResponse()
                {
                    Name = "Custom Response Action", 
                    StatusCode = 550, 
                    SubStatusCode = 100, 
                    ErrorDescription = "Custom Response Because I Said So", 
                    Reason = "Custom Response 550"
                },
                MatchType = MatchType.MatchesThePattern
            };

            InboundRules.Insert(0, newInboundRule);

            var rewriteResult = rewriter.ProcessRequestUrl(new Uri("http://fictitioussite.com/customresponse"), InboundRules);
            var customResponse = rewriteResult.FinalAction as CustomResponse;

            Assert.IsNotNull(customResponse);
            Assert.AreEqual(customResponse.StatusCode, 550);
            Assert.AreEqual(customResponse.SubStatusCode, 100);
            Assert.IsTrue(rewriteResult.ProcessedResults.Count == 1);
        }

        [TestMethod]
        public void ProcessRequestUrlWithMultipleConditionMatchBackReferences()
        {
            var serverVariables = new NameValueCollection
            {
                {"HTTP_HOST", "fictitioussite.com"},
                {"QUERY_STRING", "var1=1&var2=2"}
            };
            var rewriter = new InboundRewriter(serverVariables, null);

            InboundRules = new List<InboundRule>()
            {
                new InboundRule()
                {
                    Name = "Multiple Variable Rules",
                    Pattern = "^(.*)$",
                    Using = Using.RegularExpressions,
                    Action = new Redirect()
                    {
                        Name = "Redirect to C1 and C2",
                        AppendQueryString = false,
                        HttpCacheability = HttpCacheability.NoCache,
                        StatusCode = RedirectStatusCode.Permanent,
                        RewriteUrl = "http://{HTTP_HOST}/newpage/{C:1}/{C:2}"
                    },
                    MatchType = MatchType.MatchesThePattern,
                    ConditionLogicalGrouping = LogicalGrouping.MatchAll,
                    Conditions = new List<Condition>()
                    {
                        new Condition()
                        {
                            Name = "C1",
                            CheckIfInputString = CheckIfInputString.MatchesThePattern,
                            InputString = Tokens.QUERY_STRING.Formatted(),
                            Pattern = @"(?:^|&)var1=(\d+)(?:&|$)",
                            IgnoreCase = true
                        },
                        new Condition()
                        {
                            Name = "C2",
                            CheckIfInputString = CheckIfInputString.MatchesThePattern,
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

        [TestMethod]
        public void ProcessRequestUrlWithHttpsToHttp()
        {
            var serverVariables = new NameValueCollection
            {
                {"HTTP_HOST", "www.secured.com"},
                {"HTTPS", "on"}
            };
            var rewriter = new InboundRewriter(serverVariables, null);

            InboundRules = new List<InboundRule>()
            {
                new InboundRule()
                {
                    Name = "Redirect HTTPS to HTTP",
                    Action = new Redirect()
                    {
                        Name = "Redirect",
                        AppendQueryString = true,
                        StatusCode = RedirectStatusCode.Permanent,
                        HttpCacheability = HttpCacheability.NoCache,
                        StopProcessingOfSubsequentRules = true,
                        RewriteUrl = "http://{HTTP_HOST}/{R:1}"
                    },
                    Conditions = new List<Condition>()
                    {
                        new Condition()
                        {
                            Name = "HTTPS",
                            CheckIfInputString = CheckIfInputString.DoesNotMatchThePattern,
                            IgnoreCase = true,
                            InputString = "{HTTPS}",
                            Pattern = "off"
                        }
                    },
                    Enabled = true,
                    IgnoreCase = true,
                    Pattern = "(.*)",
                    ConditionLogicalGrouping = LogicalGrouping.MatchAll,
                    Using = Using.RegularExpressions,
                    MatchType = MatchType.MatchesThePattern
                }
            };

            var inputUrl = "www.secured.com/blah/asdfadsf.htm?test=test";
            var inputUri = new Uri("https://" + inputUrl);
            var result = rewriter.ProcessRequestUrl(inputUri, InboundRules);
            var expectedUri = new Uri("http://" + inputUrl);

            var rewrittenUri = result.RewrittenUri;

            Assert.IsNotNull(rewrittenUri);
            Assert.AreEqual(rewrittenUri, expectedUri);
        }

    }
}
