using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using WebScraper.Contracts;
using WebScraper.Models.DTO;

namespace WebScraper.Services
{
    public class Scraper : ServiceBase, IScraper
    {
        const string SRC_GROUP = "SRC";
        const string ALT_GROUP = "ALT";

        //I'm using regex for html parsing because of my job task conditions. 
        //In a production you should use some 3rd-party html parser.
        private readonly Regex _imgPattern = new Regex(
            $"(?=<img[^>]+(src=\"(?<{SRC_GROUP}>[^\"]+)\"))" +
            $"(<img[^>]+(alt=\"(?<{ALT_GROUP}>[^\"]+)\"))",
            RegexOptions.Multiline
        );
        private readonly IAppSettings _settings;

        public Scraper(IAppSettings settings, ILogger<Scraper> logger) : base(logger)
        {
            _settings = settings;
        }

        public async Task<List<ScrapedImage>> ScrapeImagesAsync(Uri url, int imageCount, CancellationToken cancellation)
        {
            Logger.LogDebug($"send request to {url}");
            var response = await HttpClient.GetAsync(url, cancellation);
            response.EnsureSuccessStatusCode();

            Logger.LogDebug("read html");
            var html = await response.Content.ReadAsStringAsync(cancellation);

            Logger.LogDebug($"scrape {imageCount} images from {url}");
            var scraped = ScrapeImages(html, imageCount);
            Logger.LogDebug($"{scraped.Count} images scraped from {url}");

            return scraped;
        }

        private List<ScrapedImage> ScrapeImages(string html, int imageCount)
        {
            Logger.LogDebug("parse html and find IMG tags");
            var matched = _imgPattern.Matches(html);
            var matchedLength = matched.Count();
            Logger.LogDebug($"{matchedLength} IMG tags found{(matchedLength > imageCount ? $", take {imageCount} from them" : "")}");

            return matched
                .Take(imageCount)
                .Select(
                m =>
                {
                    var groups = m.Groups;
                    Group src = groups[SRC_GROUP];
                    groups.TryGetValue(ALT_GROUP, out Group alt);

                    return new ScrapedImage(src.Value, alt?.Value);
                })
                .ToList();
        }
    }
}
