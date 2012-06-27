using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ahaley.AtTask;
using ahaley.AtTask.Tests.Properties;

namespace ahaley.AtTask.Test
{
    [TestFixture]
    public class PayrollMapperTests
    {
        [Test]
        public void Holiday_Value_Is_Mapped_To_Payroll()
        {
            // arrange
            JObject timesheet = CreateTimesheet().Value<JObject>("data");
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll payroll = mapper.MapTimesheetToPayrollReportItem(timesheet);

            // assert
            Assert.AreEqual(8, payroll.HolidayHours);
        }

        [Test]
        public void PTO_Value_Is_Mapped_To_Payroll()
        {
            // arrange
            const string UserID = "9d3c8120a614cebbe040007f01002438";
            JArray timesheets = CreateTimesheets().Value<JArray>("data");
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll[] payrollItems = mapper.MapTimesheetsToPayrollReportItem(timesheets);

            // assert
            var item = payrollItems.Single(x => x.EmployeeID == UserID);
            Assert.AreEqual(32, item.PaidTimeOff);
        }

        [Test]
        public void Expense_Based_Values_Map_InCharge_Correctly()
        {
            // arrange
            const string userId = "9d3c8120a653cebbe040007f01002438";

            string expenseJson = @"{ ""data"": [{
                ""actualUnitAmount"": 4,
                ""effectiveDate"": ""2010-12-26"",
                ""DE:Expense Owner"": ""9d3c8120a653cebbe040007f01002438"",
                ""expenseTypeID"": ""9d3c90342fe3fa2ae040007f01002426""},
                {""actualUnitAmount"": 3,
                ""effectiveDate"": ""2010-12-26"",
                ""DE:Expense Owner"": ""9d3c8120a653cebbe040007f01002438"",
                ""expenseTypeID"": ""9d3c90342fe3fa2ae040007f01002426""},
                {""actualUnitAmount"": 9,
                ""effectiveDate"": ""2010-12-26"",
                ""DE:Expense Owner"": ""1337"",
                ""expenseTypeID"": ""9d3c90342fe3fa2ae040007f01002426""}]}";

            JArray expenses = JObject.Parse(expenseJson).Value<JArray>("data");
            JArray timesheets = CreateTimesheets().Value<JArray>("data");
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll[] payrollItems = mapper.MapTimesheetsToPayrollReportItem(timesheets, expenses);

            // assert
            Payroll payroll = payrollItems.Single(x => x.EmployeeID == userId);
            Assert.AreEqual(7, payroll.InChargeDays);
        }

        [Test]
        public void Mileage_Expense_Calculated_Correctly()
        {
            // arrange
            const string userId = "9d3c8120a653cebbe040007f01002438";

            string expenseJson = @"{ ""data"": [{
                ""actualUnitAmount"": 4,
                ""effectiveDate"": ""2010-12-26"",
                ""DE:Expense Owner"": ""9d3c8120a653cebbe040007f01002438"",
                ""expenseTypeID"": ""9d3c90342fe3fa2ae040007f01002426""},
                {""actualUnitAmount"": 3,
                ""effectiveDate"": ""2010-12-26"",
                ""DE:Expense Owner"": ""9d3c8120a653cebbe040007f01002438"",
                ""expenseTypeID"": ""9d3c90342fc8fa2ae040007f01002426""},
                {""actualUnitAmount"": 9,
                ""effectiveDate"": ""2010-12-26"",
                ""DE:Expense Owner"": ""1337"",
                ""expenseTypeID"": ""9d3c90342fe3fa2ae040007f01002426""}]}";

            JArray expenses = JObject.Parse(expenseJson).Value<JArray>("data");
            JArray timesheets = CreateTimesheets().Value<JArray>("data");
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll[] payrollItems = mapper.MapTimesheetsToPayrollReportItem(timesheets, expenses);

            // assert
            Payroll payroll = payrollItems.Single(x => x.EmployeeID == userId);
            Assert.AreEqual(3, payroll.TotalMileage);
        }

        private static JObject CreateTimesheet()
        {
            return JObject.Parse(Resources.timesheet);
        }

        private static JObject CreateTimesheets()
        {
            return JObject.Parse(Resources.timesheets);
        }

        private static JObject GetExpensesJArray()
        {
            return JObject.Parse(Resources.expenses);
        }
    }
}
