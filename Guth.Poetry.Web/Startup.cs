
using Blazored.LocalStorage;

using Guth.PoetryDB;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MudBlazor.Services;

using RestSharp;

namespace Guth.Poetry.Web
{
    public class Startup
    {
        private const string POETRYDB_ORIGIN = "_poetryDbOrigin";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddOptions();
            services.AddTransient<IPoetryDBClient>(services => new PoetryDBClient(new RestClient("https://poetrydb.org")));
            services.AddCors(options =>
            {
                options.AddPolicy(POETRYDB_ORIGIN, builder => builder.WithOrigins("https://poetrydb.org"));
            });
            services.AddMudServices();
            services.AddBlazoredLocalStorage();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
