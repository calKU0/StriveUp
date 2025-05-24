namespace StriveUp.Sync.Application.Helpers
{
    public static class ActivityHelpers
    {
        public static int MapGoogleActivityType(int activityType)
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

        public static int MapFitbitActivityType(int activityType)
        {
            return activityType switch
            {
                90009 => 4, // Running
                91070 => 4, // Incline Running
                91069 => 4, // Trail Running
                20049 => 4, // Treadmill Running

                90001 => 5, // Cycling
                1071 => 5, // Outdoor Cycling
                91065 => 5, // Stationary Cycling
                18230 => 6, // Swimming
                18250 => 6, // Swimming
                18260 => 6, // Swimming
                18270 => 6, // Swimming
                18280 => 6, // Swimming
                18290 => 6, // Swimming
                18300 => 6, // Swimming
                18310 => 6, // Swimming
                18320 => 6, // Swimming
                18330 => 6, // Swimming
                18340 => 6, // Swimming
                18350 => 6, // Swimming
                _ => 7, // Swimming
            };
        }
    }
}