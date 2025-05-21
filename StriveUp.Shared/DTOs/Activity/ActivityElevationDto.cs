using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Activity
{
    public class ActivityElevationDto
    {
        public int UserActivityId { get; set; }
        public double ElevationValue { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
