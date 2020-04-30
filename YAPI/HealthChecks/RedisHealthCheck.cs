using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YAPI.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
		private readonly IConnectionMultiplexer connectionMultiplexer;
		public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
		{
			this.connectionMultiplexer = connectionMultiplexer;
		}
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
			try
			{
				var database = connectionMultiplexer.GetDatabase();
				database.StringGet("health");
				return Task.FromResult(HealthCheckResult.Healthy());
			}
			catch (Exception exception)
			{
				return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message));
			}
        }
    }
}
