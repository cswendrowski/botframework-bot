using BotTests.Fakes;
using BotTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using PullRequestReviewService.Interfaces;
using PullRequestReviewService.Models;
using PullRequestReviewService.Services;
using System.Collections.Generic;
using System.Linq;

namespace BotTests
{
    [TestClass]
    public class FullReviewTests
    {
        [TestMethod]
        public void CheckRulesAreBound()
        {
            var rules = GetAllBoundRules();
            Assert.IsNotNull(rules);
            Assert.IsTrue(rules.Any());
        }

        [TestMethod]
        public void ReviewWithIssues()
        {
            var code = new List<string>
            {
                "+",
                "+",
                "+ public  void TestMethod() {",
                "+",
                "+ // Code",
                "+",
                "+ }"
            };

            var file = TestHelpers.CreateTestFile("cs", code);

            var reviewService = GetReviewService(file);

            var messages = reviewService.ReviewPullRequest(0);

            Assert.IsNotNull(messages);
            Assert.IsTrue(messages.Any());
            Assert.IsTrue(messages.Count >= 3);
        }

        [TestMethod]
        public void ReviewNoIssues()
        {
            var code = new List<string>
            {
                "+ public void TestMethod()",
                "+ {",
                "+ // Code",
                "+ }",
                "-No newline at end of file"
            };

            var file = TestHelpers.CreateTestFile("cs", code);

            var reviewService = GetReviewService(file);

            var messages = reviewService.ReviewPullRequest(0);

            Assert.IsNotNull(messages);
            Assert.IsTrue(messages.Any());

            // No issues and Thanks
            Assert.IsTrue(messages.Count == 2);
        }

        [TestMethod]
        public void SkipsGlassItems()
        {
            var code = new List<string>
            {
                "+",
                "+",
                "+ public  void TestMethod() {",
                "+",
                "+ // Code",
                "+",
                "+ }"
            };

            var file = TestHelpers.CreateTestFile("GlassItems", "cs", code);

            var reviewService = GetReviewService(file);

            var messages = reviewService.ReviewPullRequest(0);

            Assert.IsNotNull(messages);
            Assert.IsTrue(messages.Any());

            // No issues
            Assert.IsTrue(messages.Count == 1);
        }

        private IPullRequestReviewService GetReviewService(ChangedFile file)
        {
            return new PrReviewService(new FakeGitService(file), GetAllBoundRules().ToArray());
        }

        private IEnumerable<IRule> GetAllBoundRules()
        {
            var kernel = new StandardKernel(new TestModule());

            return kernel.GetAll<IRule>();
        }
    }
}
