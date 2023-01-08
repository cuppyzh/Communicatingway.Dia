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
        private readonly DiscordSocketClient? _discordSocketClient;
        private readonly AppConfiguration? _appConfiguration;
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
            _discordSocketClient.Ready += _ReadyAsync;
        }

        public async Task Start()
        {
            PluginLog.Debug("Starting discord bot");

            try
            {
                if (_discordSocketClient == null)
                {
                    throw new Exception("Discord socket client is null");
                }

                if (_appConfiguration == null)
                {
                    throw new Exception("AppConfig is null");
                }

                await _discordSocketClient.LoginAsync(TokenType.Bot, _appConfiguration.DiscordBotToken);
                await _discordSocketClient.StartAsync();

                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                PluginLog.LogError($"Discord bot failed to start: {ex.Message} {ex.InnerException?.Message}");
                PluginLog.LogError($"{ex.StackTrace}");

                throw new Exception($"Discord bot failed to start: {ex.Message} {ex.InnerException?.Message}");
            }
        }

        public async Task Stop()
        {
            PluginLog.Debug("Shutting down discord bot");

            try
            {
                if (_discordSocketClient == null)
                {
                    return;
                }

                await _discordSocketClient.StopAsync();
            }
            catch (Exception ex)
            {
                PluginLog.LogError($"Discord bot failed to be stopped: {ex.Message} {ex.InnerException?.Message}");
                PluginLog.LogError($"{ex.StackTrace}");
            }
        }

        public async Task SendMessage(SeString sender, SeString message)
        {
            if (_messageChannel == null)
            {
                throw new Exception("Message Channel si null.");
            }

            string draftMessage = $"***{sender.TextValue}***: {_ParseMessage(message.TextValue)}";

            await Task.Run(() => _messageChannel.SendMessageAsync(draftMessage));
        }

        private Task _ReadyAsync()
        {
            if (_discordSocketClient == null)
            {
                throw new Exception("Discord socket client is null");
            }

            if (_appConfiguration == null)
            {
                throw new Exception("AppConfig is null");
            }

            PluginLog.Information($"{_discordSocketClient.CurrentUser} is connected!");
            _messageChannel = _discordSocketClient.GetChannel(Convert.ToUInt64(_appConfiguration.DiscordChannelId)) as IMessageChannel;

            if (_messageChannel == null)
            {
                PluginLog.Error("Message channel is null");
                throw new Exception($"Current Bot doesnt have access to channel: {_appConfiguration.DiscordChannelId}");
            }

            return Task.CompletedTask;
        }

        private string _ParseMessage(string message)
        {
            string cleanMessage = message;

            cleanMessage = cleanMessage.Replace("\ue040", "「");
            cleanMessage = cleanMessage.Replace("\ue041", "」");

            PluginLog.LogDebug($"Cleaned Message: {cleanMessage}");

            return cleanMessage;
        }
    }
}
