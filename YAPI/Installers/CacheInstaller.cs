using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Cache;
using YAPI.Services;

namespace YAPI.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisSettings);
            services.AddSingleton(redisSettings);

            if (!redisSettings.Enabled)
                return;
            //healthcheck için service containera eklendi
            services.AddSingleton<IConnectionMultiplexer>(_ =>
              ConnectionMultiplexer.Connect(redisSettings.ConnectionString));
            services.AddStackExchangeRedisCache(options => options.Configuration = redisSettings.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
