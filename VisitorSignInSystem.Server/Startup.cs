using System;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using VisitorSignInSystem.Server.Authentication;
using VisitorSignInSystem.Server.Hubs;
using VisitorSignInSystem.Server.Models;


namespace VisitorSignInSystem.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        //readonly string allowSpecificOrigins = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(o =>
            //{
            //    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(o =>
            //{
            //    o.SecurityTokenValidators.Clear();
            //    o.SecurityTokenValidators.Add(new NameTokenValidator());
            //});

            //services.AddAuthorization(o =>
            //{
            //    o.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            //        .RequireClaim(ClaimTypes.Name)
            //        .Build();
            //});

            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            //var connectionString = Configuration["ConnectionStrings:VsisdataAlias"];
            var connectionString = Configuration["ConnectionStrings:VsisdataProd"];
            var serverVersion = new MySqlServerVersion(new Version(5, 7, 35));
            services.AddDbContextPool<vsisdataContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion)
                    //.EnableSensitiveDataLogging() // These two calls are optional but help
                    .EnableDetailedErrors()      // with debugging (remove for production)
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<vsisHub>("/vsisHub");
            });
        }
    }
}
