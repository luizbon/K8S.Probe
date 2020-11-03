using System;
using Microsoft.Extensions.DependencyInjection;

namespace K8S.Probe.CommandProbe
{
    public static class CommandHealthProbeExtensions
    {
        public static IServiceCollection AddCommandHealthProbe(
            this IServiceCollection services,
            Action<CommandHealthProbeOptions> setupOptions)
        {
            var options = new CommandHealthProbeOptions();
            setupOptions(options);

            services.AddSingleton(options);

            services.AddHealthChecks();

            services.AddHostedService<CommandHealthProbeService>();

            return services;
        }
    }
}
