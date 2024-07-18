using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace VideoCatalogApp.Tests
{
    public class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }

        // Other properties of IWebHostEnvironment, you can set them as needed
        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}