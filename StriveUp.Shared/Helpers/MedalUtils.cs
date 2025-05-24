namespace StriveUp.Shared.Helpers
{
    public static class MedalUtils
    {
        public static string GetProgressGradientClass(int progress)
        {
            if (progress >= 100)
                return "progress-100";
            else if (progress >= 75)
                return "progress-75";
            else if (progress >= 50)
                return "progress-50";
            else if (progress >= 25)
                return "progress-25";
            else
                return "progress-0";
        }
    }
}