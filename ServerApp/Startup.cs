using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ServerApp.Models;

namespace ServerApp
{
    public class Startup
    {

        private readonly IWebHostEnvironment env;
        public Startup(IWebHostEnvironment env)
        {
            this.env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(System.Environment.GetCommandLineArgs()
                    .Skip(1).ToArray());

            Configuration = builder.Build();
                
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"Development => {env.IsDevelopment()}");

            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

            services.AddDbContext<IdentityDataContext>(options => 
                options.UseSqlServer(Configuration["ConnectionStrings:Identity"]));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityDataContext>();

            services.AddControllersWithViews()
                    .AddJsonOptions(opts =>
                    {
                        opts.JsonSerializerOptions.IgnoreNullValues = true;
                    })
                    .AddNewtonsoftJson();
            //.AddNewtonsoftJson(options => {
            //    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //});

            if (env.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1",
                         new OpenApiInfo { Title = "SportsStore API", Version = "v1" });
                });
            }

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "SessionData";
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = "SportsStore.Session";
                options.IdleTimeout = System.TimeSpan.FromHours(48);
                options.Cookie.HttpOnly = false;
                options.Cookie.IsEssential = true;
            });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,  
                              IServiceProvider services, IAntiforgery antiforgery,
                              IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();

            app.Use(nextDelegate => context =>
            {
                string path = context.Request.Path.Value;
                string[] directUrls = { "/admin", "/store", "/cart", "checkout" };
                if(path.StartsWith("/api") || string.Equals("/", path)
                   || directUrls.Any(url => path.StartsWith(url)))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN",
                        tokens.RequestToken, new CookieOptions()
                        {
                            HttpOnly = false,
                            Secure = false,
                            IsEssential = true
                        });
                }
                return nextDelegate(context);
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "angular_fallback",
                    pattern: "{target:regex(admin|store|cart|checkout):nonfile}/{*catchall}",
                    defaults: new { controller = "Home", action = "Index"}
                    );
            });

            


            if (env.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SportsStore API");
                });

                app.UseSpa(spa =>
                {
                    string strategy = Configuration
                         .GetValue<String>("DevTools:ConnectionStrategy");

                    if (strategy == "proxy")
                    {
                        spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4200");
                    }
                    else if (strategy == "managed")
                    {
                        spa.Options.SourcePath = "../ClientApp";
                        spa.UseAngularCliServer("start");
                    }
                });
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    RequestPath = "",
                    FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),
                    "./wwwroot/app"))
                });
            }
            


            if ((Configuration["INITDB"] ?? "false") == "true")
            {
                System.Console.WriteLine("Preparing Database...");
                SeedData.SeedDatabase(services.GetRequiredService<DataContext>());
                IdentitySeedData.SeedDatabase(services).Wait();
                System.Console.WriteLine("Database Preparation Complete");
                lifetime.StopApplication();

            }
        }
    }
}
