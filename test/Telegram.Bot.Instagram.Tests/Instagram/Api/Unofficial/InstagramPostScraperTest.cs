using System;
using System.Linq;
using Newtonsoft.Json;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial;
using Telegram.Bot.Instagram.Instagram.Api.Unofficial.Models;
using Xunit;
// ReSharper disable StringLiteralTypo

namespace Telegram.Bot.Instagram.Tests.Instagram.Api.Unofficial
{
    public class InstagramPostScraperTest
    {
        [Fact]
        public void ExtractGraphql_GivenLoggedInInstagramPost_ItExtractTheGraphql()
        {
            var instagramPost = new InstagramPost
            {
                Shortcode = "B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0",
                Url = new Uri("https://instagram.com/p/B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0/"),
                HtmlContent =
                    @"<!DOCTYPE html><html lang=""en"" class=""no-js logged-in client-root""><body>
                    <script type=""text/javascript"">window.__initialDataLoaded(window._sharedData);</script>
                    <script type=""text/javascript"">window.__additionalDataLoaded('/p/B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0/',{""graphql"":{""shortcode_media"":{""__typename"":""GraphImage"",""id"":""1280303637280241311"",""shortcode"":""B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0""}}});</script>
                    </body>"
            };

            var grapqhl = InstagramPostScraper.ExtractGraphql(instagramPost).ToArray();

            Assert.Single(grapqhl);
            Assert.Equal("B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0", grapqhl[0].Graphql.ShortCodeMedia.ShortCode);
        }

        [Fact]
        public void ExtractGraphql_GivenNotLoggedInInstagramPost_ItExtractTheGraphql()
        {
            var instagramPost = new InstagramPost
            {
                Shortcode = "B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0",
                Url = new Uri("https://instagram.com/p/B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0/"),
                HtmlContent =
                    @"<!DOCTYPE html><html lang=""en"" class=""no-js logged-in client-root""><body>
                    <script type=""text/javascript"">window.__initialDataLoaded(window._sharedData);</script>
                    <script type=""text/javascript"">window._sharedData = { ""config"": { ""csrf_token"": ""12345678"" }, ""entry_data"": { ""PostPage"": [{ ""graphql"": {""shortcode_media"":{""__typename"":""GraphImage"",""id"":""1280303637280241311"",""shortcode"":""B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0""}} }] },""hostname"": ""www.instagram.com"" };</script>
                    </body>"
            };

            var grapqhl = InstagramPostScraper.ExtractGraphql(instagramPost).ToArray();

            Assert.Single(grapqhl);
            Assert.Equal("B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0", grapqhl[0].Graphql.ShortCodeMedia.ShortCode);
        }

        [Fact]
        public void ExtractMediaFromGraphql_GivenSingleImageGraphql_ItExtractTheMedia()
        {
            var graphql = @"
            {
                ""graphql"": {
                    ""shortcode_media"": {
                        ""__typename"": ""GraphImage"",
                        ""id"": ""1280303637280241311"",
                        ""shortcode"": ""B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0"",
                        ""display_url"": ""https:\/\/scontent-mxp1-1.cdninstagram.com\/v\/t51.2885-15\/e35\/s1080x1080\/91997827_2018758304934784_3089063750128038216_n.jpg?param1=1&param2=2"",
                        ""is_video"": false
                    }
                }
            }";

            var media = InstagramPostScraper.ExtractMediaFromGraphql(graphql).ToArray();

            Assert.Single(media);
            Assert.Equal("B-lRBOYHraeXNozRvPCrtJ9h0esayWpvMjyQAE0", media[0].Shortcode);
            Assert.Equal("https://scontent-mxp1-1.cdninstagram.com/v/t51.2885-15/e35/s1080x1080/91997827_2018758304934784_3089063750128038216_n.jpg?param1=1&param2=2", media[0].Url.AbsoluteUri);
            Assert.Equal("91997827_2018758304934784_3089063750128038216_n.jpg", media[0].Name);
        }
    }
}