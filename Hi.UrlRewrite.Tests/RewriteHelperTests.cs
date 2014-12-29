using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hi.UrlRewrite.Processing;

namespace Hi.UrlRewrite.Tests
{

    [TestClass]
    public class RewriteHelperTests
    {
        [TestMethod]
        public void ReplaceTokens()
        {
            // arrange
            var replacements = new RewriteHelper.Replacements()
            {
                RequestServerVariables = new NameValueCollection()
                {
                    { "HTTPS", "off" },
                    { "HTTP_HOST", "www.iamandycohen.com"},
                    { "QUERY_STRING", "var1=1&var2=2"},
                },
                RequestHeaders = new NameValueCollection()
                {
                    { "X-Custom-Header", "This is a test"}
                },
                ResponseHeaders = new NameValueCollection()
                {
                    { "Content-Type", @"text/html"}
                }
            };

            var inputString = "test {QUERY_STRING} test {REQUEST_X_Custom_Header} test {RESPONSE_Content_Type} test {HTTPS} test";

            // act
            var replacementOutput = RewriteHelper.ReplaceTokens(replacements, inputString);

            // assert
            Assert.AreEqual(replacementOutput, @"test var1=1&var2=2 test This is a test test text/html test off test");
        }
    }
}
