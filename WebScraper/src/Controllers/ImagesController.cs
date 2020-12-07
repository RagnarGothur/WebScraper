using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using WebScraper.Contracts;
using WebScraper.Models;

namespace WebScraper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
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
            [Required, Range(0, int.MaxValue), FromQuery] int imageCount //Or maybe just to use uint...?
        )
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                ModelState.AddModelError($"{url} is invalid url", nameof(url));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cancellation = _timeouter.GetCancellationWithTimeout((int)_settings.MaxExecutionTime.TotalMilliseconds);
            var scraped = await _scraper.ScrapeImagesAsync(url, imageCount, cancellation);
            var downloaded = await _imagesDownloader.DownloadAsync(scraped.Select(s => s.Src), threadCount, cancellation);

            return new JsonResult(downloaded);
        }
    }
}
