﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class MedalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Level { get; set; }
        public double TargetValue { get; set; }
        public string Frequency { get; set; }
        public DateTime? DateEarned { get; set; }
        public int ActivityId { get; set; }
    }
}
