using System;

namespace SpotifyLibraryManager.Helpers
{
    public static class DateTimeCreator
    {
        public static DateTime CreateFromString(string input)
        {
            if (DateTime.TryParse(input, out DateTime dateTime))
            {
                return dateTime;
            }
            else
            {
                return new DateTime(Convert.ToInt32(input), 1, 1);
            }
        }
    }
}
