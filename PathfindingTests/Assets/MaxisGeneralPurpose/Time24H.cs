using System;
using UnityEngine;
using Utils;

[Serializable]
public class Time24H : IComparable<Time24H>
{
    public int hour;
    public int minute;

    public Time24H(int hour, int minute)
    {
        this.hour = Mathf.Clamp(hour, 0, 23);
        this.minute = Mathf.Clamp(minute, 0, 59);
    }

    public Time24H(DateTimeFormatter timeFormatter)
        : this(timeFormatter.Hour, timeFormatter.Minute)
    {
    }

    public Time24H(string time)
        : this(new DateTimeFormatter(time))
    {
    }

    public override string ToString()
    {
        return $"{hour:00}:{minute:00}";
    }

  

    public string Rfc3339 => TimeUtils.ConvertToRfc3339(this);


    public static Time24H operator +(Time24H a, Time24H b)
    {
        int totalMinutes = a.minute + b.minute;
        int totalHours = a.hour + b.hour + totalMinutes / 60;

        // Adjust for 24-hour format
        totalHours %= 24;
        totalMinutes %= 60;

        return new Time24H(totalHours, totalMinutes);
    }

    public static bool WillHourResetToZero(Time24H a, Time24H b)
    {
        int totalMinutes = a.minute + b.minute;
        int totalHours = a.hour + b.hour + totalMinutes / 60;

        return totalHours % 24 == 0;
    }
    public static bool operator >(Time24H a, Time24H b)
    {
        if (a.hour > b.hour)
        {
            return true;
        }

   
        return a.minute > b.minute;

    }

    public static bool operator <(Time24H a, Time24H b)
    {
        if (a.hour < b.hour)
        {
            return true;
        }
        
        return a.minute < b.minute;
       
    }

    public int CompareTo(Time24H other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        int hourComparison = this.hour.CompareTo(other.hour);
        if (hourComparison != 0) return hourComparison;

        return this.minute.CompareTo(other.minute);
    }
}