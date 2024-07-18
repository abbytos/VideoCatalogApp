using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace VideoCatalogApp.Tests
{
    // Base class for tests
    public class TestBase<TStartup> : IAsyncLifetime, IDisposable where TStartup : class
    {
        protected IServiceScope _scope;
        protected readonly WebApplicationFactory<TStartup> _factory;
        protected static readonly string MediaFolderName = "media";
        protected readonly string ProjectFolderPath;
        protected readonly string MediaFolderPath;

        public TestBase()
        {
            _factory = new WebApplicationFactory<TStartup>();

            // Determine the project folder path dynamically and make it absolute
            ProjectFolderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\.."));

            // Construct the media folder path
            MediaFolderPath = Path.Combine(ProjectFolderPath, MediaFolderName);
        }

        // Setup media folder and ensure a test file is available
        protected void SetupMediaFolder()
        {

            if (!Directory.Exists(MediaFolderPath))
            {
                Directory.CreateDirectory(MediaFolderPath);
            }

            // Create a test file if it doesn't exist
            var testFilePath = Path.Combine(MediaFolderPath, "test.mp4");
            if (!File.Exists(testFilePath))
            {
                File.WriteAllText(testFilePath, "dummy content");
            }
        }

        // Create a mock form file for testing purposes
        protected IFormFile CreateMockFormFile(string fileName, long size)
        {
            var stream = new MemoryStream(new byte[size]);
            return new FormFile(stream, 0, size, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "video/mp4"
            };
        }

        public async Task InitializeAsync()
        {
            _scope = _factory.Services.CreateScope();
            await OnInitializeAsync();
        }

        public async Task DisposeAsync()
        {
            await OnDisposeAsync();
            _scope.Dispose();
            _factory.Dispose();

            // Clean up media folder and files
            var mediaFolderPath = Path.Combine(Directory.GetCurrentDirectory(), MediaFolderName);
            if (Directory.Exists(mediaFolderPath))
            {
                Directory.Delete(mediaFolderPath, true);
            }
        }

        protected virtual Task OnInitializeAsync() => Task.CompletedTask;

        protected virtual Task OnDisposeAsync() => Task.CompletedTask;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scope.Dispose();
                _factory.Dispose();
            }
        }
    }
}
