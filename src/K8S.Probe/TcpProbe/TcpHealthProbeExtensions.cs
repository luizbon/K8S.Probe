using System;
using Microsoft.Extensions.DependencyInjection;

namespace K8S.Probe.TcpProbe
{
    public static class TcpHealthProbeExtensions
    {
        public static IServiceCollection AddTcpHealthProbe(
            this IServiceCollection services,
            Action<TcpHealthProbeOptions> setupOptions)
        {
            var options = new TcpHealthProbeOptions();
            setupOptions(options);

            services.AddSingleton(options);

            services.AddHealthChecks();

            services.AddHostedService<TcpHealthProbeService>();

            return services;
        }
    }
}
