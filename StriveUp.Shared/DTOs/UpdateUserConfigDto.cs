using StriveUp.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class UpdateUserConfigDto
    {
        public AppTheme? Theme { get; set; }
        public DefaultMetric? DefaultMetric { get; set; }
        public bool? PrivateMap { get; set; }
        public bool? PrivateActivities { get; set; }
        public string? SensorId { get; set; }
        public string? SensorName { get; set; }
    }
}