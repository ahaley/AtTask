using System;
using ahaley.AtTask;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace ahaley.AtTask.Integration
{
    [TestFixture]
    public class PayrollAdapterTests
    {
        [Test]
        public void Get_Payroll_For_Period()
        {
            var adapter = new PayrollAdapter();
            var startDate = new DateTime(2011, 1, 1);
            var endDate = new DateTime(2011, 2, 1);
            var items = adapter.GetPayroll(startDate, endDate);

            Console.WriteLine("Returned {0} items", items.Length);
            foreach (var item in items) {
                Console.WriteLine("endDate = {0}", item.WeekEnding.ToShortDateString());
            }
        }

        [Test]
        public void Get_Timesheets_For_Week_Ending()
        {
            var adapter = new PayrollAdapter();
            Payroll[] payrollItems = adapter.GetPayrollWeekEnding(new DateTime(2010, 12, 26));

            Console.WriteLine("length 1 = {0}", payrollItems.Length);
            foreach (var payrollItem in payrollItems) {
                Console.WriteLine("endDate = {0}", payrollItem.WeekEnding.ToShortDateString());
            }
        }

        [Test]
        public void Get_Timesheets_For_Period_Ending()
        {
            var adapter = new PayrollAdapter();
            Payroll[] payrollItems = adapter.GetPayrollPeriodEnding(new DateTime(2010, 12, 26));

            Console.WriteLine("length 1 = {0}", payrollItems.Length);
            foreach (var payrollItem in payrollItems) {
                Console.WriteLine("endDate = {0}", payrollItem.WeekEnding.ToShortDateString());
            }
        }

        [Test]
        public void Get_Niemeyer_Timesheets_For_Week_Ending()
        {
            var adapter = new PayrollAdapter();
            Payroll[] payrollItems = adapter.GetPayrollWeekEnding(new DateTime(2010, 12, 26));

            foreach (var payrollItem in payrollItems) {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}",
                    payrollItem.EmployeeID,
                    payrollItem.Firstname,
                    payrollItem.Lastname,
                    payrollItem.WeekEnding.ToShortDateString()
                );
            }
        }

        [Test]
        public void Get_Payroll_For_2012_7_1()
        {
            var adapter = new PayrollAdapter();
            Payroll[] payrollItems = adapter.GetPayrollWeekEnding(DateTime.Parse("2012-7-1"));

            List<Payroll> list = new List<Payroll>(payrollItems);
            Payroll payroll = list.Single(x => x.Lastname == "Denton");

            Assert.AreEqual(32, payroll.RegularHours);
        }

        [Test]
        public void Get_Combined_Payroll_For_2012_8_26()
        {
            var adapter = new PayrollAdapter();
            var items = adapter.GetCombinedPayrollPeriodEnding(DateTime.Parse("2012-8-26"));

            var item = items.ToList().Single(x => x.Lastname == "Kutler");

            Assert.AreEqual(910, item.TotalMileage);
        }

        [Test]
        public void Get_Payroll_For_2012_8_26()
        {
            var adapter = new PayrollAdapter();
            var items = adapter.GetPayrollWeekEnding(DateTime.Parse("2012-8-26"));

            var item = items.ToList().Single(x => x.Lastname == "Kutler");

            Assert.AreEqual(550, item.TotalMileage);
        }

        [Test]
        public void Get_Payroll_For_2012_8_19()
        {
            var adapter = new PayrollAdapter();
            var items = adapter.GetPayrollWeekEnding(DateTime.Parse("2012-8-19"));

            var item = items.ToList().Single(x => x.Lastname == "Kutler");

            Assert.AreEqual(360, item.TotalMileage);
        }

        [Test]
        public void Get_Payroll_For_2013_1_6()
        {
            var adapter = new PayrollAdapter();
            /*
            adapter.ApproverList = new string[] {
                 "9d3c8120a649cebbe040007f01002438",
                 "9d3c8120b2a0cebbe040007f01002438",
                 "9d3c8120a636cebbe040007f01002438",
                 "9d3c8120a63ecebbe040007f01002438",
                 "4ea8a212000509f9dc0138f379c14bbf"
            };*/

            var periodPayroll = adapter.GetPayrollPeriodEnding(DateTime.Parse("2013-2-10")).ToList();
            var week1 = adapter.GetPayrollWeekEnding(DateTime.Parse("2013-2-3")).ToList();
            var week2 = adapter.GetPayrollWeekEnding(DateTime.Parse("2013-2-10")).ToList();
            
            const string employeeName = "Bonacci";
            List<Payroll> ePeriod = periodPayroll.Where(x => x.Lastname == employeeName).ToList();

            var eWeek1 = week1.Single(x => x.Lastname == employeeName);
            var eWeek2 = week2.Single(x => x.Lastname == employeeName);

            Assert.AreEqual(eWeek1.TotalMileage + eWeek2.TotalMileage, ePeriod[0].TotalMileage + ePeriod[1].TotalMileage);
        }

        [Test]
        public void Get_Payroll_For_2013_1_6_Combined()
        {
            var adapter = new PayrollAdapter();
            /*
            adapter.ApproverList = new string[] {
                 "9d3c8120a649cebbe040007f01002438",
                 "9d3c8120b2a0cebbe040007f01002438",
                 "9d3c8120a636cebbe040007f01002438",
                 "9d3c8120a63ecebbe040007f01002438",
                 "4ea8a212000509f9dc0138f379c14bbf"
            };*/

            var periodPayroll = adapter.GetCombinedPayrollPeriodEnding(DateTime.Parse("2013-2-10")).ToList();
            var week1 = adapter.GetPayrollWeekEnding(DateTime.Parse("2013-2-3")).ToList();
            var week2 = adapter.GetPayrollWeekEnding(DateTime.Parse("2013-2-10")).ToList();

            const string employeeName = "Bonacci";
            Payroll ePeriod = periodPayroll.Single(x => x.Lastname == employeeName);

            var eWeek1 = week1.Single(x => x.Lastname == employeeName);
            var eWeek2 = week2.Single(x => x.Lastname == employeeName);

            Assert.AreEqual(eWeek1.TotalMileage + eWeek2.TotalMileage, ePeriod.TotalMileage);
        }

        [Test]
        public void Get_Payroll_For_2013_2_24_Combined()
        {
            var adapter = new PayrollAdapter();
            /*
            adapter.ApproverList = new string[] {
                 "9d3c8120a649cebbe040007f01002438",
                 "9d3c8120b2a0cebbe040007f01002438",
                 "9d3c8120a636cebbe040007f01002438",
                 "9d3c8120a63ecebbe040007f01002438",
                 "4ea8a212000509f9dc0138f379c14bbf"
            };*/

            var periodPayroll = adapter.GetCombinedPayrollPeriodEnding(DateTime.Parse("2013-2-24")).ToList();
            var week1 = adapter.GetPayrollWeekEnding(DateTime.Parse("2013-2-17")).ToList();
            var week2 = adapter.GetPayrollWeekEnding(DateTime.Parse("2013-2-24")).ToList();

            const string employeeName = "Enoch";
            Payroll ePeriod = periodPayroll.Single(x => x.Lastname == employeeName);

            var eWeek1 = week1.Single(x => x.Lastname == employeeName);
            var eWeek2 = week2.Single(x => x.Lastname == employeeName);

            Assert.AreEqual(eWeek1.TotalMileage + eWeek2.TotalMileage, ePeriod.TotalMileage);
        }

        [Test]
        public void Get_Payroll_For_2013_3_10_Combined()
        {
            var adapter = new PayrollAdapter();

            var periodPayroll = adapter.GetCombinedPayrollPeriodEnding(DateTime.Parse("2013-3-10")).ToList();
         
            Payroll ePeriod = periodPayroll.Single(x => x.EmployeeID == "4ebcb37b001928943846f3a774d7cb33");

            Assert.AreEqual(235, ePeriod.TotalMileage);
        }

        [Test]
        public void Get_Payroll_For_2013_4_14()
        {
            var adapter = new PayrollAdapter();
            var payrollPeriod = adapter.GetPayrollWeekEnding(DateTime.Parse("2013-4-21")).ToList();

            Payroll payroll = payrollPeriod.Single(x => x.EmployeeID == "4ec2cd5f00352d8e7f89ffc2f9e7ac15");

            Assert.AreEqual(40, payroll.TotalHours);
            Assert.AreEqual(20, payroll.SuspensionHours);
        }

        [Test]
        public void Get_Payroll_For_2014_3_30()
        {
            var adapter = new PayrollAdapter();
            var payrollPeriod = adapter.GetPayrollWeekEnding(DateTime.Parse("2014-3-30")).ToList();
            Payroll payroll = payrollPeriod.Single(x => x.EmployeeID == "52938133000c12a6656d813f18f95b08");
            Assert.AreEqual(237, payroll.TotalMileage);
            Assert.AreEqual(0, payroll.TotalPerDiem);
        }

        [Test]
        public void Get_Payroll_For_2014_4_6()
        {
            var adapter = new PayrollAdapter();
            var payrollPeriod = adapter.GetPayrollPeriodEnding(DateTime.Parse("2014-4-6")).ToList();
            IEnumerable<Payroll> payroll = payrollPeriod.Where(x => x.EmployeeID == "52938133000c12a6656d813f18f95b08");

            var totalMileage = payroll.Sum(p => p.TotalMileage);
            Assert.AreEqual(653.25, totalMileage);
        }

    }
}
