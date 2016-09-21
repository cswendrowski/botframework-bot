using PullRequestReviewService.Interfaces;
using PullRequestReviewService.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace BotTests.Fakes
{
    public class FakeGitService : IGitService
    {
        private ChangedFile File { get; set; }

        public FakeGitService(ChangedFile file)
        {
            File = file;
        }

        public void AddGeneralComment(string message, int prNumber)
        {
            Trace.WriteLine("General Comment: " + message);
        }

        public List<ChangedFile> DownloadPrFiles(int prNumber)
        {
            return new List<ChangedFile>
            {
                File
            };
        }

        public string GetLastestCommitInPr(int prNumber)
        {
            return "ABC123HASH";
        }

        public string GetPrAuthor(int prNumber)
        {
            return "Test Author";
        }

        public void WriteComment(ChangedFile file, CodeLine line, string message, int prNumber)
        {
            Trace.WriteLine(string.Format("Line {0} Comment: {1}", line.LineNumber, message));
        }
    }
}
