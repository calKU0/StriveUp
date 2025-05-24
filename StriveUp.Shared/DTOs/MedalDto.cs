namespace StriveUp.Shared.DTOs
{
    public class MedalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Level { get; set; }
        public int Points { get; set; }
        public int TargetValue { get; set; }
        public int DistanceToEarn { get; set; }
        public int ProgressPercent { get; set; }
        public string Frequency { get; set; }
        public DateTime? DateEarned { get; set; }
        public int ActivityId { get; set; }
        public int TimesClaimed { get; set; }
    }
}