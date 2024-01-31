using System;
using System.Globalization;

namespace Utils
{
    public static class TimeUtils
    {
        public static string ConvertToRfc3339(Time24H time24H)
        {
            return ConvertToRfc3339(time24H.hour, time24H.minute);
        }

        public static string ConvertToRfc3339(int hour, int minute)
        {
            // Get today's date in UTC
            DateTime today = DateTime.UtcNow;

            // Create a new DateTime object in UTC with today's date and specified hour and minute
            DateTime utcDateTime = new DateTime(today.Year, today.Month, today.Day, hour, minute, 0, DateTimeKind.Utc);

            // Define the Central European Time Zone (CET, UTC+1)
            TimeZoneInfo cetZone = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

            // Convert the DateTime from UTC to CET
            DateTime cetDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, cetZone);

            // Format the DateTime object to RFC-3339 format with manual offset for CET
            string rfc3339String = cetDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff", CultureInfo.InvariantCulture) + "+02:00";

            return rfc3339String;
        }
    }
}