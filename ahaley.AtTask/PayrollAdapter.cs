using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ahaley.AtTask
{
    public class PayrollAdapter : IPayrollAdapter
    {
        public PayrollAdapter(IGateway gateway)
        {
            this.gateway = gateway;
        }

        public PayrollAdapter()
            : this(new Gateway())
        {
        }

        readonly IGateway gateway;

        public static readonly string ContractorCategory = "4ea983860010230034f99079db42460f";

        public IGateway Gateway { get { return gateway; } }

        public string[] ApproverList { get; set; }

        public Payroll GetPayrollForEmployee(string userId, DateTime weekEnding)
        {
            var builder = new FilterBuilder();
            builder.AddConstraint("userID", userId);
            builder.AddConstraint("endDate", weekEnding);
            var items = Gateway.GetTimesheetsByFilter(builder);
            if (items == null || items.Length == 0)
                return null;
            return items[0];
        }

        public Payroll[] GetPayroll(DateTime startDate, DateTime stopDate)
        {
            var builder = new FilterBuilder();
            builder.DateRange("endDate", startDate, stopDate);
            return Gateway.GetTimesheetsByFilter(builder);
        }

        public Payroll[] GetPayrollWeekEnding(DateTime weekEnding)
        {
            var payrollBuilder = new PayrollFilterBuilder();
            payrollBuilder.ApplyPayrollFilter(weekEnding);
            payrollBuilder.ApplyPayrollFilterForApproverList(ApproverList);
            return Gateway.GetTimesheetsByFilter(payrollBuilder.Builder);
        }

        public Payroll[] GetPayrollPeriodEnding(DateTime periodEnding)
        {
            var payrollBuilder = new PayrollFilterBuilder();
            payrollBuilder.ApplyPayrollFilter(periodEnding);
            payrollBuilder.ApplyPayrollFilter(periodEnding.AddDays(-7), "OR:a:");
            payrollBuilder.ApplyPayrollFilterForApproverList(ApproverList);
            payrollBuilder.ApplyPayrollFilterForApproverList(ApproverList, "OR:a:");
            return gateway.GetTimesheetsByFilter(payrollBuilder.Builder);
        }

        public Payroll[] GetCombinedPayrollPeriodEnding(DateTime periodEnding)
        {
            var unmergedPayrollItems = GetPayrollPeriodEnding(periodEnding);

            var merged = from u in unmergedPayrollItems.AsQueryable<Payroll>()
                         group u by u.EmployeeID
                             into g
                             select new Payroll() {
                                 EmployeeID = g.Key,
                                 PayrollPeriodID = g.FirstOrDefault().PayrollPeriodID,
                                 Firstname = g.FirstOrDefault().Firstname,
                                 Lastname = g.FirstOrDefault().Lastname,
                                 WeekEnding = g.Max(x => x.WeekEnding),
                                 TotalHours = g.Sum(x => x.TotalHours),
                                 NWBHours = g.Sum(x => x.NWBHours),
                                 InChargeDays = g.Sum(x => x.InChargeDays),
                                 TotalMileage = g.Sum(x => x.TotalMileage),
                                 PaidTimeOff = g.Sum(x => x.PaidTimeOff),
                                 TotalPerDiem = g.Sum(x => x.TotalPerDiem),
                                 RegularHours = g.Sum(x => x.RegularHours),
                                 HolidayHours = g.Sum(x => x.HolidayHours),
                                 OvertimeHours = g.Sum(x => x.OvertimeHours),
                                 AbsentHours = g.Sum(x => x.AbsentHours),
                                 SuspensionHours = g.Sum(x => x.SuspensionHours)
                             };
            return merged.ToArray<Payroll>();
        }
    }
}
