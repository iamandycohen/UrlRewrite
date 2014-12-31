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
        public void ProcessRuleReplacementsTest()
        {
            // arrange
            var responseString = OutboundRewriterTestData.ProcessRuleReplacementsTestInput;
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
            Assert.AreEqual(output, OutboundRewriterTestData.ProcessRuleReplacementsTestExpectedOutput);

        }
    }
}
