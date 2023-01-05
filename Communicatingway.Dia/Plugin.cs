using System;
using System.Threading.Tasks;
using Communicatingway.Dia.Services;
using Communicatingway.Dia.Utilities;
using Dalamud.Game.Command;
using Dalamud.Plugin;

namespace Communicatingway.Dia
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => AppConstants.APP_NAME;

        private DiscordServices _discordService;

        public Plugin(CommandManager command)
        {
            try
            {
                _discordService = new DiscordServices();

                Task.Run(async () =>
                {
                    await _discordService.Start();
                });
            } catch(Exception ex)
            {

            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}