using System;
using Newtonsoft.Json.Linq;

namespace ahaley.AtTask
{
    public interface IPayrollMapper
    {
        Payroll[] MapTimesheetsToPayrollReportItem(JToken timesheets, JToken expenses);
        Payroll MapTimesheetToPayrollReportItem(JToken timesheet, JToken expenses);
    }
}
