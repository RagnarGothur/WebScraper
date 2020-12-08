using System.Collections.Generic;

namespace WebScraper.Models.Responses
{
    public class GroupedImages
    {
        public string Host { get; init; }

        public List<ImageInfo> Images { get; init; } = new List<ImageInfo>();
    }
}
