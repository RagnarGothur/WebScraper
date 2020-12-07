using System;
using System.ComponentModel.DataAnnotations;

namespace WebScraper.Models.Input.Images
{
    public class Get
    {
        [Required]
        public string Url { get; init; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ThreadCount { get; init; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ImageCount { get; init; }
    }
}
