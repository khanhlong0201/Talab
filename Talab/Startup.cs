using CORE_TALAB.Data;
using CORE_TALAB.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Talab
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            GlobalValues.FortmatMinute = Configuration.GetSection("Global:FormatDefault:DateTime_HH:mm_dd/MM/yyyy").Value;
            GlobalValues.FortmatDate = Configuration.GetSection("Global:FormatDefault:DateTime_dd/MM/yyyy").Value;
            GlobalValues.LinkNewsWebTTDVKH = Configuration.GetSection("Global:LinkNewsWebTTDVKH").Value;
            GlobalValues.HostImageMobile = Configuration.GetSection("Global:HostMobile").Value;
            GlobalValues.Media = Configuration.GetSection("Global:Media").Value;

            GlobalValues.ServerIdFireBase = Configuration.GetSection("Global:Firebare:ServerKey").Value;
            GlobalValues.SenderIdFireBase = String.IsNullOrWhiteSpace(Configuration.GetSection("Global:Firebare:SecretKey").Value) ? null : Configuration.GetSection("Global:Firebare:SecretKey").Value;

            //GlobalValues.SignalrPushNotification = Configuration.GetSection("Global:SignalrPushNotification").Value;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IgnoreNullValues = false;
                // Example: Change property naming policy to camelCase
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

                // Example: Ignore null values when serializing
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            });

            ConfigurationDatabaseSettings(services);

            services.AddMemoryCache();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseRouting();

            // Không cần gọi app.UseAuthentication() và app.UseAuthorization() nếu không sử dụng xác thực

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/v1/{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void ConfigurationDatabaseSettings(IServiceCollection services)
        {
            var ConnectionString = Configuration.GetConnectionString("TALAB");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(ConnectionString));
        }
    }
}