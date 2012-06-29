using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ahaley.AtTask;
using ahaley.AtTask.Tests.Properties;

namespace ahaley.AtTask.Tests
{
    [TestFixture]
    public class PayrollMapperTests
    {
        [Test]
        public void Holiday_Value_Is_Mapped_To_Payroll()
        {
            // arrange
            JObject timesheet = CreatePayrollJson();
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll payroll = mapper.MapJsonToPayroll(timesheet);

            // assert
            Assert.AreEqual(8, payroll.HolidayHours);
        }

        [Test]
        public void PTO_Value_Is_Mapped_To_Payroll()
        {
            // arrange
            const string UserID = "9d3c8120a614cebbe040007f01002438";
            JArray timesheets = CreateAggregatePayrollJson();
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll[] payrollItems = mapper.MapAggregateJsonToPayroll(timesheets);

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
            JArray timesheets = CreateAggregatePayrollJson();
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll[] payrollItems = mapper.MapAggregateJsonToPayroll(timesheets, expenses);

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

            JArray expensesJson = JObject.Parse(expenseJson).Value<JArray>("data");
            JArray aggregatePayrollJson = CreateAggregatePayrollJson();
            PayrollMapper mapper = new PayrollMapper();

            // act
            Payroll[] payrollItems = mapper.MapAggregateJsonToPayroll(aggregatePayrollJson, expensesJson);

            // assert
            Payroll payroll = payrollItems.Single(x => x.EmployeeID == userId);
            Assert.AreEqual(3, payroll.TotalMileage);
        }

        [Test]
        public void PayrollMapper_Can_Calculate_Mileage_With_Multiple_Expense_Owners()
        {
            // arrange
            var mapper = new PayrollMapper();

            // act
            var payrollItems = mapper.MapAggregateJsonToPayroll(CreateAggregatePayrollForMultipleOwnerJson(), CreateExpensesWithMultipleOwnerJson());

            // assert
            var payroll = payrollItems.Single(x => x.EmployeeID == "9d3c8120a611cebbe040007f01002438");
            Assert.AreEqual(340.0, payroll.TotalMileage);
        }

        [Test]
        public void PayrollMapper_Can_Calculate_PerDiem_With_Multiple_Expense_Owners()
        {
            // arrange
            var mapper = new PayrollMapper();
            
            // act
            var payrollItems = mapper.MapAggregateJsonToPayroll(CreateAggregatePayrollForMultipleOwnerJson(), CreateExpensesWithMultipleOwnerJson());

            // arrange
            var payroll = payrollItems.Single(x => x.EmployeeID == "9d3c8120a640cebbe040007f01002438");
            Assert.AreEqual(5.0, payroll.TotalPerDiem);

        }

        static JObject CreatePayrollJson()
        {
            return JObject.Parse(Resources.timesheet).Value<JObject>("data");
        }

        static JArray CreateAggregatePayrollJson()
        {
            return JObject.Parse(Resources.timesheets).Value<JArray>("data");
        }

        static JArray CreateExpensesJson()
        {
            return JObject.Parse(Resources.expenses).Value<JArray>("data");
        }

        static JArray CreateAggregatePayrollForMultipleOwnerJson()
        {
            return JObject.Parse(Resources.payroll_aggregate_for_2011_12_18).Value<JArray>("data");
        }

        static JArray CreateExpensesWithMultipleOwnerJson()
        {
            return JObject.Parse(Resources.expenses_multiple_owner).Value<JArray>("data");
        }
    }
}
