using PullRequestReviewService.Interfaces;
using System.Collections.Generic;

namespace PullRequestReviewService.Models
{
    public abstract class Rule : IRule
    {
        protected List<string> FileNamesToIgnore = new List<string>();

        protected List<string> FileTypesToCheck = new List<string>();

        public abstract List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound);
    }
}
