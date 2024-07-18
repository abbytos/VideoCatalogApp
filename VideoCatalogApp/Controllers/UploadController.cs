using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VideoCatalogApp.Models;

namespace VideoCatalogApp.Controllers
{
    /// <summary>
    /// API controller for handling file uploads.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly long _maxFileSizeBytes;
        private readonly string _mediaFolderPath;
        private readonly ILogger<UploadController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        /// <param name="env">The hosting environment providing information about the web host environment.</param>
        /// <param name="options">The options for configuring the home controller.</param>
        /// <param name="logger">The logger for capturing diagnostic logs.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="env"/>, <paramref name="options"/>, or <paramref name="logger"/> is null.</exception>
        public UploadController(IWebHostEnvironment env, IOptions<HomeControllerOptions> options, ILogger<UploadController> logger)
        {
            _mediaFolderPath = Path.Combine(
                env?.WebRootPath ?? throw new ArgumentNullException(nameof(env)),
                options?.Value?.MediaFolderName ?? throw new ArgumentNullException(nameof(options))
            );

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _maxFileSizeBytes = options?.Value?.MaxFileSizeBytes ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Handles file upload. Checks total file size and uploads valid MP4 files to the media folder.
        /// </summary>
        /// <param name="files">List of files to upload</param>
        /// <returns>ActionResult representing the upload result</returns>
        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            try
            {
                _logger.LogInformation("Received {0} files for upload.", files.Count);

                // Calculate total size of uploaded files
                var totalSizeBytes = files.Sum(file => file.Length);

                // Check if total file size exceeds the limit
                if (totalSizeBytes > _maxFileSizeBytes)
                {
                    _logger.LogWarning("Total file size exceeds 200 MB. Request rejected.");
                    return StatusCode(StatusCodes.Status413RequestEntityTooLarge, "Total file size exceeds 200 MB. Please upload smaller files.");
                }

                // Process each file
                foreach (var file in files)
                {
                    if (file.Length > 0 && Path.GetExtension(file.FileName).ToLower() == ".mp4")
                    {
                        var filePath = Path.Combine(_mediaFolderPath, Path.GetFileName(file.FileName));

                        // Save the file to the media folder
                        try
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            _logger.LogInformation("File '{0}' uploaded successfully.", file.FileName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred while uploading the file '{0}.", file.FileName);
                            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while uploading the file: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Log and return BadRequest for non-MP4 files
                        _logger.LogWarning("Rejected file '{0}' because it is not an MP4 file.", file.FileName);
                        return BadRequest("Only MP4 files are allowed.");
                    }
                }

                return Ok(); // Return OK if all files are processed successfully
            }
            catch (HttpRequestException ex)
            {
                // Handle network-related errors
                _logger.LogError(ex, "A network error occurred during file upload.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "A network error occurred during file upload.");
            }
            catch (Exception ex)
            {
                // Log any other exceptions that occur during file upload
                _logger.LogError(ex, "An error occurred during file upload.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during file upload.");
            }
        }
    }
}
