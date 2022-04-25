using System;
using UnityEngine;

namespace RSCards.Utilities // This is a direct copy from DHC
{
    // Token: 0x02000006 RID: 6
    internal static class DateTools
    {
        // Token: 0x06000016 RID: 22 RVA: 0x00002950 File Offset: 0x00000B50
        public static bool DayOf(DateTime holiday)
        {
            double num = DateTime.UtcNow.ToOADate();
            double num2 = holiday.AddHours(-12.0).ToOADate();
            double num3 = holiday.AddHours(36.0).ToOADate();
            return num > num2 && num < num3;
        }

        // Token: 0x06000017 RID: 23 RVA: 0x000029BC File Offset: 0x00000BBC
        public static bool DayOf(DateTime[] holidays)
        {
            for (int i = 0; i < holidays.Length; i++)
            {
                bool flag = DateTools.DayOf(holidays[i]);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06000018 RID: 24 RVA: 0x000029F8 File Offset: 0x00000BF8
        public static bool WeekOf(DateTime holiday)
        {
            double num = DateTime.UtcNow.ToOADate();
            double num2 = holiday.AddDays(-6.0).AddHours(-12.0).ToOADate();
            double num3 = holiday.AddDays(6.0).AddHours(36.0).ToOADate();
            return num > num2 && num < num3;
        }

        // Token: 0x06000019 RID: 25 RVA: 0x00002A88 File Offset: 0x00000C88
        public static bool WeekOf(DateTime[] holidays)
        {
            for (int i = 0; i < holidays.Length; i++)
            {
                bool flag = DateTools.WeekOf(holidays[i]);
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x0600001A RID: 26 RVA: 0x00002AC4 File Offset: 0x00000CC4
        public static void DateTest(DateTime testDate)
        {
            bool flag = DateTools.DayOf(testDate);
            if (flag)
            {
                UnityEngine.Debug.Log(string.Format("It's {0}/{1} somewhere", testDate.Month, testDate.Day));
            }
            else
            {
                bool flag2 = DateTools.WeekOf(testDate);
                if (flag2)
                {
                    UnityEngine.Debug.Log(string.Format("It's the week of {0}/{1} somewhere", testDate.Month, testDate.Day));
                }
                else
                {
                    UnityEngine.Debug.Log(string.Format("It's not {0}/{1} anywhere", testDate.Month, testDate.Day));
                }
            }
        }
    }
}