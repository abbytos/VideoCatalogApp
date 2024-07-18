# Video Catalog Web Application

**Developed by Albena Roshelova**

## Summary

This project is an ASP.NET Core 6 MVC Web Application designed to manage and playback MP4 video files uploaded to a server's media folder. It provides users with a simple interface to upload, view, and play back videos.

## Features

- **Upload Files:** Allows users to upload MP4 video files to the server.
- **Catalog View:** Displays a list of uploaded videos with filename and size.
- **Video Playback:** Plays back selected videos in a video player on the same page.
- **Responsive Design:** Supports a responsive front-end design across various screen widths.
- **Error Handling:** Properly handles file upload errors and size restrictions.
- **Web API:** Includes an ASP.NET Core Web API for handling file uploads.

## Technologies Used

- **ASP.NET Core 6 MVC:** Backend framework for building web applications using the Model-View-Controller pattern in .NET.
- **C#:** Primary programming language for backend development in the .NET ecosystem.
- **HTML, CSS, JavaScript:** Front-end technologies for creating the user interface and adding interactivity to web pages.
- **Bootstrap:** CSS framework used for responsive web design and UI components.
- **jQuery:** JavaScript library for simplifying DOM manipulation and handling asynchronous operations.
- **KnockoutJS:** JavaScript library for implementing the Model-View-ViewModel (MVVM) pattern, enabling dynamic UI updates and data binding.
- **Visual Studio 2022:** Integrated development environment (IDE) for .NET developers.
- **Mocking Frameworks (Moq):** Framework used in testing to create mock objects for unit testing purposes.

## Setup Instructions

1. **Open Solution:** Open the solution file (`VideoCatalogApp.sln`) in Visual Studio 2022.
2. **Restore Packages:** Restore NuGet packages if needed.
3. **Run Application:** Use Kestrel (`dotnet run`) or IIS Express in Visual Studio to run the application.
4. **Access Application:** Navigate to `https://localhost:7013/` in your web browser to access the application.

## Testing

- **Unit Tests:** Includes unit tests using xUnit and Moq for mocking dependencies.

## Future Enhancements

- Enhance UI/UX with more interactive features.
- Improve error handling and logging for better diagnostics.
- Implement automatic retry logic for network operations, such as file uploads, to handle transient errors like network timeouts or temporary service unavailability.
- Introduce caching mechanisms to optimize performance, especially for frequently accessed video files.
- Incorporate analytics or monitoring tools to gain insights into application usage and performance.
- Develop a notification system to inform users about upload progress or status updates.

## Deployment

Deploy the application to a production environment following standard ASP.NET Core deployment practices.

## Notes

- Total time spent on this challenge: ~ 6 hours.
