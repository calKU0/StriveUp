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
                90009 => 7, // Running
                91070 => 7, // Incline Running
                91069 => 7, // Trail Running
                20049 => 8, // Treadmill Running

                90001 => 10, // Cycling
                1071 => 10, // Outdoor Cycling
                91065 => 10, // Stationary Cycling
                18230 => 12, // Swimming
                18250 => 12, // Swimming
                18260 => 12, // Swimming
                18270 => 12, // Swimming
                18280 => 12, // Swimming
                18290 => 12, // Swimming
                18300 => 12, // Swimming
                18310 => 12, // Swimming
                18320 => 12, // Swimming
                18330 => 12, // Swimming
                18340 => 12, // Swimming
                18350 => 12, // Swimming
                27 => 9, // Walking
                22 => 15, // Weight Lifting
                29 => 13, // Elliptical
                90012 => 11, // Hiking
                _ => 18, // Other
            };
        }

        public static int MapStravaActivityType(string activityType)
        {
            return activityType switch
            {
                "Run" => 7, // Running
                "Ride" => 10, // Cycling
                "Swim" => 12, // Swimming
                "Walk" => 9, // Walking
                "WeightTraining" => 15, // Weight Lifting
                "Elliptical" => 13, // Elliptical
                "Hike" => 11, // Hiking
                _ => 18, // Other
            };
        }
    }
}