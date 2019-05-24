using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.DAL;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.Events;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.Services;
using Reti.Lab.FoodOnKontainers.Deliveries.Api.Services.HostedServices;
using Reti.Lab.FoodOnKontainers.Middleware;
using Reti.Lab.FoodOnKontainers.Middleware.Dto;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api
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
            services.AddDbContext<DeliveriesDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("FokDeliveriesDB"), x => x.UseNetTopologySuite());
            });

            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IRiderService, RiderService>();
            services.AddSingleton<IDeliveriesEventManager, DeliveriesEventManager>();
            services.AddHostedService<RiderHandlerHostedService>();

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
        }
    }
}
