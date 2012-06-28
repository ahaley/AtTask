using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using ahaley.AtTask;

namespace ahaley.AtTask.Tests
{
    [TestFixture]
    class PayrollAdapterTest
    {
        DateTime expectedWeekEnding, expectedStartDate, expectedStopDate;
        Mock<IGateway> gateway;
        PayrollAdapter payrollAdapter;
        string expectedUserId;
        Payroll[] expectedPayrollItems, actualPayrollItems;


        [SetUp]
        public void SetUp()
        {
            expectedWeekEnding = new DateTime(2010, 1, 5);
            expectedStartDate = new DateTime(2010, 1, 1);
            expectedStopDate = new DateTime(2010, 1, 10);
            gateway = new Mock<IGateway>();
            gateway.Setup(x => x.GetTimesheetsByFilter(It.IsAny<FilterBuilder>()))
                .Returns((Payroll[])null);
            expectedUserId = "1";
            expectedPayrollItems = null;
            actualPayrollItems = null;
            payrollAdapter = null;
        }

        [Test]
        public void PayrollForEmployee_Should_Send_Correct_Filters_To_Gateway()
        {
            Given_Payroll_Adapter();
            When_PayrollForEmployee_Is_Called();
            Then_Gateway_Should_Be_Called_For_That_User();
        }

        [Test]
        public void Payroll_Should_Send_Correct_Filters_To_Gateway()
        {
            Given_Payroll_Adapter();
            When_GetPayroll_Is_Called();
            Then_The_Gateway_Should_Be_Called_With_The_Date_Range_Filter();
        }

        [Test]
        public void PayrollWeekEnding_Should_Send_Correct_Filters_To_Gateway()
        {
            Given_Payroll_Adapter();
            When_GetPayrollWeekEnding_Is_Called();
            Then_Gateway_Should_Be_Called_For_That_Week();
        }

        [Test]
        public void PayrollWeekEnding_Should_Filter_By_Approver_List()
        {
            Given_Payroll_Adapter_Including_Approver_List();
            When_GetPayrollWeekEnding_Is_Called();
            Then_Gateway_Should_Be_Called_For_That_Week_And_Filtered_Approvers();
        }

        [Test]
        public void PayrollPeriodEnding_Should_Send_Correct_Filters_To_Gateway()
        {
            Given_Payroll_Adapter();
            When_GetPayrollPeriodEnding_Is_Called();
            Then_Gateway_Should_Be_Called_For_That_Period();
        }

        [Test]
        public void PayrollPeriodEnding_Should_Filter_By_Approver_List()
        {
            Given_Payroll_Adapter_Including_Approver_List();
            When_GetPayrollPeriodEnding_Is_Called();
            Then_Gateway_Should_Be_Called_For_That_Period_And_Filtered_Approvers();
        }

        [Test]
        public void PayRollCombinedPeriodEnding_Should_Combine_Weeks()
        {
            Given_Payroll_Adapter_Including_Multiple_Weeks_For_Employee();
            When_GetCombinedPayrollEnding_Is_Called();
            Then_The_Resulting_Payroll_Should_Be_Combined_Per_Week();
        }
        
        void Given_Payroll_Adapter()
        {
            payrollAdapter = new PayrollAdapter(gateway.Object);
        }

        void Given_Payroll_Adapter_Including_Approver_List()
        {
            payrollAdapter = new PayrollAdapter(gateway.Object);
            payrollAdapter.ApproverList = new string [] { "abc", "abd", "abe" };
        }

        void Given_Payroll_Adapter_Including_Multiple_Weeks_For_Employee()
        {
            expectedPayrollItems = new Payroll[] {
                new Payroll() { WeekEnding = expectedWeekEnding.AddDays(-7), EmployeeID = "1337", TotalHours = 10 },
                new Payroll() { WeekEnding = expectedWeekEnding, EmployeeID = "1337", TotalHours = 15 }
            };

            gateway.Setup(g => g.GetTimesheetsByFilter(It.IsAny<FilterBuilder>()))
                .Returns(expectedPayrollItems);

            payrollAdapter = new PayrollAdapter(gateway.Object);

        }

        void When_GetPayrollWeekEnding_Is_Called()
        {
            var items = payrollAdapter.GetPayrollWeekEnding(expectedWeekEnding);
        }

        void When_GetPayroll_Is_Called()
        {
            var items = payrollAdapter.GetPayroll(expectedStartDate, expectedStopDate);
        }

        void When_PayrollForEmployee_Is_Called()
        {
            var item = payrollAdapter.GetPayrollForEmployee(expectedUserId, expectedWeekEnding);
        }

