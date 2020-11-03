namespace K8S.Probe.CommandProbe
{
    public class CommandHealthProbeOptions
    {
        public int IntervalInSeconds { get; set; }
        public string Filename { get; set; } = "/tmp/healthy";
    }
}