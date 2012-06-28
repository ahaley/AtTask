﻿using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ahaley.AtTask
{
    public class PayrollMapper : IPayrollMapper
    {
        public Payroll[] MapTimesheetsToPayrollReportItem(JToken timesheets, JToken expenses = null)
        {
            var payrollItems = new List<Payroll>();
            foreach (JToken j in timesheets.Children()) {
                payrollItems.Add(MapTimesheetToPayrollReportItem(j, expenses));
            }
            return payrollItems.ToArray();
        }

        public Payroll MapTimesheetToPayrollReportItem(JToken timesheet, JToken expenses = null)
        {
            Payroll payrollItem = GetPayrollItem(timesheet);

            payrollItem.RegularHours -= (payrollItem.PaidTimeOff + payrollItem.AbsentHours + payrollItem.HolidayHours);
            payrollItem.OvertimeHours = (payrollItem.TotalHours - payrollItem.AbsentHours - payrollItem.HolidayHours - payrollItem.PaidTimeOff - 40);
            if (payrollItem.OvertimeHours < 0) payrollItem.OvertimeHours = 0;

            ApplyExpenses(expenses, payrollItem);

            return payrollItem;
        }

        private static Payroll GetPayrollItem(JToken timesheet)
        {
            JObject user = timesheet.Value<JObject>("user");
            string employeeID = timesheet.Value<string>("userID");
            DateTime endDate = timesheet.Value<string>("endDate").FromAtTaskDate();

            var payrollItem = new Payroll() {
                Firstname = user.Value<string>("firstName"),
                Lastname = user.Value<string>("lastName"),
                EmployeeID = employeeID,
                NWBHours = timesheet.CountHourType(HourType.NightWorkBonus),
                PaidTimeOff = timesheet.CountHourType(HourType.PaidTimeOff),
                TotalHours = timesheet.Value<double>("totalHours"),
                WeekEnding = endDate,
                RegularHours = timesheet.Value<double>("regularHours"),
                OvertimeHours = timesheet.Value<double>("overtimeHours"),
                AbsentHours = timesheet.CountHourType(HourType.Absent),
                HolidayHours = timesheet.CountHourType(HourType.Holiday)
            };
            return payrollItem;
        }

        private static void ApplyExpenses(JToken expenses, Payroll item)
        {
            if (null == expenses)
                return;

            DateTime endExpenseDate = DateTime.Parse(item.WeekEnding.ToShortDateString());
            DateTime startExpenseDate = endExpenseDate.AddDays(-7);
            IEnumerable<JToken> relevantExpenses = from e in expenses.Children()
                                                   where e.Value<string>("DE:Expense Owner") != null
                                                   && e.Value<string>("DE:Expense Owner").Contains(item.EmployeeID)
                                                   && DateTime.Parse(e.Value<string>("effectiveDate")) > startExpenseDate
                                                   && DateTime.Parse(e.Value<string>("effectiveDate")) <= endExpenseDate
                                                   select e;

            var list = relevantExpenses.ToList();


            Func<string, double> expenseForType = (expenseTypeID) => (from e in relevantExpenses
                                                                      where e.Value<string>("expenseTypeID") == expenseTypeID
                                                                      select e.Value<double>("actualUnitAmount")).Sum();

            item.InChargeDays = expenseForType(ExpenseType.InChargeExpenseType);
            item.TotalMileage = expenseForType(ExpenseType.MileageExpenseType);
            item.TotalPerDiem = expenseForType(ExpenseType.PerDiem);
        }

        

    }
}