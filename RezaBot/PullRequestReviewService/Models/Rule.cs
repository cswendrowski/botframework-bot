using PullRequestReviewService.Interfaces;
using System.Collections.Generic;

namespace PullRequestReviewService.Models
{
    public abstract class Rule : IRule
    {
        public abstract List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound);
    }
}
