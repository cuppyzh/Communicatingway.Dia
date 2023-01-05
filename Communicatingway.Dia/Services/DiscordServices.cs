using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using Discord.WebSocket;

namespace Communicatingway.Dia.Services
{
    public class DiscordServices
    {
        private DiscordSocketClient _discordSocketClient;

        public DiscordServices()
        {
            _discordSocketClient = new DiscordSocketClient();
        }

        public async Task Start()
        {
            PluginLog.Debug("Starting discord bot");

            try
            {
                await _discordSocketClient.LoginAsync(Discord.TokenType.Bot, "");
                await _discordSocketClient.StartAsync();
            } catch(Exception ex)
            {
                PluginLog.LogError($"Discord bot failed to start: {ex.Message} {ex.InnerException?.Message}");
                PluginLog.LogError($"{ex.StackTrace}");
            }
        }
    }
}
