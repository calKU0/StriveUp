using StriveUp.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Profile
{
    public class CreateUserGoalDto
    {
        [Required(ErrorMessage = "Activity type is required")]
        public int ActivityId { get; set; }

        public GoalType Type { get; set; }
        public GoalTimeframe Timeframe { get; set; }

        [Required(ErrorMessage = "Target value is required")]
        [Range(1, 100000, ErrorMessage = "Target value must be in range 1 - 100000")]
        public double TargetValue { get; set; }
    }
}