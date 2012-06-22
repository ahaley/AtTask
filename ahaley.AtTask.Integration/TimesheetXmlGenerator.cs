using System;
using System.Xml.Serialization;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using AtTaskRestExample;

namespace ahaley.AtTask.Integration
{
    [TestFixture]
    public class TimesheetXmlGenerator
    {
        [Test]
        public void CreateTimesheetsXml()
        {
            var weekEnding = new DateTime(2010, 12, 26);
            var builder = new FilterBuilder();
            builder
            .FieldEquals("endDate", weekEnding);

            JArray timesheets = null;
            using (var gateway = new Gateway()) {
                timesheets = gateway.Client.Search(ObjCode.TIMESHEET, builder.Filter).Value<JArray>("data");
            }

            using (StreamWriter writer = new StreamWriter("timesheets.xml")) {
                writer.Write(timesheets.ToString());
            }
        }
    }
}
