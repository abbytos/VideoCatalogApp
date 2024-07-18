function AppViewModel() {
    var self = this;

    // Observables
    self.videos = ko.observableArray([]);           // Observable array for videos
    self.selectedVideo = ko.observable(null);       // Observable for selected video
    self.selectedFiles = ko.observableArray([]);    // Observable array for selected files
    self.uploadMessage = ko.observable('');         // Observable for upload message
    self.uploadMessageColor = ko.observable('');    // Observable for upload message color

    // Observable for tracking files exceeding the size limit
    self.filesWithSizeInfo = ko.observableArray([]);

    // Computed observable to check if any file exceeds the size limit
    self.exceedingFilesExist = ko.computed(function () {
        return self.filesWithSizeInfo().some(file => file.exceedsLimit);
    });

    // Triggered when files are selected
    self.onFileChange = function (data, event) {
        var files = event.target.files;
        var fileArray = Array.from(files).map(file => {
            return {
                name: file.name,
                size: (file.size / (1024 * 1024)).toFixed(2), // Convert bytes to MB and format to 2 decimal places
                exceedsLimit: file.size > 200 * 1024 * 1024,
                isMp4: file.type === 'video/mp4'
            };
        });
        self.filesWithSizeInfo(fileArray);
        self.selectedFiles(files);

        // Show the table when files are selected
        var tableElement = document.getElementById('selected-files-table');
        if (tableElement) {
            tableElement.style.display = 'table'; // Display the table
        }
    };

    // Function to convert bytes to MB
    function bytesToMB(bytes) {
        return (bytes / (1024 * 1024)).toFixed(2); // Round to 2 decimal places
    }

    // Upload files asynchronously
    self.uploadFiles = async function () {
        var formData = new FormData();
        var filesExceedingLimit = [];

        // Hide the table before uploading
        var tableElement = document.getElementById('selected-files-table');
        if (tableElement) {
            tableElement.style.display = 'none';
        }

        for (var i = 0; i < self.selectedFiles().length; i++) {
            var file = self.selectedFiles()[i];
            if (file.size > 200 * 1024 * 1024) {
                filesExceedingLimit.push(file.name); // Collect names of files exceeding the limit
            } else {
                formData.append('files', file); // Add files within limit to formData
            }
        }

        if (filesExceedingLimit.length > 0) {
            // Display files exceeding limit in the error message
            self.uploadMessage('Files exceeding 200 MB limit: ' + filesExceedingLimit.join(', '));
            self.uploadMessageColor('red');
            return; // Abort upload if any file exceeds the limit
        }

        try {
            const response = await fetch('/api/upload', {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                self.loadVideos();
                self.uploadMessage('Upload successful');
                self.uploadMessageColor('green');
                // Clear upload message after 5 seconds
                setTimeout(function () {
                    self.uploadMessage('');
                    self.uploadMessageColor('');
                }, 5000); // 5000 milliseconds (5 seconds)

                // Reset file input value after successful upload
                var fileInput = document.getElementById('fileInput');
                if (fileInput) {
                    fileInput.value = ''; // Clear the file input value
                } else {
                    console.error('File input element not found.');
                }
            } else {
                self.uploadMessage('Error uploading files');
                self.uploadMessageColor('red');
            }

        } catch (error) {
            console.error('Error:', error);
            self.uploadMessage('Error: ' + error.message);
            self.uploadMessageColor('red');
        }
    };

    // Load videos asynchronously
    self.loadVideos = async function () {
        try {
            const response = await fetch('/Home/GetVideos');
            if (response.ok) {
                const data = await response.json();
                self.videos(data);
            } else {
                // Error loading videos
                self.uploadMessage('Error loading videos');
                self.uploadMessageColor('red');
            }
        } catch (error) {
            // Exception handling
            console.error('Error:', error);
            self.uploadMessage('Error: ' + error.message);
            self.uploadMessageColor('red');
        }
    };

    // Play selected video
    self.playVideo = function (video) {
        self.selectedVideo('/Home/Play?fileName=' + video.fileName);
    };

    // Initial load of videos
    self.loadVideos();
}

// Initialize and bind view model
var viewModel = new AppViewModel();
ko.applyBindings({ viewModel: viewModel }, document.getElementById('app'));
