using Microsoft.Extensions.Logging;

using System.Net.Http;

namespace WebScraper.Services
{
    public abstract class ServiceBase
    {
        protected static HttpClient HttpClient { get; } = new HttpClient();
        protected ILogger<ServiceBase> Logger { get; }

        public ServiceBase(ILogger<ServiceBase> logger)
        {
            Logger = logger;
        }
    }
}
