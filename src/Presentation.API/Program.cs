using Akka.Actor;
using Akka.DI.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using RU.Challenge.Infrastructure.Akka.Projection;
using RU.Challenge.Infrastructure.Dapper;
using RU.Challenge.Infrastructure.Identity;
using System;

namespace RU.Challenge.Presentation.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting the host");

                // Init AKKA projections
                {
                    var system = host.Services.GetRequiredService<ActorSystem>();
                    var resolver = host.Services.GetRequiredService<IDependencyResolver>();
                    system.ActorOf(resolver.Create<GenreProjectionActor>());
                    system.ActorOf(resolver.Create<ArtistProjectionActor>());
                    system.ActorOf(resolver.Create<PaymentMethodProjectionActor>());
                    system.ActorOf(resolver.Create<DistributionPlatformProjectionActor>());
                    system.ActorOf(resolver.Create<SubscriptionProjectionActor>());
                    system.ActorOf(resolver.Create<ReleaseProjectionActor>());
                }

                // Init Database
                {
                    using (var scope = host.Services.CreateScope())
                    {
                        var environment = scope.ServiceProvider.GetRequiredService<IHostingEnvironment>();
                        if (!environment.IsProduction())
                        {
                            host.Services.GetRequiredService<ReadDbInitializer>().Init().GetAwaiter().GetResult();
                            host.Services.GetRequiredService<AuthDbInitializer>().Init().GetAwaiter().GetResult();
                        }
                    }
                }

                host.RunAsync().Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while trying to run the host");
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }
}