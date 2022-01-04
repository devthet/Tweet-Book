using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tweet_Book.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _connectionMultiplaexer;

        public RedisHealthCheck(IConnectionMultiplexer connectionMultiplaexer)
        {
            _connectionMultiplaexer = connectionMultiplaexer;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var database = _connectionMultiplaexer.GetDatabase();
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
