using Microsoft.Extensions.Logging;

using System;
using System.Threading;

using WebScraper.Contracts;

namespace WebScraper.Services
{
    public class Timeouter : ITimeouter
    {
        private readonly ILogger<Timeouter> _logger;

        public Timeouter(ILogger<Timeouter> logger)
        {
            _logger = logger;
        }

        public CancellationToken GetCancellationWithTimeout(int timeout, string methodName = null, Action onCancellationRequested = null)
        {
            var cts = new CancellationTokenSource();
            var cancellation = cts.Token;

            cancellation.Register(() => _logger.LogDebug($"{methodName}: Timeout"));
            if (onCancellationRequested is not null )
                cancellation.Register(onCancellationRequested);

            cts.CancelAfter(timeout);

            return cancellation;
        }
    }
}
