using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class SegmentDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public string Name { get; set; } = "";
        public string ShortName { get; set; } = "";
        public int DistanceMeters { get; set; }
    }
}
