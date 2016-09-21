using BotTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RezaBot.Rules.Sitecore;
using System.Collections.Generic;

namespace BotTests
{
    [TestClass]
    public class WhitespacesTests
    {
        [TestMethod]
        public void Exists()
        {
            var rule = GetRule();

            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void WhitespacesBetweenTags()
        {
            var file = TestHelpers.CreateTestFile("cshtml", "+ <div  class=\"test\">");

            GetRule().AssertFoundIssue(file, file.ChangedLines);
        }

        [TestMethod]
        public void WhitespacesAroundSemicolon()
        {
            var file = TestHelpers.CreateTestFile("cs", "+ return ;");

            GetRule().AssertFoundIssue(file, file.ChangedLines);

            file = TestHelpers.CreateTestFile("cs", "+ return; ");

            GetRule().AssertFoundIssue(file, file.ChangedLines);
        }

        [TestMethod]
        public void WhitespacesInMethods()
        {
            var file = TestHelpers.CreateTestFile("cs", "+ void  TestMethod()");

            GetRule().AssertFoundIssue(file, file.ChangedLines);
        }

        [TestMethod]
        public void NoExtraWhitespaces()
        {
            var file = TestHelpers.CreateTestFile("cs", "+ void TestMethod()");

            GetRule().AssertIsIssueFree(file, file.ChangedLines);
        }

        private Whitespaces GetRule()
        {
            return TestHelpers.GetRule<Whitespaces>();
        }
    }
}
