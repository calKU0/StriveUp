using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Sync.Application.Helpers
{
    public static class ActivityHelpers
    {
        public static int MapActivityType(int activityType)
        {
            return activityType switch
            {
                82 => 6,
                83 => 6,
                84 => 6,
                8 => 4,
                1 => 5,
                _ => 7
            };
        }
    }
}
