using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial.DTO
{
    public class SharedDataContract
    {
        [JsonProperty("config")]
        public ConfigType Config { get; set; }
    }

    public class ConfigType
    {
        [JsonProperty("csrf_token")]
        public string CSRFToken { get; set; }
    }
}