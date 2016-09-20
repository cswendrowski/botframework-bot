using BotTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using RezaBot.Models;
using RezaBot.Modules;
using RezaBot.Rules.Sitecore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BotTests
{
    [TestClass]
    public class ExtraNewLineTests
    {
        [TestMethod]
        public void Exists()
        {
            var rule = GetRule();

            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void ExtraNewLines()
        {
            var code = new List<string>
            {
                "+",
                "+"
            };
            var file = TestHelpers.CreateTestFile("cs", code);

            GetRule().AssertFoundIssue(file, null, null);
        }

        [TestMethod]
        public void StopsBetweenPatches()
        {
            var code = new List<string>
            {
                "+",
                "-",
                "+"
            };
            var file = TestHelpers.CreateTestFile("cs", code);

            GetRule().AssertIsIssueFree(file);
        }

        private ExtraNewLine GetRule()
        {
            return TestHelpers.GetRule<ExtraNewLine>();
        }
    }
}
