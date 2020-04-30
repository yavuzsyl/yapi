using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Text;

namespace Yapi.Contracts.HealthChecks
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
