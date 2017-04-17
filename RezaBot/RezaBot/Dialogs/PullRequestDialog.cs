using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Ninject;
using PullRequestReviewService.Interfaces;
using RezaBot.Modules;
using RezaBot.Services;
using System;
using System.Diagnostics;
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

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            string message = "Hello! I could really go for some fish right now. Or whiskey..";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Taco Tuesday")]
        public async Task Tacos(IDialogContext context, LuisResult result)
        {
            var today = GetCstNow();

            string message;

            if (today.DayOfWeek == DayOfWeek.Tuesday)
            {
                message = "It's Tuesday, which means TACOS! I love fish tacos <3";
            }
            else if (today.DayOfWeek == DayOfWeek.Friday)
            {
                message = "No Tacos today, but it's Whiskey Friday at least! Drinks at 4!";
            }
            else
            {
                message = "It's not Taco Tuesday :(";
            }

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        public static DateTime GetCstNow()
        {
            var timeUtc = DateTime.UtcNow;
            try
            {
                var cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                var cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
                return cstTime;
            }
            catch (TimeZoneNotFoundException)
            {
                Debug.WriteLine("The registry does not define the Central Standard Time zone.");
            }
            catch (InvalidTimeZoneException)
            {
                Debug.WriteLine("Registry data on the Central Standard Time zone has been corrupted.");
            }
            return DateTime.Now;
        }


        [LuisIntent("Analyze PR")]
        public async Task AnalyzePR(IDialogContext context, LuisResult result)
        {
            var message = $"I'm sorry, PR reviewing has been disabled for now :(";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
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
                var today = GetCstNow();
                var nextWeek = today.AddDays(7);
                var weekEnding = nextWeek.AddDays((-1) * (int)nextWeek.DayOfWeek).AddDays(5);

                foreach (var msg in forecast.Select(message => message.GetForecastMessageForWeek(2)).Where(msg => !string.IsNullOrEmpty(msg)))
                {
                    var updatedMsg = msg.Replace(name, "you").Replace("has", "have");
                    if (msg.Contains("total"))
                    {
                        await context.PostAsync(
                            "For week ending " + weekEnding.ToString("MM/dd") + ", " + updatedMsg);
                    }
                    else
                    {
                        await context.PostAsync(updatedMsg);
                    }
                }

                context.Wait(MessageReceived);
            }
        }

        public async Task AfterGetName(IDialogContext context, IAwaitable<string> argument)
        {
            var name = await argument;

            context.ConversationData.SetValue("rmName", name);

            var forecast = RmService.GetForecastForSherpa(name);
            var today = GetCstNow();
            var nextWeek = today.AddDays(7);
            var weekEnding = nextWeek.AddDays((-1)*(int) nextWeek.DayOfWeek).AddDays(5);

            foreach (var msg in forecast.Select(message => message.GetForecastMessageForWeek(2)).Where(msg => !string.IsNullOrEmpty(msg)))
            {
                var updatedMsg = msg.Replace(name, "you").Replace("has", "have");
                if (msg.Contains("total"))
                {
                    await context.PostAsync(
                        "For week ending " + weekEnding.ToString("MM/dd") + ", " + updatedMsg);
                }
                else
                {
                    await context.PostAsync(updatedMsg);
                }
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("PTO")]
        public async Task ViewPtoHours(IDialogContext context, LuisResult result)
        {
            string name;
            context.ConversationData.TryGetValue("netsuiteName", out name);

            if (string.IsNullOrEmpty(name))
            {
                PromptDialog.Text(context, AfterGetNetsuiteName, "What is your name, as displayed in Netsuite?");
            }
            else
            {
                var hours = RmService.GetPtoHoursForYear(name);
                if (hours < 0)
                {
                    await context.PostAsync("I could not find you in Netsuite, sorry!");
                }
                else
                {
                    await context.PostAsync("You have used " + hours + " hours (" + hours / 8 + " days) this year");
                }

                context.Wait(MessageReceived);
            }
        }

        public async Task AfterGetNetsuiteName(IDialogContext context, IAwaitable<string> argument)
        {
            var name = await argument;

            context.ConversationData.SetValue("netsuiteName", name);

            var hours = RmService.GetPtoHoursForYear(name);
            if (hours < 0)
            {
                await context.PostAsync("I could not find you in Netsuite, sorry!");
            }
            else
            {
                await context.PostAsync("You have used " + hours + " hours (" + hours / 8 + " days) this year");
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
