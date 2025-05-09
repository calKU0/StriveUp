﻿using StriveUp.Shared.DTOs.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class CreateUserActivityDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int ActivityId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public double DurationSeconds { get; set; }
        public double Distance { get; set; }
        public List<GeoPointDto> Route { get; set; }
        public List<ActivityHrDto> HrData { get; set; } = new();
        public List<ActivitySpeedDto> SpeedData { get; set; } = new();
    }

}
