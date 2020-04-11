using System;
using System.Collections.Generic;
using System.Globalization;
using MonkeyCache;
using MonkeyCache.FileStore;
using TurnipTracker.Model;
using Xamarin.Essentials;

namespace TurnipTracker.Services
{
    public static class DataService
    {
        static IBarrel barrel;
        static object locker = new object();
        static CalendarWeekRule myCWR;
        static DayOfWeek myFirstDOW;
        static CultureInfo myCI;
        static DataService()
        {
            Barrel.ApplicationId = AppInfo.PackageName;
            barrel = Barrel.Create(FileSystem.AppDataDirectory);
            myCI = new CultureInfo("en-US");
            myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
        }

        public static Profile GetProfile()
        {
            lock (locker)
            {
                var profile = barrel.Get<Profile>("profile");
                return profile ?? new Profile();
            }
        }

        public static void SaveProfile(Profile profile)
        {
            lock(locker)
            {
                barrel.Add<Profile>("profile", profile, TimeSpan.FromDays(1));
            }
        }

        static int GetWeekOfYear() =>
            myCI.Calendar.GetWeekOfYear(DateTime.Now, myCWR, myFirstDOW);

        static string GetCurrentWeekKey() =>
            $"week_{GetWeekOfYear()}_{DateTime.Now.Year}";


        public static List<Day> GetCurrentWeek()
        {
            lock (locker)
            {
                var days = barrel.Get<List<Day>>(GetCurrentWeekKey());

                return days ?? new List<Day>
                {
                    new Day { DayLong = "Sunday", IsSunday = true},
                    new Day { DayLong = "Monday" },
                    new Day { DayLong = "Tuesday" },
                    new Day { DayLong = "Wednesday" },
                    new Day { DayLong = "Thursday" },
                    new Day { DayLong = "Friday" },
                    new Day { DayLong = "Saturday" }
                };
            }
        }

        public static void SaveCurrentWeek(List<Day> days)
        {
            lock(locker)
            {
                barrel.Add<List<Day>>(GetCurrentWeekKey(), days, TimeSpan.FromDays(7));
            }
        }


    }
}
