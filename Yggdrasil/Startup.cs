using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Yggdrasil.DAL;
using Yggdrasil.HttpExceptions;
using Yggdrasil.Services.PlayerNotification;
using Yggdrasil.Services.PlayerRecord;

namespace Yggdrasil
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGODB_URI", EnvironmentVariableTarget.User);
            var mongoDbName = Environment.GetEnvironmentVariable("MONGODB_DB_NAME", EnvironmentVariableTarget.User);
            var playerRecordMongoDbCollection = Environment.GetEnvironmentVariable("MONGODB_COLLECTION_NAME_PLAYERRECORD", EnvironmentVariableTarget.User);
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY", EnvironmentVariableTarget.User);

            if (!mongoDbConnectionString.IsNullOrEmpty())
            {
                Environment.SetEnvironmentVariable("MONGODB_URI", mongoDbConnectionString);
            }
            if (!mongoDbName.IsNullOrEmpty())
            {
                Environment.SetEnvironmentVariable("MONGODB_DB_NAME", mongoDbName);
            }
            if (!playerRecordMongoDbCollection.IsNullOrEmpty())
            {
                Environment.SetEnvironmentVariable("MONGODB_COLLECTION_NAME_PLAYERRECORD", playerRecordMongoDbCollection);
            }
            if (!jwtKey.IsNullOrEmpty())
            {
                Environment.SetEnvironmentVariable("JWT_KEY", jwtKey);
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSingleton<IDataAccessLayer, DataAccessLayer>()
                .AddSingleton<IPlayerNotificationService, PlayerNotificationService>()
                .AddSingleton<IPlayerRecordService, PlayerRecordService>();

            services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
