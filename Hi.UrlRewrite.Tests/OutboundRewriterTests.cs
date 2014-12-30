using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi.UrlRewrite.Processing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Hi.UrlRewrite.Entities.Match;

namespace Hi.UrlRewrite.Tests
{
    [TestClass]
    public class OutboundRewriterTests
    {

        [TestMethod]
        public void TestRuleMatches()
        {
            // arrange
            var responseString = OutboundRewriterTestData.ResponseString;
            var matchScope = new Mock<IMatchScope>();
            var matchTags = new List<MatchTag>
            {
                new MatchTag {Tag = "a", Attribute = "href"}
            };
            matchScope.Setup(e => e.MatchTheContentWithin).Returns(matchTags);

            matchScope.Object.MatchTheContentWithin = matchTags;
            System.Text.RegularExpressions.Match match;

            // act
            OutboundRewriter.TestRuleMatches(responseString, matchScope.Object, out match);


            // assert
        }
    }
}
