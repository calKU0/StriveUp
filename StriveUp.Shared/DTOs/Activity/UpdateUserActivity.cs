namespace StriveUp.Shared.DTOs.Activity
{
    public class UpdateUserActivityDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int ActivityId { get; set; }
        public bool IsPrivate { get; set; }
        public bool ShowHeartRate { get; set; }
        public bool ShowSpeed { get; set; }
        public bool ShowCalories { get; set; }
    }
}