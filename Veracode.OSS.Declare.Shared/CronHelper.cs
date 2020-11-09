using Cronos;
using System;
using System.Collections.Generic;
using System.Text;
using Veracode.OSS.Wrapper.Rest;

namespace Veracode.OSS.Declare.Shared
{
    public static class CronHelper
    {
        public static bool IsANewScanDue(string cronString, DateTime last_scan_time)
        {
            CronExpression expression = CronExpression.Parse(cronString);
            var utcTime = last_scan_time.ToUniversalTime();
            DateTime? nextUtc = expression.GetNextOccurrence(utcTime);
            return nextUtc <= DateTime.Now;
        }

        public static double HowManyDaysAgo(string cronString, DateTime last_scan_time)
        {
            CronExpression expression = CronExpression.Parse(cronString);
            var utcTime = last_scan_time.ToUniversalTime();
            DateTime? nextUtc = expression.GetNextOccurrence(utcTime);
            return (DateTime.Now - nextUtc).Value.TotalDays;
        }
    }
}
