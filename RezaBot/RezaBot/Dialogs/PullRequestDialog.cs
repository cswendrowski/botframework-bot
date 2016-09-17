using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Ninject;
using RezaBot.Modules;
using RezaBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RezaBot.Dialogs
{
    [LuisModel("51cbb4d1-e66f-40dd-b6aa-ac6b07ad84f5", "da73fb812e2542858a4688863dd0ffcb")]
    [Serializable]
    public class PullRequestDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        public enum PrReviewOptions
        {
            Yes,
            Preview,
            No
        }

        [LuisIntent("Analyze PR")]
        public async Task AnalyzePR(IDialogContext context, LuisResult result)
        {
            int number;
            var message = string.Empty;

            if (TryFindPrNumber(result, out number))
            {
                context.ConversationData.SetValue("prNumber", number);

                PromptDialog.Choice(
                    context: context,
                    resume: AfterPRConfirm,
                    options: Enum.GetValues(typeof(PrReviewOptions)).Cast<PrReviewOptions>().ToArray(),
                    prompt: "Are you sure you want me to review PR " + number + "?",
                    retry: "Didn't get that, sorry!",
                    promptStyle: PromptStyle.Auto);
            }
            else
            {
                message = $"I'm sorry, please include a PR number that you want me to analyze. Thanks!";
                await context.PostAsync(message);
                context.Wait(MessageReceived);
            }
        }

        public async Task AfterPRConfirm(IDialogContext context, IAwaitable<PrReviewOptions> argument)
        {
            var confirm = await argument;
            var number = context.ConversationData.Get<int>("prNumber");

            var kernel = new StandardKernel(new NinjectBotModule());
            var reviewService = kernel.Get<IPullRequestReviewService>();

            switch (confirm)
            {
                case PrReviewOptions.Yes:
                    await context.PostAsync("Starting review of PR " + number + ".");
                    reviewService.ReviewPullRequest(number);
                    break;

                case PrReviewOptions.Preview:
                    await context.PostAsync("Starting preview of PR " + number + ".");
                    reviewService.ReviewPullRequest(number, context);
                    break;

                case PrReviewOptions.No:
                    await context.PostAsync("Did not review PR " + number + ".");
                    break;
            }

            context.Wait(MessageReceived);
        }

        public bool TryFindPrNumber(LuisResult result, out int prNumber)
        {
            prNumber = -1;
            EntityRecommendation number;
            if (result.TryFindEntity("builtin.number", out number))
            {
                if (Int32.TryParse(number.Entity, out prNumber))
                {
                    return true;
                }
            }
            // Did not succeed
            return false;
        }
    }
}
