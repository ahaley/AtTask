using System;

namespace ahaley.AtTask
{
    public interface IPayrollAdapter
    {
        string[] ApproverList { get; set; }
        Payroll GetPayrollForEmployee(string userId, DateTime weekEnding);
        Payroll[] GetPayroll(DateTime startDate, DateTime stopDate);
        Payroll[] GetPayrollWeekEnding(DateTime weekEnding);
        Payroll[] GetPayrollPeriodEnding(DateTime periodEnding);
        Payroll[] GetCombinedPayrollPeriodEnding(DateTime periodEnding);
    }
}
