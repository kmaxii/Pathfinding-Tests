using System;
using System.Globalization;

namespace Utils
{
    public class DateTimeFormatter
    {
        private readonly DateTimeOffset _dateTime;

        public DateTimeFormatter(string dateTimeString)
        {
            // Parsing the provided date-time string
            _dateTime = DateTimeOffset.Parse(dateTimeString, null, DateTimeStyles.RoundtripKind);
        }

        public int Year => _dateTime.Year;
        public int Month => _dateTime.Month;
        public int Day => _dateTime.Day;
        public int Hour => _dateTime.Hour;
        public int Minute => _dateTime.Minute;
        public int Second => _dateTime.Second;
        public int Millisecond => _dateTime.Millisecond;
        public TimeSpan Offset => _dateTime.Offset;
        public string HourMinute => $"{Hour:D2}:{Minute:D2}";
    }
}