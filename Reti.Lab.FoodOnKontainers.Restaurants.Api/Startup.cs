﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reti.Lab.FoodOnKontainers.Middleware;
using Reti.Lab.FoodOnKontainers.Middleware.Dto;
using Reti.Lab.FoodOnKontainers.Restaurants.Api.DAL;
using Reti.Lab.FoodOnKontainers.Restaurants.Api.Events;
using Reti.Lab.FoodOnKontainers.Restaurants.Api.Services;

namespace Reti.Lab.FoodOnKontainers.Restaurants.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RestaurantsDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("FokRestaurantsDB"));
            });

            //registering restaurants service
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IRestaurantMenuService, RestaurantMenuService>();
            //registering bus event manager
            services.AddSingleton<IRestaurantsEventsManager, RestaurantsEventsManager>();

            LogMiddleware.AddRabbitMQConfiguration(services, Configuration);

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (app.ApplicationServices.GetRequiredService<RabbitMQConfigurations>().Enabled)
            {
                app.UseMiddleware<LogMiddleware>();
            }
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });
            app.UseMvc();

            //create events manager instance
            app.ApplicationServices.GetService<IRestaurantsEventsManager>();

            //centralized exception handler middleware
            /*
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async httpContext =>
                {
                    httpContext.Response.StatusCode = 500;


                });

            });
            */
        }
    }
}
