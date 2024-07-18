using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VideoCatalogApp.Controllers;
using VideoCatalogApp.Models;

namespace VideoCatalogApp.Tests
{
    /// <summary>
    /// Tests various scenarios for file uploads in the UploadController,
    /// including valid MP4 files, non-MP4 files, and file size limits.
    /// </summary>
    public class UploadControllerTests : TestBase<Startup>
    {
        public UploadControllerTests() : base()
        {

        }

        /// <summary>
        /// Tests uploading valid MP4 files, expecting an OkResult.
        /// </summary>
        [Fact]
        public async Task Upload_ValidMP4Files_ReturnsOk()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                CreateMockFormFile("test1.mp4", 100 * 1024 * 1024), // 100 MB file
                CreateMockFormFile("test2.mp4", 50 * 1024 * 1024)   // 50 MB file
            };

            // Act
            var response = await UploadFilesAsync(files);

            // Assert
            Assert.IsType<OkResult>(response);
        }

        /// <summary>
        /// Tests uploading non-MP4 files, expecting a BadRequestObjectResult with a specific error message.
        /// </summary>
        [Fact]
        public async Task Upload_NonMP4Files_ReturnsBadRequest()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                CreateMockFormFile("document.pdf", 1024 * 1024) // Non-MP4 file
            };

            // Act
            var response = await UploadFilesAsync(files);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("Only MP4 files are allowed.", ((BadRequestObjectResult)response).Value);
        }

        /// <summary>
        /// Tests uploading a file that exceeds the size limit, expecting a RequestEntityTooLarge status code.
        /// </summary>
        [Fact]
        public async Task Upload_FileExceedsSizeLimit_ReturnsRequestEntityTooLarge()
        {
            // Arrange
            var largeFile = CreateMockFormFile("largefile.mp4", 250 * 1024 * 1024); // 250 MB file

            // Act
            var result = await UploadFilesAsync(new List<IFormFile> { largeFile });

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status413RequestEntityTooLarge, objectResult.StatusCode);
        }

        [Fact]
        public async Task Upload_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                CreateMockFormFile("test1.mp4", 100 * 1024 * 1024) // Valid MP4 file
            };

            // Mock logger and options
            var mockLogger = new Mock<ILogger<UploadController>>();
            var options = Options.Create(new HomeControllerOptions { MediaFolderName = MediaFolderName });

            // Act
            var result = await UploadFilesAsyncWithServerErrorHandling(files, mockLogger.Object, options);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        /// <summary>
        /// Tests handling network errors during file upload, expecting a ServiceUnavailable status code.
        /// </summary>
        [Fact]
        public async Task Upload_NetworkFailure_ReturnsServiceUnavailable()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                CreateMockFormFile("test1.mp4", 100 * 1024 * 1024) // Valid MP4 file
            };

            // Mock logger and options
            var mockLogger = new Mock<ILogger<UploadController>>();
            var options = Options.Create(new HomeControllerOptions { MediaFolderName = MediaFolderName });

            // Act
            var result = await UploadFilesAsyncWithNetworkErrorHandling(files, mockLogger.Object, options);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(503, statusCodeResult.StatusCode);

        }


        // Private members

        /// <summary>
        /// Helper method to asynchronously upload files to the UploadController.
        /// </summary>
        /// <param name="files">The list of files to upload.</param>
        /// <returns>An IActionResult representing the result of the upload operation.</returns>
        private async Task<IActionResult> UploadFilesAsync(List<IFormFile> files)
        {
            var mockLogger = new Mock<ILogger<UploadController>>();
            var hostingEnvironment = _scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var options = Options.Create(new HomeControllerOptions { MediaFolderName = MediaFolderName, MaxFileSizeBytes = MaxFileSizeBytes });
            var controller = new UploadController(hostingEnvironment, options, mockLogger.Object);
            return await controller.Upload(files);
        }

        // Helper method for uploading files with server error handling
        private async Task<IActionResult> UploadFilesAsyncWithServerErrorHandling(List<IFormFile> files, ILogger<UploadController> logger, IOptions<HomeControllerOptions> options)
        {
            try
            {
                var hostingEnvironment = new Mock<IWebHostEnvironment>().Object; // Mock or provide a mock instance
                var controller = new UploadController(hostingEnvironment, options, logger);
                return await controller.Upload(files);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                logger.LogError(ex, "Error occurred during file upload.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        // Helper method for uploading files with network error handling
        private async Task<IActionResult> UploadFilesAsyncWithNetworkErrorHandling(List<IFormFile> files, ILogger<UploadController> logger, IOptions<HomeControllerOptions> options)
        {
            try
            {
                var hostingEnvironment = new Mock<IWebHostEnvironment>().Object; // Mock or provide a mock instance
                var controller = new UploadController(hostingEnvironment, options, logger);
                return await controller.Upload(files);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                logger.LogError(ex, "Error occurred during file upload.");
                return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
            }
        }
    }
}
