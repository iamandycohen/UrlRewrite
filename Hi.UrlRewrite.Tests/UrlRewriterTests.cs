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
                    LogicalGrouping = LogicalGrouping.MatchAll
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
    }
}
