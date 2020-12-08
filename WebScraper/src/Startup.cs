using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

using WebScraper.Contracts;
using WebScraper.Models.DTO;
using WebScraper.Services;

namespace WebScraper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAppSettings>(provider => 
            {
                var maxExecTime = Configuration.GetValue<int>("MaxExecutionTime:Images:Get");
                return new AppSettings() { MaxExecutionTime = TimeSpan.FromMilliseconds(maxExecTime) };
            });
            services.AddScoped<IMassFileDownloader<DownloadedImage>, ImagesDownloader>();
            services.AddScoped<IScraper, Scraper>();
            services.AddScoped<ITimeouter, Timeouter>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                var exception = context.HttpContext.Features.Get<IExceptionHandlerFeature>().Error;

                await context.HttpContext.Response.WriteAsync(
                    String.Format(
                        "{\"error\": \"{0}\"}",
                        exception.Message
                    )
                );
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
