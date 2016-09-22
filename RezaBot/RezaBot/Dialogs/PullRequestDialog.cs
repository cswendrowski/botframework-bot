using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Ninject;
using PullRequestReviewService.Interfaces;
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
            string message = $"Sorry, I'm not sure what you want me to do. My currently available Intents are: "
                + string.Join(", ", result.Intents.Select(i => i.Intent));
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
                    retry: "I'm not sure what option you selected, sorry!",
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

            var reviewService = GetClass<IPullRequestReviewService>();

            switch (confirm)
            {
                case PrReviewOptions.Yes:
                    await context.PostAsync("I am starting my review of PR " + number + ".");
                    var count = reviewService.ReviewPullRequest(number).Count;
                    await context.PostAsync("I finished my review and left " + count +
                        " comments, let me know if there's anything else I can do to help!");
                    break;

                case PrReviewOptions.Preview:
                    await context.PostAsync("Here is a preview of PR " + number + ":");
                    var messages = reviewService.ReviewPullRequest(number, false);

                    foreach (var message in messages)
                    {
                        await context.PostAsync(message.ToString());
                    }

                    break;

                case PrReviewOptions.No:
                    await context.PostAsync("Sounds good, I will ignore PR " + number + " for now.");
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

        [LuisIntent("View Hivecast")]
        public async Task ViewHivecast(IDialogContext context, LuisResult result)
        {
            string name;
            context.ConversationData.TryGetValue("rmName", out name);

            if (string.IsNullOrEmpty(name))
            {
                PromptDialog.Text(context, AfterGetName, "What is your name, as displayed in RM-ify?");
            }
            else
            {
                var forecast = RmService.GetForecastForSherpa(name);

                foreach (var message in forecast.Where(x => !string.IsNullOrEmpty(x.Forecast.ElementAt(2))))
                {
                    await context.PostAsync(message.ToString());
                }

                context.Wait(MessageReceived);
            }
        }

        public async Task AfterGetName(IDialogContext context, IAwaitable<string> argument)
        {
            var name = await argument;

            context.ConversationData.SetValue("rmName", name);

            var forecast = RmService.GetForecastForSherpa(name);

            foreach (var message in forecast.Where(x => !string.IsNullOrEmpty(x.Forecast.ElementAt(2))))
            {
                await context.PostAsync(message.GetForecastMessageForWeek(2));
            }

            context.Wait(MessageReceived);
        }

        private T GetClass<T>()
        {
            var kernel = new StandardKernel(new NinjectBotModule());
            return kernel.Get<T>();
        }
    }
}
