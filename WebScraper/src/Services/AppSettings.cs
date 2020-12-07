using System;

using WebScraper.Contracts;

namespace WebScraper.Services
{
    public class AppSettings : IAppSettings
    {
        public TimeSpan MaxExecutionTime { get; init; }
        public int MaxThreadCount { get; } = Environment.ProcessorCount;
    }
}
