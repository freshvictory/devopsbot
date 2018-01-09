using System;
using System.Configuration;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Text;
using System.Linq;
using SimpleEchoBot.Models;

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
            try
            {
                var entityLookup = result.Entities?.ToLookup(e => e.Type);

                if (entityLookup == null) {
                    await context.PostAsync("No entities found for this command.");
                    context.Wait(MessageReceived);
                    return;
                }

                var environment = entityLookup[DeployEntity.Environment]?.FirstOrDefault();
                var buildNumber = entityLookup[DeployEntity.BuildNumber]?.FirstOrDefault();

                await context.PostAsync($"Deploying build {buildNumber.Entity} to environment {environment.Entity}");
            }
            catch (Exception e)
            {
                await context.PostAsync($"Exception:\n{e.ToString()}");
            }

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
