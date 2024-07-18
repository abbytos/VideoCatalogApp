using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VideoCatalogApp.Models;


namespace VideoCatalogApp.Controllers
{
    /// <summary>
    /// Controller responsible for managing video-related actions such as listing, playing, and uploading.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly string _mediaFolderPath;
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="env">The hosting environment providing web root path.</param>
        /// <param name="options">Options for configuring the media folder name.</param>
        /// <param name="logger">Logger instance for logging errors and information.</param>
        public HomeController(IWebHostEnvironment env, IOptions<HomeControllerOptions> options, ILogger<HomeController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediaFolderPath = Path.Combine(env?.WebRootPath ?? throw new ArgumentNullException(nameof(env)),
                               options?.Value?.MediaFolderName ?? throw new ArgumentNullException(nameof(options)));
        }

        /// <summary>
        /// Retrieves a list of MP4 videos from the media folder and displays them in the view.
        /// </summary>
        /// <returns>View with a model containing video file information.</returns>
        public IActionResult Index()
        {
            try
            {
                var model = Directory.GetFiles(_mediaFolderPath, "*.mp4")
                .Select(f => new VideoFileModel
                {
                    FileName = Path.GetFileName(f),
                    FileSize = new FileInfo(f).Length
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching videos in Index method.");
                throw;
            }
        }

        /// <summary>
        /// Streams a specified video file for playback.
        /// </summary>
        /// <param name="fileName">Name of the video file to play.</param>
        /// <returns>Physical file response with the video content.</returns>
        public IActionResult Play(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_mediaFolderPath, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                return PhysicalFile(filePath, "video/mp4");
            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Error playing video: {fileName}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a list of MP4 videos from the media folder as JSON data.
        /// </summary>
        /// <returns>JSON response containing video file information.</returns>
        [HttpGet]
        public IActionResult GetVideos()
        {
            try
            {
                var videos = Directory.GetFiles(_mediaFolderPath, "*.mp4")
                   .Select(f => new VideoFileModel
                   {
                       FileName = Path.GetFileName(f),
                       FileSize = new FileInfo(f).Length
                   }).ToList();

                return Json(videos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching videos in GetVideos method.");
                throw;
            }
        }
    }
}
