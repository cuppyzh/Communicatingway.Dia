using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Communicatingway.Dia.Utilities;
using Dalamud.Logging;
using Newtonsoft.Json;

namespace Communicatingway.Dia.Models
{
    public class AppConfiguration
    {
        [JsonProperty("DiscordBotToken")]
        public string? DiscordBotToken { get; set; }

        [JsonProperty("DiscordServerId")]
        public string? DiscordServerId { get; set; }

        [JsonProperty("DiscordChannelId")]
        public string? DiscordChannelId { get; set; }

        public AppConfiguration()
        {

        }

        public void LoadConfig()
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

                if (string.IsNullOrEmpty(configFile))
                {
                    throw new Exception("Config file is empty");
                }

                _PopulateConfig(configFile);
            }

            PluginLog.LogDebug($"Load config is success");
        }

        private void _PopulateConfig(string configFile)
        {
            var appConfiguration = JsonConvert.DeserializeObject<AppConfiguration>(configFile);

            if (appConfiguration == null)
            {
                throw new Exception("Config file is empty");
            }

            DiscordBotToken = appConfiguration.DiscordBotToken;
            DiscordChannelId = appConfiguration.DiscordChannelId;
            DiscordServerId = appConfiguration.DiscordServerId;
        }
    }
}
