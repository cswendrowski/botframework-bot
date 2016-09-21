using PullRequestReviewService.Interfaces;
using PullRequestReviewService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PullRequestReviewService.Services
{
    public class PrReviewService : IPullRequestReviewService
    {
        private IGitService _gitService;
        private IRule[] _rules;

        public PrReviewService(IGitService gitService, IRule[] rules)
        {
            _gitService = gitService;
            _rules = rules;
        }

        public List<CodeComment> ReviewPullRequest(int prNumber, bool outputMessagesToGitService = true)
        {
            var files = _gitService.DownloadPrFiles(prNumber);

            return NitpickFiles(files, prNumber, outputMessagesToGitService);
        }

        private List<CodeComment> NitpickFiles(List<ChangedFile> files, int prNumber, bool outputMessagesToGitService = true)
        {
            var issueWasFound = false;
            var messages = new List<CodeComment>();

            foreach (var file in files)
            {
                if (file.ChangedLines == null || !file.ChangedLines.Any())
                {
                    Console.WriteLine("File has no changed lines");
                    continue;
                }

                var addedLines = file.ChangedLines.Where(x => x.WasAdded).ToList();
                var removedLines = file.ChangedLines.Where(x => x.WasDeleted).ToList();

                foreach (var rule in _rules)
                {
                    var ruleFoundIssue = false;

                    messages.AddRange(rule.Evaluate(file, addedLines, out ruleFoundIssue));

                    if (ruleFoundIssue)
                    {
                        issueWasFound = true;
                    }
                }
            }

            foreach (var message in messages)
            {
                if (outputMessagesToGitService)
                {
                    _gitService.WriteComment(message.File, message.Line, message.Comment, prNumber);
                }
            }

            if (!issueWasFound)
            {
                messages.Add(new CodeComment
                {
                    Comment = "No issues found in this PR, good job!"
                });

                if (outputMessagesToGitService)
                {
                    _gitService.AddGeneralComment("No issues found in this PR, good job!", prNumber);
                }
            }

            return messages;
        }
    }
}
