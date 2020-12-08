using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WebScraper.Models.DTO;

namespace WebScraper.Contracts
{
    public interface IScraper
    {
        public Task<List<ScrapedImage>> ScrapeImagesAsync(Uri url, int imageCount, CancellationToken cancellation);
    }
}
