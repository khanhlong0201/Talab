namespace CORE_TALAB.Services
{
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        public string KeyGGMaps { get; set; }
        public string MediaServer { get; set; }
        public string TimeLoadDashboard { get; set; }
        public string HostMaps { get; set; }
        public string SignalrServer { get; set; }
    }
}
