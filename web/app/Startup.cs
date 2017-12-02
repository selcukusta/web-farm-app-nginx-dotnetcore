using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using app.Models;
using Microsoft.AspNetCore.DataProtection;
using app.Caching;

namespace app
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RedisConfiguration>(Configuration.GetSection("Redis"));
            services.Configure<ApplicationConfiguration>(Configuration.GetSection("Application"));

            var appConfig = new ApplicationConfiguration();
            Configuration.GetSection("Application").Bind(appConfig);
            var redisConfig = new RedisConfiguration();
            Configuration.GetSection("Redis").Bind(redisConfig);

            services.AddSingleton<RedisCacheProvider>(new RedisCacheProvider(redisConfig.Host, redisConfig.DefaultDb));

            var redisProvider = services.BuildServiceProvider().GetService<RedisCacheProvider>();
            var redisConnection = redisProvider.GetConnection();

            /*
             * References:
             *      https://github.com/aspnet/Session/issues/159
             *      https://github.com/aspnet/Docs/issues/3255
             *      http://www.paraesthesia.com/archive/2016/06/15/set-up-asp-net-dataprotection-in-a-farm/
             */
            services.AddDataProtection()
                    .SetApplicationName(appConfig.Name)
                    .PersistKeysToRedis(redisConnection, appConfig.DataProtection);

            services.AddDistributedRedisCache((cache) =>
            {
                cache.InstanceName = appConfig.Name;
                cache.Configuration = redisConfig.Host;
            });

            services.AddOptions();
            services.AddSession();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });
        }
    }
}
