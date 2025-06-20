﻿using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StriveUp.Infrastructure.Models
{
    public class UserGoal
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public int ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public Activity Activity { get; set; }

        public GoalType Type { get; set; }
        public GoalTimeframe Timeframe { get; set; }
        public double TargetValue { get; set; }
    }
}