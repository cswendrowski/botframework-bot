using BotTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RezaBot.Rules.Sitecore;
using System.Collections.Generic;

namespace BotTests
{
    [TestClass]
    public class BracketTests
    {
        [TestMethod]
        public void Exists()
        {
            var rule = GetRule();

            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void BracketsOnSameLine()
        {
            var file = TestHelpers.CreateTestFile("cs", "+ void TestMethod() {");

            GetRule().AssertFoundIssue(file, file.ChangedLines);
        }

        [TestMethod]
        public void BracketsOnDifferentLine()
        {
            var code = new List<string>
            {
                "+ void TestMethod()",
                "+ {"
            };
            var file = TestHelpers.CreateTestFile("cs", code);

            GetRule().AssertIsIssueFree(file, file.ChangedLines);
        }

        [TestMethod]
        public void ExcludesCertainFiletypes()
        {
            var types = new List<string>
            {
                "item",
                "js",
                "cshtml",
                "proj"
            };

            foreach (var fileType in types)
            {
                var file = TestHelpers.CreateTestFile(fileType, "+ void TestMethod() {");

                GetRule().AssertIsIssueFree(file, file.ChangedLines);
            }
        }

        private Brackets GetRule()
        {
            return TestHelpers.GetRule<Brackets>();
        }
    }
}
