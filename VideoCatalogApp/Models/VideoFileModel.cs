namespace VideoCatalogApp.Models
{
    /// <summary>
    /// Represents a model for video file details including name and size.
    /// </summary>
    public class VideoFileModel
    {
        /// <summary>
        /// Gets or sets the name of the video file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the video file in bytes.
        /// </summary>
        public long FileSize { get; set; }
    }
}
