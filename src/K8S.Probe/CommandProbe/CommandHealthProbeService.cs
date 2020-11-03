using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using K8S.Probe.TcpProbe;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace K8S.Probe.CommandProbe
{
    public class CommandHealthProbeService : BackgroundService
    {
        private HealthCheckService _healthCheckService;
        private ILogger<TcpHealthProbeService> _logger;
        private CommandHealthProbeOptions _options;

        public CommandHealthProbeService(
            HealthCheckService healthCheckService,
            ILogger<TcpHealthProbeService> logger,
            CommandHealthProbeOptions options)
        {
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Tcp Health Probe started");
            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                await HeartbeatAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalInSeconds), stoppingToken);
            }
        }

        private async Task HeartbeatAsync(CancellationToken stoppingToken)
        {
            try
            {
                var result = await _healthCheckService.CheckHealthAsync(stoppingToken);
                var isHealthy = result.Status == HealthStatus.Healthy;

                if (!isHealthy)
                {
                    TryDeleteFile();
                    _logger.LogInformation("Service is {status}. Probe stopped.", result.Status);
                    return;
                }

                TryCreateFile();
                _logger.LogInformation("Successfully processed heartbeat request.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred when processing heartbeat");
            }
        }

        private void TryCreateFile()
        {
            if (File.Exists(_options.Filename))
            {
                File.SetLastWriteTimeUtc(_options.Filename, DateTime.UtcNow);
                return;
            }

            var directory = Path.GetDirectoryName(_options.Filename);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var stream = File.Create(_options.Filename);
        }

        private void TryDeleteFile()
        {
            if (File.Exists(_options.Filename))
            {
                File.Delete(_options.Filename);
            }
        }
    }
}
