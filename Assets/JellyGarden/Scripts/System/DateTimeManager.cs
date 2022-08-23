using System;
using UnityEngine;

public static class DateTimeManager
{
    public static bool IsDayPassed(string dateTime)
    {
        var s = PlayerPrefs.GetString(dateTime);
        if (s != "")
        {
            var d = DateTime.Parse(s);
            if (DateTime.Now.Subtract(d).Days >= 1)
            {
                PlayerPrefs.SetInt(dateTime, 0);
                return true;
            }
            return false;
        }

        return true;
    }
    public static bool IsMinutesPassed(string dateTime)
    {
        var s = PlayerPrefs.GetString(dateTime);
        if (s != "")
        {
            var d = DateTime.Parse(s);
            if (DateTime.Now.Subtract(d).Minutes >= 1)
            {
                PlayerPrefs.SetInt(dateTime, 0);
                return true;
            }
            return false;
        }

        return true;
    }

    public static bool IsHourPassed(string dateTime)
    {
        var s = PlayerPrefs.GetString(dateTime);
        if (s != "")
        {
            var d = DateTime.Parse(s);
            if (DateTime.Now.Subtract(d).Hours >= 1)
            {
                PlayerPrefs.SetInt(dateTime, 0);
                return true;
            }
            return false;
        }

        return true;
    }

    public static bool IsPeriodPassed(string dateTime)
    {
        switch (InitScript.Instance.dailyRewardedFrequencyTime)
        {
            case RewardedAdsTime.Day:
                return IsDayPassed(dateTime);
            case RewardedAdsTime.Hour:
                return IsHourPassed(dateTime);
            case RewardedAdsTime.Minute:
                return IsMinutesPassed(dateTime);
        }
        return IsMinutesPassed(dateTime);
    }

    public static void SetDateTimeNow(string dateTime)
    {
        PlayerPrefs.SetString(dateTime, DateTime.Now.ToString());
    }

    public static DateTime GetLastDateTime(string dateTime)
    {
        return DateTime.Parse(PlayerPrefs.GetString(dateTime, DateTime.Now.ToString()));
    }

}

public enum RewardedAdsTime
{
    Day,
    Hour,
    Minute
}