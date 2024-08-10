namespace CORE_TALAB.Services
{
    public interface IApplicationConfiguration
    {
        string KeyGGMaps { get; set; }
        string MediaServer { get; set; }
        string TimeLoadDashboard { get; set; }
        string HostMaps { get; set; }
        string SignalrServer { get; set; }
    }
}
