using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.DTO;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;

namespace Telegram.Bot.Instagram.Instagram.Api.Unofficial
{
    public static class InstagramPostScraper
    {
        public static IEnumerable<GraphqlContract> ExtractGraphql(InstagramPost post)
        {
            var results = new List<GraphqlContract>();

            MatchCollection extractScriptContents = Regex.Matches(post.HtmlContent, "<script type=\\\"text\\/javascript\\\">(.*?)<\\/script>", RegexOptions.IgnoreCase);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Match extractScript in extractScriptContents.Where(i => i.Success && i.Groups.Count == 2))
            {
                var scriptContent = extractScript.Groups[1].Value;

                Match extractGraphql = Regex.Match(scriptContent, "window\\.__additionalDataLoaded\\(\\'\\/p\\/" + post.Shortcode + "\\/\\'\\,(.*?)\\)\\;", RegexOptions.IgnoreCase);

                if (extractGraphql.Success && extractGraphql.Groups.Count == 2)
                {
                    var graphqlContent = extractGraphql.Groups[1].Value;

                    var graphql = JsonConvert.DeserializeObject<GraphqlContract>(graphqlContent);

                    results.Add(graphql);
                }

                extractGraphql = Regex.Match(scriptContent, "window\\._sharedData \\=(.*)\\;", RegexOptions.IgnoreCase);

                if (extractGraphql.Success && extractGraphql.Groups.Count == 2)
                {
                    var sharedDataContent = extractGraphql.Groups[1].Value;

                    var sharedData = JsonConvert.DeserializeObject<SharedDataContract>(sharedDataContent);

                    if (sharedData?.EntryData?.PostPage?.Length > 0)
                    {
                        results.AddRange(sharedData.EntryData.PostPage);
                    }
                }
            }

            return results;
        }

        public static IEnumerable<InstagramMedia> ExtractMediaFromGraphql(string graphqlContent)
        {
            var graphql = JsonConvert.DeserializeObject<GraphqlContract>(graphqlContent);

            return ExtractMediaFromGraphql(graphql);
        }

        public static IEnumerable<InstagramMedia> ExtractMediaFromGraphql(GraphqlContract graphql)
        {
            var results = new List<InstagramMedia>();

            if (graphql?.Graphql?.ShortCodeMedia == null) return results;

            if (graphql.Graphql?.ShortCodeMedia?.TypeName != "GraphSidecar")
            {
                var mediaUrl = graphql.Graphql.ShortCodeMedia.IsVideo
                    ? graphql.Graphql.ShortCodeMedia.VideoUrl
                    : graphql.Graphql.ShortCodeMedia.ImageUrl;

                var mainMedia = new InstagramMedia
                {
                    Shortcode = graphql.Graphql.ShortCodeMedia.ShortCode,
                    Url = new Uri(mediaUrl), Name = ExtractMediaNameFromMediaUrl(mediaUrl)

                };

                results.Add(mainMedia);
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (ChildType child in graphql.Graphql.ShortCodeMedia.ChildrenCollection?.Items ?? new ChildType[0])
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