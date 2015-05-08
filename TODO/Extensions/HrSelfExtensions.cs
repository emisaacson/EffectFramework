using HrSelf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HRMS.Core.Extensions
{
    public static class HrSelfExtensions
    {
        public static DataTable ToDataTable<T>(this List<T> items, string tableName = null)
        {
            var tb = new DataTable(tableName ?? typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                tb.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        // sql is set to start week on Sunday, and week 1 is defined by Jan 1
        // cf iso_week 1 is defined by first 4 day week, depending on which day the week starts on
        public static int WeekOfYear(this DateTime date, DayOfWeek firstWeekDay = DayOfWeek.Sunday, CalendarWeekRule weekRule = CalendarWeekRule.FirstDay)
        {
            Calendar calendar = System.Threading.Thread.CurrentThread.CurrentCulture.Calendar;
            int currentWeek = calendar.GetWeekOfYear(date, weekRule, firstWeekDay);
            return currentWeek;
        }

        public static DateTime WeekStart(this DateTime date, DayOfWeek startDay = DayOfWeek.Sunday)
        {
            DateTime result = date;
            for (var i = 0; i < 7; i++)
            {
                result = date.AddDays(i * -1);
                if (result.DayOfWeek == startDay)
                {
                    return result;
                }
            }
            return result;
        }

        public static DateTime WeekEnd(this DateTime date, DayOfWeek endDay = DayOfWeek.Saturday)
        {
            DateTime result = date;
            for (var i = 0; i < 7; i++)
            {
                result = date.AddDays(i);
                if (result.DayOfWeek == endDay)
                {
                    return result;
                }
            }
            return result;
        }

        // this is the actual logged in user (not impersonated user, if any)
        public static string GetCurrentIdentityName()
        {
            var username = HttpContext.Current.User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                username = username.ToLower();
            }
            return username;
        }

        public static usp_PersonResult GetCurrentPerson()
        {
            var ud = Helper.UserDetails;
            return ud.me;
        }

        public static string CurrentUserDomain
        {
            get
            {
                return Helper.UserDetails.ADDomain;
            }
        }

        public static string CurrentUserName
        {
            get
            {
                return Helper.UserDetails.ADUserId;
            }
        }

        public static string CurrentDatabase
        {
            get
            {
                var ud = Helper.UserDetails;
                if (!string.IsNullOrEmpty(ud.EntityDb))
                {
                    return ud.EntityDb;
                }
                //$BUGBUG: do we want to default to this?
                return Helper.GetDatabaseName(Helper.CurrentConnectionString);
            }
        }

        public static int UserId
        {
            get
            {
                int? id = Helper.UserDetails.EmpGeneralId;
                if (!id.HasValue)
                {
                    // most likely to happen if impersonating a bad user and going directly to a page
                    // no go, redirect to default
                    HttpContext.Current.Response.Redirect("~/Default.aspx", false);
                    HttpContext.Current.Response.End();
                    return 0;
                }
                return id.Value;
            }
        }

        public static int UserIdNoRedirect
        {
            get
            {
                int? id = Helper.UserDetails.EmpGeneralId;
                if (!id.HasValue)
                {
                    return 0;
                }
                return id.Value;
            }
        }

        public static List<Setting> GetSettings(ref TimeKeeperDataClassesDataContext tk)
        {
            var db = Common.CurrentDatabase;
            if (tk == null)
            {
                tk = Helper.GetContextTimeKeeper();
            }
            var result = tk.Settings.Where(x => x.EntityDb == db && x.IsDeleted == false).ToList();
            return result;
        }

        public static string GetStringSetting(ref TimeKeeperDataClassesDataContext tk, string name, string defaultIfNullOrEmpty = null, List<Setting> settings = null)
        {
            HrSelf.Setting result = null;
            string setting = null;

            if (settings != null)
            {
                result = settings.Where(x => x.Name == name).FirstOrDefault();
            }
            else
            {
                var db = HrSelf.Common.CurrentDatabase;
                if (tk == null)
                {
                    tk = Helper.GetContextTimeKeeper();
                }
                result = tk.Settings.Where(x => x.EntityDb == db && x.IsDeleted == false && x.Name == name).FirstOrDefault();
            }
            if (result != null)
            {
                setting = result.Value;
            }
            if (string.IsNullOrEmpty(setting))
            {
                setting = defaultIfNullOrEmpty;
            }
            return setting;
        }

        public static bool GetBoolSetting(ref TimeKeeperDataClassesDataContext tk, string name, List<Setting> settings = null)
        {
            bool setting = false;
            string value = GetStringSetting(ref tk, name, null, settings);

            if (!string.IsNullOrEmpty(value) && value.ToLower() == "true")
            {
                setting = true;
            }
            return setting;
        }

        public static int GetIntSetting(ref TimeKeeperDataClassesDataContext tk, string name, List<Setting> settings = null)
        {
            int setting = 0;
            string value = GetStringSetting(ref tk, name, null, settings);

            if (!string.IsNullOrEmpty(value))
            {
                setting = Convert.ToInt32(value);
            }
            return setting;
        }

        public static DateTime? GetDateTimeSetting(ref TimeKeeperDataClassesDataContext tk, string name, List<Setting> settings = null)
        {
            DateTime? dt = null;
            string value = GetStringSetting(ref tk, name, null, settings);

            if (!string.IsNullOrEmpty(value))
            {
                dt = DateTime.Parse(value);
            }
            return dt;
        }

        // caller must be certain that there is a default for the value, or pass a real global default
        public static DateTime GetDateTimeSettingNotNull(ref TimeKeeperDataClassesDataContext tk, string name
            , List<HrSelf.Setting> settings = null, string defaultIfNullOrEmpty = null)
        {
            DateTime? dt = null;
            string value = GetStringSetting(ref tk, name, null, settings);

            if (string.IsNullOrEmpty(value))
            {
                value = defaultIfNullOrEmpty;
            }

            if (!string.IsNullOrEmpty(value))
            {
                dt = DateTime.Parse(value);
            }
            // yes this will cause exception if null comes back
            return dt.Value;
        }

    }
}