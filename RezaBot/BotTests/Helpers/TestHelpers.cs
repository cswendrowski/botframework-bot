using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using RezaBot.Models;
using RezaBot.Modules;
using RezaBot.Rules;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BotTests.Helpers
{
    public static class TestHelpers
    {
        public static T GetRule<T>()
        {
            return new StandardKernel(new NinjectBotModule()).Get<T>();
        }

        public static ChangedFile CreateTestFile(string fileType, string codeLine)
        {
            return new ChangedFile()
            {
                FileName = "testFile." + fileType,
                ChangedLines = new List<CodeLine>
                {
                    new CodeLine(codeLine)
                }
            };
        }

        public static ChangedFile CreateTestFile(string fileType, List<string> codeLines)
        {
            return CreateTestFile("testFile", fileType, codeLines);
        }

        public static ChangedFile CreateTestFile(string fileName, string fileType, List<string> codeLines)
        {
            var file = new ChangedFile()
            {
                FileName = fileName + "." + fileType,
                ChangedLines = codeLines.Select(x => new CodeLine(x)).ToList()
            };

            for (int x = 0; x < file.ChangedLines.Count; x++)
            {
                file.ChangedLines[x].LineNumber = x;
            }

            return file;
        }

        public static void AssertFoundIssue(this Rule rule, ChangedFile file, List<CodeLine> addedLines = null, List<CodeLine> removedLines = null)
        {
            var foundIssue = false;

            var messages = rule.Evaluate(file, addedLines, out foundIssue);

            Assert.IsTrue(foundIssue);
            Assert.IsNotNull(messages);
            Assert.IsTrue(messages.Any());
            Trace.WriteLine(messages.First());
        }

        public static void AssertIsIssueFree(this Rule rule, ChangedFile file, List<CodeLine> addedLines = null, List<CodeLine> removedLines = null)
        {
            var foundIssue = false;

            var messages = rule.Evaluate(file, addedLines, out foundIssue);

            Assert.IsFalse(foundIssue);
            Assert.IsNotNull(messages);
            Assert.IsFalse(messages.Any());
        }
    }
}
