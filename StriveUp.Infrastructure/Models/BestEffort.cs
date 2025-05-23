﻿using StriveUp.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;

namespace StriveUp.Infrastructure.Models
{
    public class BestEffort
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public AppUser User { get; set; }

        [Required]
        public int UserActivityId { get; set; }

        public UserActivity UserActivity { get; set; }

        [Required]
        public int SegmentConfigId { get; set; }

        public SegmentConfig SegmentConfig { get; set; }

        [Required]
        public double DurationSeconds { get; set; }

        [Required]
        public DateTime ActivityDate { get; set; }

        public double Speed => SegmentConfig.DistanceMeters / DurationSeconds;
    }
}