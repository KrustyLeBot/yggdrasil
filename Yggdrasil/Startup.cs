using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Yggdrasil.DAL;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Services.PlayerNotification;

namespace Yggdrasil
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
            services.AddControllersWithViews();

            services.AddSingleton<IDataAccessLayer, DataAccessLayer>()
                .AddSingleton<IPlayerNotificationService, PlayerNotificationService>();

            services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                var mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_URI", EnvironmentVariableTarget.User);
                var mongoDbName = Environment.GetEnvironmentVariable("MONGODB_DB_NAME", EnvironmentVariableTarget.User);
                var mongoDbCollection = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME", EnvironmentVariableTarget.User);

                Environment.SetEnvironmentVariable("MONGODB_URI", mongoDbConnectionString);
                Environment.SetEnvironmentVariable("MONGODB_DB_NAME", mongoDbName);
                Environment.SetEnvironmentVariable("MONGODB_COLLECTION_NAME", mongoDbCollection);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
