using System;
using System.Collections.Generic;
using System.Linq;
using AtTaskRestExample;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ahaley.AtTask.Integration
{
    [TestFixture]
    public class GatewayTest
    {
        [Test]
        public void Gateway_Can_Connect()
        {
            using (Gateway gateway = new Gateway()) {
            }
        }

        [Test]
        public void Gateway_Can_Retrieve_Employee()
        {
            const string drecksageCode = "9d3c8120a653cebbe040007f01002438";
            Employee emp = null;
            using (Gateway gateway = new Gateway()) {
                emp = gateway.GetEmployee(drecksageCode);
            }
            Assert.AreEqual("Barry Drecksage", emp.Name);
        }

        [Test]
        public void Calculate_InCharge_Bonus_From_Raw_REST_Client()
        {
            var gateway = new Gateway();
            var builder = new FilterBuilder();
            builder.DateRange("effectiveDate", new DateTime(2010, 11, 22), new DateTime(2010, 11, 28));
            var filter = builder.Filter;
            filter.Add("fields=expenseTypeID,actualUnitAmount,effectiveDate,parameterValues");

            JArray expenses = gateway.Client.Search(ObjCode.EXPENSE, filter).Value<JArray>("data");

            const string drecksageCode = "9d3c8120a653cebbe040007f01002438";
            const string InChargeExpenseType = "9d3c90342fe3fa2ae040007f01002426";

            IEnumerable<JToken> inChargeExpenses = from e in expenses.Children()
                                                   where e.Value<string>("expenseTypeID") == InChargeExpenseType
                                                   && e.Value<JObject>("parameterValues").Value<string>("DE:Expense Owner") == drecksageCode
                                                   select e;
            Assert.AreEqual(1, inChargeExpenses.Sum(x => x.Value<int>("actualUnitAmount")));

            gateway.Dispose();
        }


        [Test]
        public void Calculate_Drecksage_InChargeDays()
        {
            // arrange
            const string drecksageCode = "9d3c8120a653cebbe040007f01002438";
            var filter = new FilterBuilder();
            filter.DateRange("endDate", new DateTime(2010, 11, 22), new DateTime(2010, 11, 28));
            Payroll[] payroll = null;

            // act
            using (Gateway gateway = new Gateway()) {
                payroll = gateway.GetTimesheetsByFilter(filter);
            }

            // assert
            Payroll drecksagePayroll = payroll.Single(x => x.EmployeeID == drecksageCode);

            Console.WriteLine("in charge = {0}", drecksagePayroll.InChargeDays);
        }

        [Test]
        public void Get_Holiday_From_Raw_REST_Client()
        {
            // arrange
            var empId = "9d3c8120a643cebbe040007f01002438";
            int expectedHoliday = 8;
            var weekEnding = new DateTime(2010, 12, 26);
            var gateway = new Gateway();

            FilterBuilder builder = new FilterBuilder();
            builder.FieldEquals("userID", empId);
            builder.FieldEquals("endDate", weekEnding);
            List<string> filter = builder.Filter;
            filter.Add("fields=hours,hours:hourTypeID,hours:hours");

            // act
            JArray timesheets = gateway.Client.Search(ObjCode.TIMESHEET, filter).Value<JArray>("data");

            // assert
            Assert.AreEqual(1, timesheets.Count);
            JObject timesheet = timesheets.Children<JObject>().First();

            IEnumerable<JObject> holidayHours = from h in timesheet.Value<JArray>("hours").Children<JObject>()
                                                where h.Value<string>("hourTypeID") == "9d3c6e51ed444d35e040007f0100243c"
                                                select h;

            double holiday = holidayHours.Sum(x => x.Value<double>("hours"));
            Assert.AreEqual(expectedHoliday, holiday);
        }
    }
}
