using BotTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RezaBot.Rules.Sitecore;

namespace BotTests.Rules.Sitecore
{
    [TestClass]
    public class Scss0000Tests
    {
        [TestMethod]
        public void Exists()
        {
            var rule = GetRule();

            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void FindsExtra0s()
        {
            var file = TestHelpers.CreateTestFile("scss", "+ margin: 0 0 0 0;");

            GetRule().AssertFoundIssue(file, file.ChangedLines);

            file = TestHelpers.CreateTestFile("scss", "+ padding: 0 0 0;");

            GetRule().AssertFoundIssue(file, file.ChangedLines);

            file = TestHelpers.CreateTestFile("scss", "+ margin: 0 0;");

            GetRule().AssertFoundIssue(file, file.ChangedLines);
        }

        [TestMethod]
        public void OnlyOne0()
        {
            var file = TestHelpers.CreateTestFile("scss", "+ margin: 0;");

            GetRule().AssertIsIssueFree(file, file.ChangedLines);
        }

        private Scss0000 GetRule()
        {
            return TestHelpers.GetRule<Scss0000>();
        }
    }
}
