using Microsoft.Bot.Builder.Dialogs;

namespace RezaBot.Services
{
    public interface IPullRequestReviewService
    {
        void ReviewPullRequest(int prNumber, IDialogContext context = null);
    }
}
