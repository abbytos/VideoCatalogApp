using Microsoft.AspNetCore.Hosting;
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
    /// Tests cover various scenarios for video handling: index page, playing existing and non-existing videos, and retrieving videos as JSON.
    /// </summary>
    public class HomeControllerTests : TestBase<Program>
    {
        public HomeControllerTests() : base()
        {
            SetupMediaFolder();
        }

        /// <summary>
        /// Tests that Index action returns a ViewResult containing a list of VideoFileModel.
        /// </summary>
        [Fact]
        public void Index_ReturnsViewResultWithVideos()
        {
            // Arrange

            // Act
            var response = Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(response);
            var model = Assert.IsAssignableFrom<List<VideoFileModel>>(viewResult.ViewData.Model);

            Assert.NotNull(model);
            Assert.All(model, item =>
            {
                Assert.False(string.IsNullOrEmpty(item.FileName));
                Assert.True(item.FileSize > 0);
            });
        }

        /// <summary>
        /// Tests that Play action returns PhysicalFileResult when the file exists.
        /// </summary>
        [Fact]
        public void Play_ReturnsPhysicalFileResult_WhenFileExists()
        {
            // Arrange
            var testFileName = "test.mp4";
            var expectedFilePath = Path.Combine(MediaFolderPath, testFileName);

            // Act
            var result = Play(testFileName);

            // Assert
            var physicalFileResult = Assert.IsType<PhysicalFileResult>(result);
            Assert.Equal("video/mp4", physicalFileResult.ContentType);
            Assert.Equal(expectedFilePath, physicalFileResult.FileName);
        }

        /// <summary>
        /// Tests that Play action returns NotFoundResult when the file does not exist.
        /// </summary>
        [Fact]
        public void Play_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var nonExistentFileName = "nonexistent.mp4";

            // Act
            var result = Play(nonExistentFileName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Tests that GetVideos action returns JsonResult with a list of VideoFileModel.
        /// </summary>
        [Fact]
        public void GetVideos_ReturnsJsonResultWithVideos()
        {
            // Arrange

            // Act
            var result = GetVideos();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var model = Assert.IsAssignableFrom<List<VideoFileModel>>(jsonResult.Value);
            Assert.NotNull(model);
            Assert.All(model, item =>
            {
                Assert.False(string.IsNullOrEmpty(item.FileName));
                Assert.True(item.FileSize > 0);
            });
        }

        // Private methods

        /// <summary>
        /// Helper method to create an instance of HomeController with mock dependencies.
        /// </summary>
        /// <returns>An instance of HomeController.</returns>
        private HomeController CreateController()
        {
            var mockLogger = new Mock<ILogger<HomeController>>();
            var hostingEnvironment = _scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var options = Options.Create(new HomeControllerOptions { MediaFolderName = MediaFolderName });
            return new HomeController(hostingEnvironment, options, mockLogger.Object);
        }

        /// <summary>
        /// Helper method to call Index action via HomeController instance.
        /// </summary>
        /// <returns>The IActionResult returned by Index action.</returns>
        private IActionResult Index()
        {
            var controller = CreateController();
            return controller.Index();
        }

        /// <summary>
        /// Helper method to call GetVideos action via HomeController instance.
        /// </summary>
        /// <returns>The IActionResult returned by GetVideos action.</returns>
        private IActionResult GetVideos()
        {
            var controller = CreateController();
            return controller.GetVideos();
        }

        /// <summary>
        /// Helper method to call Play action via HomeController instance.
        /// </summary>
        /// <param name="file">The file name to play.</param>
        /// <returns>The IActionResult returned by Play action.</returns>
        private IActionResult Play(string file)
        {
            var mockLogger = new Mock<ILogger<HomeController>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(env => env.WebRootPath).Returns(ProjectFolderPath);

            var options = Options.Create(new HomeControllerOptions { MediaFolderName = MediaFolderName });
            var controller = new HomeController(mockEnvironment.Object, options, mockLogger.Object);

            return controller.Play(file);
        }
    }
}
