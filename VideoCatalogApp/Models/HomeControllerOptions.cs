namespace VideoCatalogApp.Models
{
    /// <summary>
    /// Options class for configuring settings related to HomeController.
    /// </summary>
    public class HomeControllerOptions
    {
        /// <summary>
        /// Gets or sets the name of the media folder where video files are stored.
        /// </summary>
        public string MediaFolderName { get; set; }


        /// <summary>
        /// Gets or sets the size of the media video file.
        /// </summary>
        public long MaxFileSizeBytes { get; set; } 
    }
}

