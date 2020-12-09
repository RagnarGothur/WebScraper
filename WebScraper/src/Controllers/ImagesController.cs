using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using WebScraper.Contracts;
using WebScraper.Models;
using WebScraper.Models.DTO;
using WebScraper.Models.Responses;

namespace WebScraper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private const string FULLY_COMPLETED_HEADER = "Fully-Completed";

        private readonly ILogger<ImagesController> _logger;
        private readonly IAppSettings _settings;
        private readonly IScraper _scraper;
        private readonly ITimeouter _timeouter;
        private readonly IMassFileDownloader<DownloadedImage> _imagesDownloader;

        public ImagesController(
            ILogger<ImagesController> logger,
            IAppSettings settings,
            IScraper scraper,
            ITimeouter timeouter,
            IMassFileDownloader<DownloadedImage> imagesDownloader
        )
        {
            _logger = logger;
            _settings = settings;
            _scraper = scraper;
            _timeouter = timeouter;
            _imagesDownloader = imagesDownloader;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(
            [Required, FromQuery] string url,
            [Required, Range(1, int.MaxValue), FromQuery] int threadCount,
            [Required, Range(0, int.MaxValue), FromQuery] int imageCount //Or maybe just to use uint...
        )
        {
            Uri uri = null;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException)
            {
                ModelState.AddModelError(nameof(url), $"Url misformat: {url}");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cancellation = _timeouter.GetCancellationWithTimeout((int)_settings.MaxExecutionTime.TotalMilliseconds);
            var scraped = await _scraper.ScrapeImagesAsync(uri, imageCount, cancellation);
            var downloaded = await _imagesDownloader.DownloadAsync(scraped.Select(s => s.Src), threadCount, cancellation);

            Response.Headers.Add(FULLY_COMPLETED_HEADER, cancellation.IsCancellationRequested ? "0": "1");

            var result = scraped
                .Join(downloaded, s => s.Src, d => d.Src, (s, d) => new ImageInfo(s.Src, s.Alt, d.Size))
                .GroupBy(i => new Uri(i.Src).Host)
                .Select(g => new GroupedImages() { Host = g.Key, Images = g.ToList() })
                .ToList();

            return new JsonResult(result);
        }
    }
}
