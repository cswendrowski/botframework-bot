using PullRequestReviewService.Models;
using System.Collections.Generic;

namespace PullRequestReviewService.Interfaces
{
    public interface IRule
    {
        List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound);
    }
}
