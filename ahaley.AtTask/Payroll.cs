using System;
using Newtonsoft.Json.Linq;

namespace ahaley.AtTask
{
    public class Payroll
    {
        public int PayrollPeriodID { get; set; }

        public string PayrollReportItemID { get; set; }

        public string EmployeeID { get; set; }

        public string Lastname { get; set; }

        public string Firstname { get; set; }

        public DateTime WeekEnding { get; set; }

        public double TotalHours { get; set; }

        public double RegularHours { get; set; }

        public double OvertimeHours { get; set; }

        public double NWBHours { get; set; }

        public double PaidTimeOff { get; set; }

        public double HolidayHours { get; set; }

        public double SuspensionHours { get; set; }

        public double AbsentHours { get; set; }

        public double InChargeDays { get; set; }

        public double TotalMileage { get; set; }

        public double TotalPerDiem { get; set; }
    }
}
