using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace ahaley.AtTask
{
    public static class AtTaskExtentionMethods
    {
        static readonly string AtTaskDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss:fffzz00";
        static readonly string AtTaskDateFormat = "yyyy-MM-dd'T'13:27:29:999";

        public static string ToAtTaskDate(this DateTime dt)
        {
            return dt.ToString(AtTaskDateFormat);
        }

        public static string ToAtTaskDateTime(this DateTime dt)
        {
            return dt.ToString(AtTaskDateTimeFormat);
        }

        public static DateTime FromAtTaskDate(this string s)
        {
            return DateTime.ParseExact(s, AtTaskDateTimeFormat, CultureInfo.InvariantCulture);
        }

        public static double CountHourType(this JToken timesheet, string hourType)
        {
            IEnumerable<double> relevantHours =
            from h in timesheet.Value<JArray>("hours").Children<JObject>()
            where h.Value<string>("hourTypeID") == hourType
            select h.Value<double>("hours");
            return relevantHours.Sum();
        }

    }
}
