using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace worker
{
    public class HealthCheck: IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public HealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(new HealthCheckResult((HealthStatus) _configuration.GetValue<int>("HEALTH_CHECK")));
        }
    }
}
