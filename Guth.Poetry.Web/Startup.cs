
using Guth.Poetry.Db;
using Guth.Poetry.Web.Data;
using Guth.PoetryDB;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddOptions();
            services.AddTransient<IPoetryDBClient>(services => new PoetryDBClient(new RestClient("https://poetrydb.org")));
            services.AddCors(options =>
            {
                options.AddPolicy(POETRYDB_ORIGIN, builder => builder.WithOrigins("https://poetrydb.org"));
            });
            services.AddDbContextFactory<PoetryContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            using (var context = scope.ServiceProvider
                    .GetRequiredService<IDbContextFactory<PoetryContext>>()
                    .CreateDbContext())
            {
                context.Database.Migrate();
            }
        }
    }
}
