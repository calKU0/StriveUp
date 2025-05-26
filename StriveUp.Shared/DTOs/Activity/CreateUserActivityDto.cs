using StriveUp.Shared.DTOs.Activity;

namespace StriveUp.Shared.DTOs
{
    public class CreateUserActivityDto
    {
        public string? UserId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int ActivityId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int Distance { get; set; }
        public bool isSynchronized { get; set; } = false;
        public string? SynchroId { get; set; }
        public bool IsManuallyAdded { get; set; } = false;
        public string? SensorName { get; set; }
        public double? AvarageSpeed { get; set; }
        public double? MaxSpeed { get; set; }
        public int? AvarageHr { get; set; }
        public int? MaxHr { get; set; }
        public int? ElevationGain { get; set; }
        public List<GeoPointDto>? Route { get; set; }
        public List<ActivityHrDto>? HrData { get; set; }
        public List<ActivitySpeedDto>? SpeedData { get; set; }
        public List<ActivityElevationDto>? ElevationData { get; set; }
    }
}