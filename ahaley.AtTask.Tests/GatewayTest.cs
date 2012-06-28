using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using Newtonsoft.Json.Linq;
using AtTaskRestExample;
using ahaley.AtTask.Tests.Properties;

namespace ahaley.AtTask.Tests
{
    [TestFixture]
    public class GatewayTest
    {
        [Test]
        public void TestMethod1()
        {
            // arrange
            var builder = new FilterBuilder();
            builder.AddConstraint("endDate", DateTime.Parse("2010-12-26"));

            var client = new Mock<IMyAtTaskRestClient>();
            client.Setup(x => x.Search(ObjCode.TIMESHEET, builder.Filter))
                .Returns(JToken.Parse(Resources.timesheets));

            client.Setup(x => x.Search(ObjCode.EXPENSE, It.IsAny<List<string>>()))
                .Returns(JToken.Parse(Resources.expenses));

            /*
            var mapper = new Mock<IPayrollMapper>();
            var expectedPayroll = new Payroll[0];
            mapper.Setup(x => x.MapTimesheetsToPayrollReportItem(It.IsAny<JToken>(), It.IsAny<JToken>()))
                .Returns(expectedPayroll);*/
            
            var gateway = new Gateway(new PayrollMapper(), client.Object);


            // act
            Payroll[] result = gateway.GetTimesheetsByFilter(builder);


            // assert
            var payrollFive = result.Single(x => x.Lastname == "Five");
            Assert.AreEqual(13, payrollFive.TotalMileage);
            //mapper.Verify(m => m.MapTimesheetsToPayrollReportItem(
        }

    }

    public class GatewayTestHelper
    {
        public Mock<IMyAtTaskRestClient> Client { get; private set; }
        public Mock<IPayrollMapper> Mapper { get; private set; }
        public Payroll[] ExpectedPayroll { get; private set; }

        public GatewayTestHelper()
        {
            Client = new Mock<IMyAtTaskRestClient>();

        }

        public void GivenSearchableTimesheetsForBuilder(FilterBuilder builder)
        {
        }
    }
}
