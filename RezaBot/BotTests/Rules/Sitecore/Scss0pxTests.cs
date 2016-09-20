using BotTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RezaBot.Rules.Sitecore;
using System.Collections.Generic;

namespace BotTests
{
    [TestClass]
    public class Scss0pxTests
    {
        [TestMethod]
        public void Exists()
        {
            var rule = GetRule();

            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void Finds0px()
        {
            var file = TestHelpers.CreateTestFile("scss", "+ margin: 0px;");

            GetRule().AssertFoundIssue(file, file.ChangedLines);
        }

        [TestMethod]
        public void Ignores10px()
        {
            var file = TestHelpers.CreateTestFile("scss", "+ margin: 10px;");

            GetRule().AssertIsIssueFree(file, file.ChangedLines);
        }

        [TestMethod]
        public void Ignores0()
        {
            var file = TestHelpers.CreateTestFile("scss", "+ margin: 0;");

            GetRule().AssertIsIssueFree(file, file.ChangedLines);
        }

        private Scss0px GetRule()
        {
            return TestHelpers.GetRule<Scss0px>();
        }
    }
}
