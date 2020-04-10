using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.DTO;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial
{
    public static class InstagramPostScraper
    {
        public static IEnumerable<string> ExtractGraphql(InstagramPost post)
        {
            var results = new List<string>();

            MatchCollection extractScriptContents = Regex.Matches(post.HtmlContent, "<script type=\\\"text\\/javascript\\\">(.*?)<\\/script>", RegexOptions.IgnoreCase);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Match extractScript in extractScriptContents.Where(i => i.Success && i.Groups.Count == 2))
            {
                var scriptContent = extractScript.Groups[1].Value;

                Match extractGraphql = Regex.Match(scriptContent, "window\\.__additionalDataLoaded\\(\\'\\/p\\/" + post.Shortcode + "\\/\\'\\,(.*?)\\)\\;", RegexOptions.IgnoreCase);

                if (!extractGraphql.Success || extractGraphql.Groups.Count != 2) continue;

                var rawGraphql = extractGraphql.Groups[1].Value;

                results.Add(rawGraphql);
            }

            return results;
        }

        public static IEnumerable<InstagramMedia> ExtractMediaFromGraphql(string graphql)
        {
            var results = new List<InstagramMedia>();

            var root = JsonConvert.DeserializeObject<GraphqlContract>(graphql);

            if (root?.Graphql?.ShortCodeMedia == null) return results;

            if (root.Graphql?.ShortCodeMedia?.TypeName != "GraphSidecar")
            {
                var mediaUrl = root.Graphql.ShortCodeMedia.IsVideo
                    ? root.Graphql.ShortCodeMedia.VideoUrl
                    : root.Graphql.ShortCodeMedia.ImageUrl;

                var mainMedia = new InstagramMedia
                {
                    Shortcode = root.Graphql.ShortCodeMedia.ShortCode,
                    Url = new Uri(mediaUrl), Name = ExtractMediaNameFromMediaUrl(mediaUrl)

                };

                results.Add(mainMedia);
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (ChildType child in root.Graphql.ShortCodeMedia.ChildrenCollection?.Items ?? new ChildType[0])
            {
                if (child?.Item == null) continue;

                var media = new InstagramMedia
                {
                    Shortcode = child.Item.ShortCode,
                    Url = new Uri(child.Item.ImageUrl),
                    Name = ExtractMediaNameFromMediaUrl(child.Item.ImageUrl)
                };

                results.Add(media);
            }

            return results;
        }

        private static string ExtractMediaNameFromMediaUrl(string mediaUrl)
        {
            Match match = Regex.Match(mediaUrl, "\\/([^\\/]*?)\\?", RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count == 2)
            {
                return match.Groups[1].Value;
            }

            return mediaUrl;
        }
    }
}