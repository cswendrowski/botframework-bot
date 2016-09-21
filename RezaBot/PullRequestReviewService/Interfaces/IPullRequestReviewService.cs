using PullRequestReviewService.Models;
using System.Collections.Generic;

namespace PullRequestReviewService.Interfaces
{
    public interface IPullRequestReviewService
    {
        List<CodeComment> ReviewPullRequest(int prNumber, bool outputMessagesToGitService = true);
    }
}