        void When_GetPayrollPeriodEnding_Is_Called()
        {
            var items = payrollAdapter.GetPayrollPeriodEnding(expectedWeekEnding);
        }

        void When_GetCombinedPayrollEnding_Is_Called()
        {
            actualPayrollItems = payrollAdapter.GetCombinedPayrollPeriodEnding(expectedWeekEnding);
        }

        void Then_Gateway_Should_Be_Called_For_That_Week()
        {
            var expectedBuilder = new FilterBuilder();
            expectedBuilder.AddConstraint("endDate", expectedWeekEnding.ToAtTaskDate());
            expectedBuilder.NotEquals("user:categoryID", PayrollAdapter.ContractorCategory);

            gateway.Verify(g => g.GetTimesheetsByFilter(
                It.Is<FilterBuilder>(b => b.Equals(expectedBuilder))
            ));
        }

        void Then_Gateway_Should_Be_Called_For_That_Week_And_Filtered_Approvers()
        {
            var expectedBuilder = new FilterBuilder();
            expectedBuilder.AddConstraint("endDate", expectedWeekEnding.ToAtTaskDate());
            expectedBuilder.AddConstraint("approverID", "abc");
            expectedBuilder.AddConstraint("approverID", "abd");
            expectedBuilder.AddConstraint("approverID", "abe");
            expectedBuilder.NotEquals("user:categoryID", PayrollAdapter.ContractorCategory);

            gateway.Verify(g => g.GetTimesheetsByFilter(
                It.Is<FilterBuilder>(b => b.Equals(expectedBuilder))
            ));
        }

        void Then_Gateway_Should_Be_Called_For_That_Period()
        {
            var expectedBuilder = new FilterBuilder();
            expectedBuilder.AddConstraint("endDate", expectedWeekEnding.ToAtTaskDate());
            expectedBuilder.AddConstraint("OR:a:endDate", expectedWeekEnding.AddDays(-7).ToAtTaskDate());
            expectedBuilder.NotEquals("user:categoryID", PayrollAdapter.ContractorCategory);
            expectedBuilder.NotEquals("OR:a:user:categoryID", PayrollAdapter.ContractorCategory);

            gateway.Verify(g => g.GetTimesheetsByFilter(
                It.Is<FilterBuilder>(b => b.Equals(expectedBuilder))
            ));
        }

        void Then_The_Gateway_Should_Be_Called_With_The_Date_Range_Filter()
        {
            var expectedBuilder = new FilterBuilder();

            expectedBuilder.DateRange("endDate", expectedStartDate, expectedStopDate);
        }

        void Then_Gateway_Should_Be_Called_For_That_Period_And_Filtered_Approvers()
        {
            var expectedBuilder = new FilterBuilder();
            expectedBuilder.AddConstraint("endDate", expectedWeekEnding.ToAtTaskDate());
            expectedBuilder.AddConstraint("approverID", "abc");
            expectedBuilder.AddConstraint("approverID", "abd");
            expectedBuilder.AddConstraint("approverID", "abe");
            expectedBuilder.AddConstraint("OR:a:endDate", expectedWeekEnding.AddDays(-7).ToAtTaskDate());
            expectedBuilder.AddConstraint("OR:a:approverID", "abc");
            expectedBuilder.AddConstraint("OR:a:approverID", "abd");
            expectedBuilder.AddConstraint("OR:a:approverID", "abe");
            expectedBuilder.NotEquals("user:categoryID", PayrollAdapter.ContractorCategory);
            expectedBuilder.NotEquals("OR:a:user:categoryID", PayrollAdapter.ContractorCategory);

            gateway.Verify(g => g.GetTimesheetsByFilter(
                It.Is<FilterBuilder>(b => b.Equals(expectedBuilder))
            ));
        }

        void Then_Gateway_Should_Be_Called_For_That_User()
        {
            var expectedBuilder = new FilterBuilder();
            expectedBuilder.AddConstraint("userID", expectedUserId);
            expectedBuilder.AddConstraint("endDate", expectedWeekEnding.ToAtTaskDate());

            gateway.Verify(g => g.GetTimesheetsByFilter(
                It.Is<FilterBuilder>(b => b.Equals(expectedBuilder))
            ));
        }

        void Then_The_Resulting_Payroll_Should_Be_Combined_Per_Week()
        {
            Assert.AreEqual(1, actualPayrollItems.Length);
            Assert.AreEqual("1337", actualPayrollItems[0].EmployeeID);
            Assert.AreEqual(25, actualPayrollItems[0].TotalHours);
        }

    }
}
