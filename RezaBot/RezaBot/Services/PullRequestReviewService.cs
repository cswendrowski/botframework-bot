using Microsoft.Bot.Builder.Dialogs;
using RezaBot.Models;
using RezaBot.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RezaBot.Services
{
    public class PullRequestReviewService : IPullRequestReviewService
    {
        private IGitService _gitService;
        private IRule[] _rules;

        public PullRequestReviewService(IGitService gitService, IRule[] rules)
        {
            _gitService = gitService;
            _rules = rules;
        }

        public void ReviewPullRequest(int prNumber, IDialogContext context = null)
        {
            var files = _gitService.DownloadPrFiles(prNumber);

            NitpickFiles(files, prNumber, context);
        }

        private void NitpickFiles(List<ChangedFile> files, int prNumber, IDialogContext context = null)
        {
            _gitService.ConversationContext = context;

            var issueWasFound = false;

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
                    rule.GitService = _gitService;

                    if (rule.Evaluate(prNumber, file, addedLines, removedLines, file.ChangedLines))
                    {
                        issueWasFound = true;
                    }
                }
            }

            if (!issueWasFound)
            {
                _gitService.AddGeneralComment("No issues found in this PR, good job!", prNumber);
            }
        }
    }
}
