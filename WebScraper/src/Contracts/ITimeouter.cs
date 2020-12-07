using System;
using System.Threading;

namespace WebScraper.Contracts
{
    public interface ITimeouter
    {
        public CancellationToken GetCancellationWithTimeout(int timeout, string methodName = null, Action onCancellationRequested = null);
    }
}
