using System.Linq;
using Irrelephant.DnB.Server.Authentication.Extensions;
using Irrelephant.DnB.Server.Authentication.Options;
using Irrelephant.DnB.Server.Hubs;
using Irrelephant.DnB.Server.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Irrelephant.DnB.Server
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
            services.AddSignalR(cfg => {
                cfg.EnableDetailedErrors = true;
            });
            services
                .AddOptions()
                .Configure<GoogleApiCredentials>(Configuration.GetSection("Auth:GoogleCredentials").Bind);
            services.AddConfiguredAuthentication(Configuration);
            services.AddScoped(container => container);
            services.AddSingleton<ICombatRepository, MemoryCombatRepository>();
            services.AddResponseCompression(options =>
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" })
            );
            services.AddCors(options => options.AddPolicy("DefaultCorsPolicy", builder => {
                builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(Configuration.GetValue<string>("Cors:FrontEndOrigin"));
            }));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseResponseCompression();
            app.UseCors("DefaultCorsPolicy");
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<CombatHub>("/combat");
            });
        }
    }
}
