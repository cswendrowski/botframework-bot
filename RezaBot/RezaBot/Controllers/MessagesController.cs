using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RezaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                //// calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                //// return our reply to the user
                //Activity reply = activity.CreateReply($"You sent \"{activity.Text}\" which was {length} characters long");
                //await connector.Conversations.ReplyToActivityAsync(reply);

                await Conversation.SendAsync(activity, () => new PrDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}

[LuisModel("51cbb4d1-e66f-40dd-b6aa-ac6b07ad84f5", "da73fb812e2542858a4688863dd0ffcb")]
[Serializable]
public class PrDialog : LuisDialog<object>
{
    [LuisIntent("")]
    public async Task None(IDialogContext context, LuisResult result)
    {
        string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
        await context.PostAsync(message);
        context.Wait(MessageReceived);
    }

    [LuisIntent("Analyze PR")]
    public async Task AnalyzePR(IDialogContext context, LuisResult result)
    {
        int number;
        var message = string.Empty;

        if (TryFindPrNumber(result, out number))
        {
            message = $"You want me to analyze PR " + number + "? I see. That is coming soon!";
        }
        else
        {
            message = $"I'm sorry, please include a PR number that you want me to analyze. Thanks!";
        }
        await context.PostAsync(message);
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

[LuisModel("c413b2ef-382c-45bd-8ff0-f76d60e2a821", "6d0966209c6e4f6b835ce34492f3e6d9")]
[Serializable]
public class SimpleAlarmDialog : LuisDialog<object>
{
    private readonly Dictionary<string, Alarm> alarmByWhat = new Dictionary<string, Alarm>();
    public const string DefaultAlarmWhat = "default";

    public bool TryFindAlarm(LuisResult result, out Alarm alarm)
    {
        alarm = null;
        string what;
        EntityRecommendation title;
        if (result.TryFindEntity(Entity_Alarm_Title, out title))
        {
            what = title.Entity;
        }
        else
        {
            what = DefaultAlarmWhat;
        }
        return this.alarmByWhat.TryGetValue(what, out alarm);
    }

    public const string Entity_Alarm_Title = "builtin.alarm.title";
    public const string Entity_Alarm_Start_Time = "builtin.alarm.start_time";
    public const string Entity_Alarm_Start_Date = "builtin.alarm.start_date";

    [LuisIntent("")]
    public async Task None(IDialogContext context, LuisResult result)
    {
        string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
        await context.PostAsync(message);
        context.Wait(MessageReceived);
    }

    [LuisIntent("builtin.intent.alarm.delete_alarm")]
    public async Task DeleteAlarm(IDialogContext context, LuisResult result)
    {
        Alarm alarm;
        if (TryFindAlarm(result, out alarm))
        {
            this.alarmByWhat.Remove(alarm.What);
            await context.PostAsync($"alarm {alarm} deleted");
        }
        else
        {
            await context.PostAsync("did not find alarm");
        }
        context.Wait(MessageReceived);
    }

    [LuisIntent("builtin.intent.alarm.find_alarm")]
    public async Task FindAlarm(IDialogContext context, LuisResult result)
    {
        Alarm alarm;
        if (TryFindAlarm(result, out alarm))
        {
            await context.PostAsync($"found alarm {alarm}");
        }
        else
        {
            await context.PostAsync("did not find alarm");
        }
        context.Wait(MessageReceived);
    }

    [LuisIntent("builtin.intent.alarm.set_alarm")]
    public async Task SetAlarm(IDialogContext context, LuisResult result)
    {
        EntityRecommendation title;
        if (!result.TryFindEntity(Entity_Alarm_Title, out title))
        {
            title = new EntityRecommendation(type: Entity_Alarm_Title) { Entity = DefaultAlarmWhat };
        }
        EntityRecommendation date;
        if (!result.TryFindEntity(Entity_Alarm_Start_Date, out date))
        {
            date = new EntityRecommendation(type: Entity_Alarm_Start_Date) { Entity = string.Empty };
        }
        EntityRecommendation time;
        if (!result.TryFindEntity(Entity_Alarm_Start_Time, out time))
        {
            time = new EntityRecommendation(type: Entity_Alarm_Start_Time) { Entity = string.Empty };
        }
        var parser = new Chronic.Parser();
        var span = parser.Parse(date.Entity + " " + time.Entity);
        if (span != null)
        {
            var when = span.Start ?? span.End;
            var alarm = new Alarm() { What = title.Entity, When = when.Value };
            this.alarmByWhat[alarm.What] = alarm;
            string reply = $"alarm {alarm} created";
            await context.PostAsync(reply);
        }
        else
        {
            await context.PostAsync("could not find time for alarm");
        }
        context.Wait(MessageReceived);
    }

    [LuisIntent("builtin.intent.alarm.snooze")]
    public async Task AlarmSnooze(IDialogContext context, LuisResult result)
    {
        Alarm alarm;
        if (TryFindAlarm(result, out alarm))
        {
            alarm.When = alarm.When.Add(TimeSpan.FromMinutes(7));
            await context.PostAsync($"alarm {alarm} snoozed!");
        }
        else
        {
            await context.PostAsync("did not find alarm");
        }
        context.Wait(MessageReceived);
    }

    [LuisIntent("builtin.intent.alarm.time_remaining")]
    public async Task TimeRemaining(IDialogContext context, LuisResult result)
    {
        Alarm alarm;
        if (TryFindAlarm(result, out alarm))
        {
            var now = DateTime.UtcNow;
            if (alarm.When > now)
            {
                var remaining = alarm.When.Subtract(DateTime.UtcNow);
                await context.PostAsync($"There is {remaining} remaining for alarm {alarm}.");
            }
            else
            {
                await context.PostAsync($"The alarm {alarm} expired already.");
            }
        }
        else
        {
            await context.PostAsync("did not find alarm");
        }
        context.Wait(MessageReceived);
    }

    private Alarm turnOff;

    [LuisIntent("builtin.intent.alarm.turn_off_alarm")]
    public async Task TurnOffAlarm(IDialogContext context, LuisResult result)
    {
        if (TryFindAlarm(result, out this.turnOff))
        {
            PromptDialog.Confirm(context, AfterConfirming_TurnOffAlarm, "Are you sure?", promptStyle: PromptStyle.None);
        }
        else
        {
            await context.PostAsync("did not find alarm");
            context.Wait(MessageReceived);
        }
    }

    public async Task AfterConfirming_TurnOffAlarm(IDialogContext context, IAwaitable<bool> confirmation)
    {
        if (await confirmation)
        {
            this.alarmByWhat.Remove(this.turnOff.What);
            await context.PostAsync($"Ok, alarm {this.turnOff} disabled.");
        }
        else
        {
            await context.PostAsync("Ok! We haven't modified your alarms!");
        }
        context.Wait(MessageReceived);
    }

    [LuisIntent("builtin.intent.alarm.alarm_other")]
    public async Task AlarmOther(IDialogContext context, LuisResult result)
    {
        await context.PostAsync("what ?");
        context.Wait(MessageReceived);
    }

    public SimpleAlarmDialog()
    {
    }

    public SimpleAlarmDialog(ILuisService service)
        : base(service)
    {
    }

    [Serializable]
    public sealed class Alarm : IEquatable<Alarm>
    {
        public DateTime When { get; set; }
        public string What { get; set; }

        public override string ToString()
        {
            return $"[{this.What} at {this.When}]";
        }

        public bool Equals(Alarm other)
        {
            return other != null
                && this.When == other.When
                && this.What == other.What;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Alarm);
        }

        public override int GetHashCode()
        {
            return this.What.GetHashCode();
        }
    }
}
