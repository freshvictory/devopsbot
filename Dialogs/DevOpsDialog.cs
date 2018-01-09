using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Text;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class DevOpsDialog : LuisDialog<object>
    {
        public DevOpsDialog()
            :base(CreateLuisService())
        {
        }

        [LuisIntent("ListBuilds")]
        public async Task ListBuildsIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Let me list those builds for you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("DeployBuild")]
        public async Task DeployBuildIntent(IDialogContext context, LuisResult result)
        {
            var sb = new StringBuilder();

            foreach (var entity in result.Entities) {
                sb.AppendLine($"entity: {entity.Entity}, role: {entity.Role}, resolution: {entity.Resolution}, type: {entity.Type}");
            }

            await context.PostAsync(sb.ToString());
            context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You said: {result.Query}. I don't know how to do that.");
            context.Wait(MessageReceived);
        }

        private static ILuisService CreateLuisService()
        {
            var appId = ConfigurationManager.AppSettings["LuisAppId"];
            var apiKey = ConfigurationManager.AppSettings["LuisApiKey"];

            return new LuisService(new LuisModelAttribute(appId, apiKey));
        }
    }
}
