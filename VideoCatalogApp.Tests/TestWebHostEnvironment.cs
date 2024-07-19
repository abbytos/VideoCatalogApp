using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace VideoCatalogApp.Tests
{
    /// <summary>
    /// Custom implementation of <see cref="IWebHostEnvironment"/> for testing purposes.
    /// This allows overriding the default WebRootPath to point to a test-specific directory.
    /// </summary>
    public class TestWebHostEnvironment : IWebHostEnvironment
    {
        /// <summary>
        /// The root directory of the web application used to serve static files during testing.
        /// </summary>
        public string WebRootPath { get; set; }

        /// <summary>
        /// Provides access to the file system of the WebRootPath.
        /// </summary>
        public IFileProvider WebRootFileProvider { get; set; }

        /// <summary>
        /// The name of the application.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The name of the environment (e.g., Development, Staging, Production).
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// The root directory of the application used to serve content files during testing.
        /// </summary>
        public string ContentRootPath { get; set; }

        /// <summary>
        /// Provides access to the file system of the ContentRootPath.
        /// </summary>
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}