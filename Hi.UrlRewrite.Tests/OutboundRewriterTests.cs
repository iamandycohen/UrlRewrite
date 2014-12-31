using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.UrlRewrite.Entities.Actions;
using Hi.UrlRewrite.Processing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Hi.UrlRewrite.Entities.Match;
using Hi.UrlRewrite.Entities.Rules;

namespace Hi.UrlRewrite.Tests
{
    [TestClass]
    public class OutboundRewriterTests
    {

        [TestMethod]
        public void ProcessRuleReplacementsWithMatchTags()
        {
            // arrange
            var responseString = OutboundRewriterTestData.ProcessRuleReplacementsWithMatchTagsInput;
            var outboundRule = new OutboundRule
            {
                MatchTheContentWithin = new List<MatchTag>
                {
                    new MatchTag {Tag = "a", Attribute = "href"}
                },
                Pattern = @"/article\.aspx\?id=([0-9]+)(?:&|&amp;)title=([_0-9a-z-]+)$",
                Action = new OutboundRewriteAction()
                {
                    Value = @"/article/{R:1}/{R:2}"
                }
            };

            // act
            var output = OutboundRewriter.ProcessRuleReplacements(responseString, outboundRule);

            // assert
            Assert.AreEqual(output, OutboundRewriterTestData.ProcessRuleReplacementsWithMatchTagsExpectedOutput);
        }

        [TestMethod]
        public void ProcessRuleReplacementsWithNoMatchTags()
        {
            // arrange
            var responseString = OutboundRewriterTestData.ProcessRuleReplacementsWithMatchTagsInput;
            var outboundRule = new OutboundRule
            {
                Pattern = @"/article\.aspx\?id=([0-9]+)(?:&|&amp;)title=([_0-9a-z-]+)",
                Action = new OutboundRewriteAction()
                {
                    Value = @"/article/{R:1}/{R:2}"
                }
            };

            // act
            var output = OutboundRewriter.ProcessRuleReplacements(responseString, outboundRule);

            // assert
            Assert.AreEqual(output, OutboundRewriterTestData.ProcessRuleReplacementsWithMatchTagsExpectedOutput);
        }

        [TestMethod]
        public void ProcessRuleReplacementsWithExactMatch()
        {
            // arrange
            var responseString = OutboundRewriterTestData.ProcessRuleReplacementsWithMatchTagsInput;
            var outboundRule = new OutboundRule
            {
                Using = Using.ExactMatch,
                Pattern = @"</body>",
                Action = new OutboundRewriteAction()
                {
                    Value = @"<script type='text/javascript'>//Your web analytics tracking code goes here...</script></body>"
                }
            };

            // act
            var output = OutboundRewriter.ProcessRuleReplacements(responseString, outboundRule);

            // assert
            Assert.AreEqual(output, OutboundRewriterTestData.ProcessRuleReplacementsWithExactMatchExpectedOutput);
        }

        [TestMethod]
        public void ProcessRuleReplacementsWithMatchTagsAndExactMatch()
        {
            // arrange
            var responseString = OutboundRewriterTestData.ProcessRuleReplacementsWithMatchTagsInput;
            var outboundRule = new OutboundRule
            {
                Using = Using.ExactMatch,
                MatchTheContentWithin = new List<MatchTag>
                {
                    new MatchTag {Tag = "a", Attribute = "href"}
                },
                Pattern = @"/article\.aspx\?id=([0-9]+)(?:&|&amp;)title=([_0-9a-z-]+)$",
                Action = new OutboundRewriteAction()
                {
                    Value = @"/newarticle.aspx"
                }
            };

            // act
            var output = OutboundRewriter.ProcessRuleReplacements(responseString, outboundRule);

            // assert
            Assert.AreEqual(output, OutboundRewriterTestData.ProcessRuleReplacementsWithMatchTagsAndExactMatchExpectedOutput);
        }
    }
}
