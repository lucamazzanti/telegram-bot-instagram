using Newtonsoft.Json;
// ReSharper disable StringLiteralTypo

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial.DTO
{
    public class GraphqlContract
    {
        [JsonProperty("graphql")]
        public GraphqlType Graphql { get; set; }
    }

    public class GraphqlType
    {
        [JsonProperty("shortcode_media")]
        public ShortCodeMediaType ShortCodeMedia { get; set; }
    }

    public class ChildrenCollectionType
    {
        [JsonProperty("edges")]
        public ChildType[] Items { get; set; }
    }

    public class ChildType
    {
        [JsonProperty("node")]
        public ChildMediaType Item { get; set; }
    }

    public class ChildMediaType
    {
        [JsonProperty("__typename")]
        public string TypeName { get; set; }

        [JsonProperty("shortcode")]
        public string ShortCode { get; set; }

        [JsonProperty("is_video")]
        public bool IsVideo { get; set; }

        [JsonProperty("display_url")]
        public string ImageUrl { get; set; }
    }

    public class ShortCodeMediaType
    {
        [JsonProperty("__typename")]
        public string TypeName { get; set; }

        [JsonProperty("shortcode")]
        public string ShortCode { get; set; }

        [JsonProperty("is_video")]
        public bool IsVideo { get; set; }

        [JsonProperty("display_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("video_url")]
        public string VideoUrl { get; set; }

        [JsonProperty("edge_sidecar_to_children")]
        public ChildrenCollectionType ChildrenCollection { get; set; }
    }
}