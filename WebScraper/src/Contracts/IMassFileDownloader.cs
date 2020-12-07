using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebScraper.Contracts
{
    public interface IMassFileDownloader<T>
    {
        public Task<List<T>> DownloadAsync(IEnumerable<string> urls, int threadCount, CancellationToken cancellation);
    }
}
