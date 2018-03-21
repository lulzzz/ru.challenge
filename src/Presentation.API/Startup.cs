using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Autofac.Core;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using RU.Challenge.Domain.Entities.Auth;
using RU.Challenge.Infrastructure.Akka.Projection;
using RU.Challenge.Infrastructure.Dapper;
using RU.Challenge.Infrastructure.Dapper.Repositories;
using RU.Challenge.Infrastructure.Identity;
using RU.Challenge.Presentation.API.Autofac;
using RU.Challenge.Presentation.API.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RU.Challenge.Presentation.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment env)
        {
            Environment = env;
            NLog.LogManager.LoadConfiguration($"nlog.{env.EnvironmentName}.config");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .AddOptions()
                .AddSwaggerGen(c =>
                {
                    c.CustomSchemaIds(x => x.FullName);
                    c.SwaggerDoc("v1", new Info { Title = "RU Challenge", Version = "v1" });
                    c.DescribeAllEnumsAsStrings();
                    c.OperationFilter<AddAuthorizationHeaderParameter>();
                });

            services
                .AddIdentity<User, Role>()
                .AddDefaultTokenProviders();

            var tokenValidation = ConfigureJWTToken(services);

            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.ClaimsIssuer = tokenValidation.ValidIssuer;
                    opt.TokenValidationParameters = tokenValidation;
                    opt.SaveToken = true;
                });

            services.AddMvc(options => options.Filters.Add(new ProducesAttribute("application/json")));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            RegisterCQRS(builder);
            RegisterAkka(builder);
            RegisterReadDatabase(builder);
            RegisterAuthDatabase(builder);
        }

        private void RegisterCQRS(ContainerBuilder builder)
        {
            var assemblies =
                DependencyContext
                    .Default
                    .GetDefaultAssemblyNames()
                    .Where(e => e.FullName.Contains("RU.Challenge"))
                    .Select(e => Assembly.Load(e))
                    .ToArray();

            builder.RegisterModule(new MediatorModule(assemblies));
        }

        private void RegisterAkka(ContainerBuilder builder)
        {
            var config = ConfigurationFactory.FromResource<Startup>($"RU.Challenge.Presentation.API.akka.{Environment.EnvironmentName}.conf");

            // System
            builder.Register(e => ActorSystem.Create("ru-challenge-system", config)).SingleInstance();

            // Projection actors
            builder.RegisterType<GenreProjectionActor>();
            builder.RegisterType<ArtistProjectionActor>();
            builder.RegisterType<PaymentMethodProjectionActor>();
            builder.RegisterType<DistributionPlatformProjectionActor>();
            builder.RegisterType<SubscriptionProjectionActor>();

            // Resolver
            builder
                .Register(e => new AutoFacDependencyResolver(e.Resolve<ILifetimeScope>(), e.Resolve<ActorSystem>()))
                .As<IDependencyResolver>();
        }

        private void RegisterAuthDatabase(ContainerBuilder builder)
        {
            builder
                .Register(e => new NpgsqlConnection(Configuration.GetConnectionString("ruauth")))
                .Named<IDbConnection>("ruauth")
                .InstancePerLifetimeScope();

            builder
                .RegisterType<AuthDbInitializer>()
                .WithParameter(ResolvedParameter.ForNamed<IDbConnection>("ruauth"))
                .AsSelf();

            builder
                .RegisterType<UserPasswordStore>()
                .WithParameter(ResolvedParameter.ForNamed<IDbConnection>("ruauth"))
                .As<IUserStore<User>>();

            builder
                .RegisterType<RoleStore>()
                .WithParameter(ResolvedParameter.ForNamed<IDbConnection>("ruauth"))
                .As<IRoleStore<Role>>();
        }

        private void RegisterReadDatabase(ContainerBuilder builder)
        {
            builder
                .Register(e => new NpgsqlConnection(Configuration.GetConnectionString("ruread")))
                .As<IDbConnection>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ReadDbInitializer>()
                .AsSelf();

            builder
                .RegisterType<GenreRepository>()
                .As<IGenreRepository>();

            builder
                .RegisterType<ArtistRepository>()
                .As<IArtistRepository>();

            builder
                .RegisterType<PaymentMethodRepository>()
                .As<IPaymentMethodRepository>();

            builder
                .RegisterType<DistributionPlatformRepository>()
                .As<IDistributionPlatformRepository>();

            builder
                .RegisterType<SubscriptionRepository>()
                .As<ISubscriptionRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app
             .UseAuthentication()
             .UseRewriter(new RewriteOptions().AddRedirect("^$", "/help"))
             .UseMvcWithDefaultRoute()
             .UseSwagger()
             .UseSwaggerUI(c =>
             {
                 c.SwaggerEndpoint("/swagger/v1/swagger.json", "RU Challenge API v1");
                 c.RoutePrefix = "help";
                 c.DefaultModelsExpandDepth(-1);
             });
        }

        private TokenValidationParameters ConfigureJWTToken(IServiceCollection services)
        {
            // Set up JWT
            var issuer = Configuration.GetValue<string>("Jwt:Issuer");
            var audience = Configuration.GetValue<string>("Jwt:Audience");
            var secretKey = Configuration.GetValue<string>("Jwt:SecretKey");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            // This line can be replaced with Autofac or other DI container
            services.AddSingleton(e =>
                new JwtFactory(
                    issuer,
                    audience,
                    new SigningCredentials(
                        key: signingKey,
                        algorithm: SecurityAlgorithms.HmacSha256)));

            return new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = false,
            };
        }
    }
}