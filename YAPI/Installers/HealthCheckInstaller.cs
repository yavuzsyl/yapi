using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.HealthChecks;

namespace YAPI.Installers
{
    public class HealthCheckInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis")
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"));
                
        }
    }
}
