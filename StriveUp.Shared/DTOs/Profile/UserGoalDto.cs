using StriveUp.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Profile
{
    public class UserGoalDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public GoalType Type { get; set; }
        public GoalTimeframe Timeframe { get; set; }
        public double AmountCompleted { get; set; }
        public double AmountRemaining { get; set; }
        public double PercentCompleted { get; set; }
        public double TargetValue { get; set; }
    }
}