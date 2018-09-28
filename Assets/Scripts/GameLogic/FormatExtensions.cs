namespace GameLogic
{
    public static class FormatExtensions
    {
        public static string FormatTime(this int time)
        {
            if (time < 60)
            {
                return time.ToString("0");
            }

            if (time > 60 && time < 3600)
            {
                return (time / 60).ToString("00") + ":" + (time % 60).ToString("00");
            }

            return (time / 3600).ToString("00") + ":" + (time % 3600).ToString("00") + ":" + (time % 60).ToString("00") ;
        }
    }
}