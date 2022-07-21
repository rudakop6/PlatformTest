namespace Platform
{
    public class Constants
    {
        public const int NumberHoursInDay = 24;
        public const int NumberMinutesInHour = 60;
        public const int StartIndexMap = 1;

        public const int FirstDimension = 0;
        public const int SecondDimension = 1;
        public const int ThirdDimension = 2;

        public static int ConvertToInt(int? num)
        {
            return num ?? 0;
        }
    }
}
