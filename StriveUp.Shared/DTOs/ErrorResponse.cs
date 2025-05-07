using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.DTOs
{
    public class ErrorResponse
    {
        public string? Message { get; set; }
        public string? Details { get; set; }
        public List<string>? Errors { get; set; }
    }
}
