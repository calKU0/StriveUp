using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs.Activity
{
    public class ActivitySplitDto
    {
        public int SplitNumber { get; set; }
        public double DistanceMeters { get; set; }
        public double? AvgSpeed { get; set; }
        public int? AvgHr { get; set; }
        public int? ElevationGain { get; set; }
    }
}
