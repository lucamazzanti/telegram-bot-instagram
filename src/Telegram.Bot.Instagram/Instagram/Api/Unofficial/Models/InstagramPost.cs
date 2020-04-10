using System;

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models
{
    public class InstagramPost
    {
        public string Shortcode { get; set; }

        public Uri Url { get; set; }

        public string HtmlContent { get; set; }
    }
}