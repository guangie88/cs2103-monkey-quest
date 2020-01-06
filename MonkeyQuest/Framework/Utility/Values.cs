using System;

namespace MonkeyQuest.Framework.Utility
{
    public static class Values
    {
        public const int FinenessLimit = 5000;
        public const int FramesPerSecond = 50;

        public static int CalculateFineness(double proportion, int finenessLimit)
        {
            return (int)(proportion * finenessLimit * 2);
        }
    }
}
