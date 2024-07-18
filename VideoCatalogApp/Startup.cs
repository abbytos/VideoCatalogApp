using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using VideoCatalogApp.Controllers;
using VideoCatalogApp.Models;

namespace VideoCatalogApp
{
    /// <summary>
    /// Configures services and the HTTP request pipeline for the application.
    /// </summary>
    public class Startup
    {
        private const long MaxFileSizeBytes = 200 * 1024 * 1024; // 200 MB limit

        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class with the provided configuration.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Configures the services used by the application.
        /// </summary>
        /// <param name="services">The collection of services to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add controllers and views support
            services.AddControllersWithViews();

            // Configure the multipart body length limit
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = MaxFileSizeBytes; // Set the maximum upload size
            });

            // Configure HomeController options
            services.Configure<HomeControllerOptions>(Configuration.GetSection("HomeControllerOptions"));

            // Register the HomeController with dependencies
            services.AddTransient<HomeController>((serviceProvider) =>
            {
                var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var logger = serviceProvider.GetRequiredService<ILogger<HomeController>>();
                var options = serviceProvider.GetRequiredService<IOptions<HomeControllerOptions>>();

                return new HomeController(env, options, logger);
            });
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
