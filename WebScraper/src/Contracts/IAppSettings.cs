using System;

namespace WebScraper.Contracts
{
    public interface IAppSettings
    {
        public TimeSpan MaxExecutionTime { get; }
        public int MaxThreadCount { get; }
    }
}
