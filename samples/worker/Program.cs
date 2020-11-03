using K8S.Probe;
using K8S.Probe.CommandProbe;
using K8S.Probe.TcpProbe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    services.AddHostedService<Worker>();
                    services.AddHealthChecks().AddCheck<HealthCheck>("health_check");
                    services.AddTcpHealthProbe(options =>
                    {
                        options.IntervalInSeconds = 1;
                        options.Port = configuration.GetValue<int>("PROBE_PORT");
                    });
                    services.AddCommandHealthProbe(options =>
                    {
                        options.IntervalInSeconds = 1;
                    });
                });
    }
}
