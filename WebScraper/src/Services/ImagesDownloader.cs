using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using WebScraper.Contracts;
using WebScraper.Models.DTO;

namespace WebScraper.Services
{
    public class ImagesDownloader : ServiceBase, IMassFileDownloader<DownloadedImage>
    {
        private const string IMAGES_PATH = @".files\images";//..\..\..\..\..\WebScraper\.files\images

        private readonly IAppSettings _settings;
        protected string DownloadPath { get; } = $"{IMAGES_PATH}\\{DateTime.Today.Date.ToShortDateString()}"; //e.g. 08.12.2020

        public ImagesDownloader(IAppSettings settings, ILogger<ImagesDownloader> logger) : base(logger)
        {
            _settings = settings;
        }

        public async Task<List<DownloadedImage>> DownloadAsync(IEnumerable<string> sources, int threadCount, CancellationToken cancellation)
        {
            var maxConcurrentDownloads = threadCount > _settings.MaxThreadCount ? _settings.MaxThreadCount : threadCount;
            var semaphoreSlim = new SemaphoreSlim(maxConcurrentDownloads);
            var tasks = new List<Task>();
            var downloadedImages = new ConcurrentBag<DownloadedImage>();
            var downloadPath = DownloadPath;

            //Ensuring dir already created
            if (sources.Any())
                Directory.CreateDirectory(downloadPath);

            //TODO: Count() can take a long time
            Logger.LogDebug($"downloading {sources.Count()} images in {threadCount} threads");
            foreach (string src in sources)
            {
                await semaphoreSlim.WaitAsync(cancellation);

                //Partially completed task in the case of cancellation requested
                if (cancellation.IsCancellationRequested)
                    return downloadedImages.ToList();

                Task t = Task.Run(async () =>
                {
                    try
                    {
                        var data = await HttpClient.GetByteArrayAsync(src, cancellation);
                        var fileSize = data.Length; //TODO: check whether is it actual image size

                        if (cancellation.IsCancellationRequested)
                            return;

                        var img = Image.FromStream(new MemoryStream(data));

                        var filePath = $"{downloadPath}\\{Guid.NewGuid()}";

                        img.Save(filePath, ImageFormat.Png);

                        downloadedImages.Add(new DownloadedImage(src, $"{filePath}.png", fileSize));
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                });

                tasks.Add(t);
            }

            await Task.WhenAll(tasks.ToArray());
            Logger.LogDebug($"{downloadedImages.Count} images have been downloaded");

            return downloadedImages.ToList();
        }
    }
}
