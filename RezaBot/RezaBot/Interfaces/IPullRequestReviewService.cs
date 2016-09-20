using Microsoft.Bot.Builder.Dialogs;
using RezaBot.Models;
using System.Collections.Generic;

namespace RezaBot.Services
{
    public interface IPullRequestReviewService
    {
        List<CodeComment> ReviewPullRequest(int prNumber, IDialogContext context = null);
    }
}
