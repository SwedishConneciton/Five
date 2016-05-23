using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Five.Managers;
using Five.Models;
using System;
using AspNet.Security.OpenIdConnect.Server;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Five
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }


        public void ConfigureServices(IServiceCollection services)
        {
            /// Note: hosting.json is auto-magically pulled in
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();


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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseIISPlatformHandler();


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

                options.Provider = new OpenIdConnectServerProvider
                {
                    OnValidateTokenRequest = context =>
                    {
                        /// Skip since the resource server is embedded
                        context.Skip();
                        return Task.FromResult(0);
                    },

                    /// Populate the ticket always with the whom
                    /// the request is from (i.e. user) then depending
                    /// on the scopes extra info.  jwt.io can help
                    /// unravel a JWT token.
                    OnGrantResourceOwnerCredentials = context =>
                    {
                        var Options = context
                            .HttpContext
                            .RequestServices
                            .GetRequiredService<IOptions<IdentityOptions>>()
                            .Value;

                        var Scopes = context.Request.GetScopes();

                        var Identity = new ClaimsIdentity(
                            OpenIdConnectServerDefaults.AuthenticationScheme,
                            Options.ClaimsIdentity.UserNameClaimType,
                            Options.ClaimsIdentity.RoleClaimType
                        );

                        Identity.AddClaim(
                            ClaimTypes.Name,
                            "me",
                            OpenIdConnectConstants.Destinations.AccessToken,
                            OpenIdConnectConstants.Destinations.IdentityToken
                        );

                        /// Dummy examples of scope to claim mappings
                        if (Scopes.Contains(OpenIdConnectConstants.Scopes.Profile))
                        {
                            Identity.AddClaim(
                                ClaimTypes.NameIdentifier,
                                "12345678",
                                OpenIdConnectConstants.Destinations.AccessToken,
                                OpenIdConnectConstants.Destinations.IdentityToken
                            );
                        }

                        if (Scopes.Contains(OpenIdConnectConstants.Scopes.Email))
                        {
                            Identity.AddClaim(
                                ClaimTypes.Email,
                                "me",
                                OpenIdConnectConstants.Destinations.AccessToken,
                                OpenIdConnectConstants.Destinations.IdentityToken
                            );
                        }

                        // Create a new authentication ticket holding the user identity.
                        var Ticket = new AuthenticationTicket(
                            new ClaimsPrincipal(Identity),
                            new AuthenticationProperties(),
                            context.Options.AuthenticationScheme
                        );

                        Ticket.SetResources(
                            context.Request.GetResources()
                        );
                        Ticket.SetScopes(
                            Scopes
                        );

                        context.Validate(Ticket);

                        return Task.FromResult(0);
                    }
                };
            });


            // MVC
            app.UseMvc();
        }

        public static void Main(string[] args)
        {
            var application = new WebHostBuilder()
                .ConfigureLogging(options => options.AddDebug(minLevel: LogLevel.Trace))
                .ConfigureLogging(options => options.AddConsole(minLevel: LogLevel.Trace))
                .UseDefaultHostingConfiguration(args)
                .UseIISPlatformHandlerUrl()
                .UseKestrel() // http://tostring.it/2016/01/12/Using-Kestrel-with-ASPNET-5/
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }
    }
}
