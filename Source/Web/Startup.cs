using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DrakeLambert.Peerra.WebApi.SharedKernel.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.IdentityCore;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces;
using DrakeLambert.Peerra.WebApi.IdentityCore.Interfaces.Infrastructure;
using DrakeLambert.Peerra.WebApi.IdentityCore.Services;
using DrakeLambert.Peerra.WebApi.Web.Data;
using DrakeLambert.Peerra.WebApi.Web.Entities;
using DrakeLambert.Peerra.WebApi.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using DrakeLambert.Peerra.WebApi.Web.Controllers;

namespace DrakeLambert.Peerra.WebApi.Web
{
    public class Startup
    {
        /// <summary>
        /// Creates a new instance with the given configuration.
        /// </summary>
        /// <param name="configuration">The configuration for the web host.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The configuration for the web host.
        /// </summary>
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add mvc
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory.Handle;
            });

            // Configure Db Context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(ApplicationDbContext));
                options.EnableSensitiveDataLogging();
            });
            services.AddTransient<SeedData, SeedData>();

            // Configure identity
            var identityBuilder = services.AddIdentityCore<User>(options =>
            {
                // configure identity options
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            });
            identityBuilder = new IdentityBuilder(identityBuilder.UserType, typeof(IdentityRole), identityBuilder.Services);
            identityBuilder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            // Configure client authentication
            var authOptions = Configuration.GetSection(nameof(AuthOptions));
            services.Configure<AuthOptions>(authOptions);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateActor = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authOptions.Get<AuthOptions>().SecurityKey
                };
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
            });

            // Configure Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = nameof(Peerra), Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });
            });

            // Configure Identity Core
            services.AddTransient<IUserTokenService, UserTokenService>();
            services.AddTransient<IIdentityUserPasswordService, IdentityUserPasswordService>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<ITokenFactory, TokenFactory>();
            services.AddTransient<ITokenValidator, TokenValidator>();

            // Configure shared kernel
            services.AddTransient(typeof(IAppLogger<>), typeof(AppLogger<>));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SeedData seed)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHsts();
            app.UseHttpsRedirection();

            var allowedOrigins = Configuration.GetSection("Origins").Get<string[]>();

            app.UseCors(builder => builder.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Peerra Api v1");
            });

            app.UseSwagger();

            app.UseAuthentication();

            app.UseMvc();

            seed.Seed();
        }
    }
}
