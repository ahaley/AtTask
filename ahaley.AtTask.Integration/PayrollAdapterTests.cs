using System;
using ahaley.AtTask;
using NUnit.Framework;

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
    }
}
