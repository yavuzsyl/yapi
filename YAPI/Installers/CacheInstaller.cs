using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddStackExchangeRedisCache(options => options.Configuration = redisSettings.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
