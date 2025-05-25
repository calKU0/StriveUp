using StriveUp.Shared.Enums;

namespace StriveUp.Shared.DTOs
{
    public class UserConfigDto
    {
        public AppTheme Theme { get; set; }
        public DefaultMetric DefaultMetric { get; set; }
        public bool PrivateMap { get; set; }
        public bool PrivateActivities { get; set; }
        public string? SensorId { get; set; }
        public string? SensorName { get; set; }
    }
}