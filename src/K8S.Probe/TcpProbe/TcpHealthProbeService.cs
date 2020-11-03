using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace K8S.Probe.TcpProbe
{
    public class TcpHealthProbeService: BackgroundService
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<TcpHealthProbeService> _logger;
        private readonly TcpHealthProbeOptions _options;
        private readonly TcpListener _listener;

        public TcpHealthProbeService(
            HealthCheckService healthCheckService,
            ILogger<TcpHealthProbeService> logger,
            TcpHealthProbeOptions options)
        {
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
            _logger = logger;
            _options = options;
            _listener = new TcpListener(options.IpAddress, options.Port);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Tcp Health Probe started");
            await Task.Yield();
            _listener.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                await HeartbeatAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalInSeconds), stoppingToken);
            }
            _listener.Stop();
        }

        private async Task HeartbeatAsync(CancellationToken stoppingToken)
        {
            try
            {
                var result = await _healthCheckService.CheckHealthAsync(stoppingToken);
                var isHealthy = result.Status == HealthStatus.Healthy;

                if (!isHealthy)
                {
                    _listener.Stop();
                    _logger.LogInformation("Service is {status}. Probe stopped.", result.Status);
                    return;
                }

                _listener.Start();
                while (_listener.Server.IsBound && _listener.Pending())
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    client.Close();
                    _logger.LogInformation("Successfully processed heartbeat request.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred when processing heartbeat");
            }
        }
    }
}
