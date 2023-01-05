using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Communicatingway.Dia.Models;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using Discord;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;

namespace Communicatingway.Dia.Services
{
    public class DiscordServices
    {
        private DiscordSocketClient? _discordSocketClient;
        private AppConfiguration? _appConfiguration;
        private IMessageChannel? _messageChannel;

        public DiscordServices(AppConfiguration appConfiguration)
        {
            _appConfiguration = appConfiguration;
            _discordSocketClient = new DiscordSocketClient(
                new DiscordSocketConfig()
                {
                    LogLevel = LogSeverity.Debug
                }
            );
            _discordSocketClient.Ready += ReadyAsync;
        }

        public async Task Start()
        {
            PluginLog.Debug("Starting discord bot");

            try
            {
                await _discordSocketClient.LoginAsync(TokenType.Bot, _appConfiguration.DiscordBotToken);
                await _discordSocketClient.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                PluginLog.LogError($"Discord bot failed to start: {ex.Message} {ex.InnerException?.Message}");
                PluginLog.LogError($"{ex.StackTrace}");

                throw ex;
            }
        }

        public async Task Stop()
        {
            PluginLog.Debug("Shutting down discord bot");

            try
            {
                await _discordSocketClient.StopAsync();
            }
            catch (Exception ex)
            {
                PluginLog.LogError($"Discord bot failed to be stopped: {ex.Message} {ex.InnerException?.Message}");
                PluginLog.LogError($"{ex.StackTrace}");
            }
        }

        public void SendMessage(SeString sender, SeString message)
        {
            string draftMessage = $"***{sender.TextValue}***: {message.TextValue}";
            PluginLog.LogDebug($"Draft Message: {draftMessage}");

            _messageChannel.SendMessageAsync(draftMessage).GetAwaiter().GetResult();
        }

        private Task ReadyAsync()
        {
            PluginLog.Information($"{_discordSocketClient.CurrentUser} is connected!");
            _messageChannel = _discordSocketClient.GetChannel(Convert.ToUInt64(_appConfiguration.DiscordChannelId)) as IMessageChannel;

            if (_messageChannel == null)
            {
                PluginLog.Error("Message channel is null");
                throw new Exception($"Current Bot doesnt have access to channel: {_appConfiguration.DiscordChannelId}");
            }

            return Task.CompletedTask;
        }
    }
}
