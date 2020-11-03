using System.Net;

namespace K8S.Probe.TcpProbe
{
    public class TcpHealthProbeOptions
    {
        public IPAddress IpAddress { get; set; } = IPAddress.Any;
        public int Port { get; set; }
        public int IntervalInSeconds { get; set; }
    }
}
