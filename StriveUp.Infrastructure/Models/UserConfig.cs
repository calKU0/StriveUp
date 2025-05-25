using StriveUp.Infrastructure.Identity;
using StriveUp.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Models
{
    public class UserConfig
    {
        [Key]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public AppTheme Theme { get; set; } = AppTheme.Light;
        public DefaultMetric DefaultMetric { get; set; } = DefaultMetric.Kilometers;
        public bool PrivateMap { get; set; } = false;
        public bool PrivateActivities { get; set; } = false;
        public string? SensorId { get; set; }
        public string? SensorName { get; set; }
    }
}