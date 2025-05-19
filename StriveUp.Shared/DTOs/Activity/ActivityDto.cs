using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Activity
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MeasurementType { get; set; }
        public bool ElevationRelevant { get; set; }
        public bool Indoor { get; set; }
        public bool SpeedRelevant { get; set; }
        public bool UseHeartRate { get; set; }
    }
}
