using System;
using NUnit.Framework;
using System.IO;
using Newtonsoft.Json.Linq;
using AtTaskRestExample;

namespace ahaley.AtTask.Integration
{
    [TestFixture]
    class ExpenseXmlGenerator
    {
        [Test]
        public void CreateFlatExpenseXml()
        {
            var builder = new FilterBuilder();
            builder.DateRange("entryDate", new DateTime(2011, 2, 1), new DateTime(2011, 3, 15));

            JArray expenses = null;
            using (Gateway gateway = new Gateway()) {
                expenses = gateway.Client.Search(ObjCode.EXPENSE, builder.Filter).Value<JArray>("data");
            }

            using (StreamWriter writer = new StreamWriter("expenses.xml")) {
                writer.Write(expenses.ToString());
            }
        }
    }
}
