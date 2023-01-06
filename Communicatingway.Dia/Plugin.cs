using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Communicatingway.Dia.Models;
using Communicatingway.Dia.Services;
using Communicatingway.Dia.Utilities;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace Communicatingway.Dia
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => AppConstants.APP_NAME;
        public AppConfiguration AppConfiguration = new AppConfiguration();

        [PluginService]
        private static ChatGui _chatGui { get; set; } = null!;

        private DiscordServices? _discordService;

        public Plugin(CommandManager command)
        {
            try
            {
                _LoadConfig();

                PluginLog.Debug("Load event ... please dont crash");
                _chatGui.ChatMessage += ChatMessage;

                _discordService = new DiscordServices(AppConfiguration);

                Task.Run(async () =>
                {
                    PluginLog.Debug("Starting Discord bot ... please dont crash");
                    await _discordService.Start();
                });
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error occurred: {ex.Message}");
                PluginLog.Error($"{ex.StackTrace}");
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_discordService != null)
            {
                _discordService.Stop().GetAwaiter().GetResult();
            }

            _chatGui.ChatMessage -= ChatMessage;
        }

        private void _LoadConfig()
        {
            PluginLog.LogDebug($"Loading config in progress");

            var currentPath = Assembly.GetExecutingAssembly().Location;

            if (string.IsNullOrEmpty(currentPath))
            {
                throw new Exception("Current assembly path is null");
            }

            string configFilePath = Path.Combine(Path.GetDirectoryName(currentPath),
                AppConstants.CONFIG_FILE);

            if (string.IsNullOrEmpty(configFilePath))
            {
                throw new Exception($"Config file {AppConstants.CONFIG_FILE} is empty");
            }

            PluginLog.LogDebug($"Find config file with path: {configFilePath}");

            using (StreamReader streamReader = new StreamReader(configFilePath))
            {
                string configFile = streamReader.ReadToEnd();
                this.AppConfiguration = JsonConvert.DeserializeObject<AppConfiguration>(configFile);
            }

            PluginLog.LogDebug($"Load config is success");
        }

        private void ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (_discordService == null)
            {
                return;
            }

            if (type != XivChatType.FreeCompany)
            {
                return;
            }

            _discordService.SendMessage(sender.TextValue, message.TextValue).GetAwaiter().GetResult();
        }
    }
}