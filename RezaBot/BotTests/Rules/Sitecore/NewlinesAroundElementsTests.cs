using BotTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RezaBot.Rules;
using System.Collections.Generic;

namespace BotTests
{
    [TestClass]
    public class NewlinesAroundElementsTests
    {
        [TestMethod]
        public void Exists()
        {
            var rule = GetRule();

            Assert.IsNotNull(rule);
        }

        [TestMethod]
        public void NewlineAfterOpeningElement()
        {
            var openingElements = new List<string>
            {
                "<div>",
                "{",
                "<b>"
            };

            foreach (var element in openingElements)
            {
                var code = new List<string>
                {
                    "+" + element,
                    "+",
                    "+ // Some code"
                };

                var file = TestHelpers.CreateTestFile("cs", code);

                GetRule().AssertFoundIssue(file);
            }
        }

        [TestMethod]
        public void NewlineBeforeOpeningElement()
        {
            var openingElements = new List<string>
            {
                "</div>",
                "}",
                "</b>"
            };

            foreach (var element in openingElements)
            {
                var code = new List<string>
                {
                    "+ // Some code",
                    "+",
                    "+" + element,
                };

                var file = TestHelpers.CreateTestFile("cs", code);

                GetRule().AssertFoundIssue(file);
            }
        }

        public void NoExtraNewLines()
        {
            var code = new List<string>
            {
                "+ void TestMethod()",
                "+ {",
                "+ // Some code",
                "+ }"
            };

            var file = TestHelpers.CreateTestFile("cs", code);

            GetRule().AssertIsIssueFree(file);

            code = new List<string>
            {
                "+ <div>",
                "+ // Some code",
                "+ </div>"
            };

            file = TestHelpers.CreateTestFile("cs", code);

            GetRule().AssertIsIssueFree(file);
        }

        private NewlinesAroundElements GetRule()
        {
            return TestHelpers.GetRule<NewlinesAroundElements>();
        }
    }
}
