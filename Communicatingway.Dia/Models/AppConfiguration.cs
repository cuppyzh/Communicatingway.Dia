using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
