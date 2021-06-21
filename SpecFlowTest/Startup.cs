using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpecFlowTest.Controllers;
using SpecFlowTest.Filters;
using SpecFlowTest.Middleware;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace SpecFlowTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
             // Added for functional tests
             .AddApplicationPart(typeof(WeatherForecastController).Assembly)
             .AddNewtonsoftJson();

            ConfigureAuthService(services);

            //For functional tests purpose we do have this so we can override it in test scenarious
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }


            app.UseRouting();

            ConfigureAuth(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();
            app.UseAuthorization();
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = Configuration.GetValue<string>("IdentityUrlExternal");
                options.Audience = "weather_api";
                options.RequireHttpsMetadata = false;
            });
        }
    }
}
