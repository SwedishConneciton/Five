using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Five.Managers;
using Five.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Five.Directives;

namespace Five
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }


	public Startup(IHostingEnvironment env)
	{
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            /// Blogging uses Sqlite
            services
                .AddEntityFrameworkSqlite()
                .AddDbContext<BloggingContext>(
                    options => options.UseSqlite(
                            Configuration["Data:DefaultConnection:ConnectionString"]
                        )
                );
            
            // DI the Blog manager for the controller
            services.AddScoped<IBlogManager, BlogManager>();


            // MVC services (also the Web API in Core)
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Static File Suppport
            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new[] { "index.html" }
            });
            app.UseStaticFiles();


            /// Resource server
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                RequireHttpsMetadata = false,
                /// Whom consumes the JWT token (i.e. this embedded server) but
                /// since nothing the validate token event is just a positive 
                /// passthrough then the audience value could be anything
                Audience = Configuration["OpenId:Audience"],
                /// The embedded OpenId Server 
                Authority = Configuration["OpenId:Authority"]
            });


            // Authority server (open-id connect)
            app.UseOpenIdConnectServer(options =>
            {
                /// Jacks in the Jwt Token handler which 
                /// matches the handler used by the JWT Bearer middleware
                options.UseJwtTokens();

                options.AllowInsecureHttp = true;
                options.ApplicationCanDisplayErrors = true;

                options.TokenEndpointPath = new PathString("/token");
                options.AccessTokenLifetime = TimeSpan.FromDays(1);
                options.AuthorizationCodeLifetime = TimeSpan.FromDays(1);

                options.Provider = new BasicOpenIdConnectServerProvider();
            });


            // MVC
            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var application = new WebHostBuilder()
                .UseConfiguration(config)
                .ConfigureLogging(options => options.AddDebug(minLevel: LogLevel.Trace))
                .ConfigureLogging(options => options.AddConsole(minLevel: LogLevel.Trace))
                .UseKestrel() 
		        .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
