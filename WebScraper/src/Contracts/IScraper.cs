using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WebScraper.Models;

namespace WebScraper.Contracts
{
    public interface IScraper
    {
        public Task<List<ScrapedImage>> ScrapeImagesAsync(string url, int imageCount, CancellationToken cancellation);
    }
}
