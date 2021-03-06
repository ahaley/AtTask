﻿using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ahaley.AtTask
{
    public class PayrollMapper : IPayrollMapper
    {
        const int BaseWeekHour = 40;

        public Payroll[] MapAggregateJsonToPayroll(JToken aggregatePayrollJson, JToken expensesJson = null)
        {
            var payrollItems = new List<Payroll>();
            foreach (JToken j in aggregatePayrollJson.Children()) {
                payrollItems.Add(MapJsonToPayroll(j, expensesJson));
            }
            return payrollItems.ToArray();
        }

        static double CalculateRegular(Payroll p)
        {
            return Math.Min(BaseWeekHour, p.TotalHours - p.PaidTimeOff - p.AbsentHours - p.OvertimeHours - p.SuspensionHours);
        }

        static double CalculateOvertime(Payroll p)
        {
            return Math.Max(0, p.TotalHours - BaseWeekHour - p.AbsentHours - p.SuspensionHours);
        }

        public Payroll MapJsonToPayroll(JToken payrollJson, JToken expenses = null)
        {
            Payroll payrollItem = GetPayrollItem(payrollJson);

            payrollItem.RegularHours = CalculateRegular(payrollItem);
            payrollItem.OvertimeHours = CalculateOvertime(payrollItem);

            ApplyExpenses(expenses, payrollItem);

            DateTime weekEndingUtc = payrollItem.WeekEnding;

            payrollItem.WeekEnding = weekEndingUtc.Subtract(TimeZone.CurrentTimeZone.GetUtcOffset(weekEndingUtc));

            return payrollItem;
        }

        static Payroll GetPayrollItem(JToken payrollJson)
        {
            JObject user = payrollJson.Value<JObject>("user");
            string employeeID = payrollJson.Value<string>("userID");
            DateTime endDate = payrollJson.Value<string>("endDate").FromAtTaskDate();

            var payroll = new Payroll() {
                Firstname = user.Value<string>("firstName"),
                Lastname = user.Value<string>("lastName"),
                EmployeeID = employeeID,
                NWBHours = payrollJson.CountHourType(HourType.NightWorkBonus),
                PaidTimeOff = payrollJson.CountHourType(HourType.PaidTimeOff),
                TotalHours = payrollJson.Value<double>("totalHours"),
                WeekEnding = endDate,
                RegularHours = payrollJson.Value<double>("regularHours"),
                OvertimeHours = payrollJson.Value<double>("overtimeHours"),
                AbsentHours = payrollJson.CountHourType(HourType.Absent),
                HolidayHours = payrollJson.CountHourType(HourType.Holiday),
                SuspensionHours = payrollJson.CountHourType(HourType.Suspension)
            };
            return payroll;
        }

        static void ApplyExpenses(JToken expensesJson, Payroll payroll)
        {
            if (null == expensesJson)
                return;

            List<JToken> relevantExpenses = ExtractRelevantExpenses(expensesJson, payroll);

            Func<string, double> expenseForType = (expenseTypeID) => (from e in relevantExpenses
                                                                      where e.Value<string>("expenseTypeID") == expenseTypeID
                                                                      select e.Value<double>("actualUnitAmount")).Sum();

            payroll.InChargeDays = expenseForType(ExpenseType.InChargeExpenseType);
            payroll.TotalMileage = expenseForType(ExpenseType.MileageExpenseType);
            payroll.TotalPerDiem = expenseForType(ExpenseType.PerDiem);
        }

        static List<JToken> ExtractRelevantExpenses(JToken expensesJson, Payroll payroll)
        {
            var weekEnding = (payroll.WeekEnding + TimeSpan.FromDays(7 - (int)payroll.WeekEnding.DayOfWeek)).Date;
            var weekStarting = weekEnding.AddDays(-6);

            JEnumerable<JToken> children = expensesJson.Children();

            var elts = children.Where(x => {
                JToken ownerToken = x["DE:Expense Owner"];
                //JToken ownerToken = x.SelectToken("DE:Expense Owner");

                if (ownerToken == null)
                    return false;

                if (ownerToken is JArray) {
                    IJEnumerable<JToken> owners = (ownerToken as JArray).Values();
                    if (!owners.Any(s => ((string)s).Contains(payroll.EmployeeID)))
                        return false;
                }
                else {
                    string owner = (string)ownerToken;
                    if (owner == null || !owner.Contains(payroll.EmployeeID))
                        return false;
                    else {
                        Debug.WriteLine("at least once");
                    }
                }

                var effectiveDate = DateTime.Parse(x.Value<string>("effectiveDate"));

                var valid = weekStarting <= effectiveDate && effectiveDate <= weekEnding;

                if (valid) {
                    Debug.WriteLine("we're valid");
                }

                return valid;
            });

            return elts.ToList();
        }
    }
}
