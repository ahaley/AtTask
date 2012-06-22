using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using ahaley.AtTask;

namespace ahaley.AtTask.Test
{
    [TestFixture]
    public class FilterBuilderTest
    {
        [Test]
        public void Builders_Should_Be_Equal_If_Filters_Match()
        {
            // arrange
            var builder1 = new FilterBuilder();
            var builder2 = new FilterBuilder();
            builder1.DateRange("startDate", new DateTime(2011, 1, 1), new DateTime(2011, 1, 12));
            builder2.DateRange("startDate", new DateTime(2011, 1, 1), new DateTime(2011, 1, 12));

            // act, assert
            Assert.IsTrue(builder1.Equals(builder2));
            Assert.IsTrue(builder2.Equals(builder1));
            Assert.IsTrue(builder1.Equals(builder1));
        }

        [Test]
        public void Builders_Should_Not_Be_Equal_If_Filters_Do_Not_Match()
        {
            // arrange
            var builder1 = new FilterBuilder();
            var builder2 = new FilterBuilder();
            builder1.DateRange("startDate", new DateTime(2011, 1, 1), new DateTime(2011, 1, 12));
            builder2.DateRange("startDate", new DateTime(2011, 1, 1), new DateTime(2011, 1, 13));

            // act, assert
            Assert.IsFalse(builder1.Equals(builder2));
            Assert.IsFalse(builder2.Equals(builder1));
        }

        [Test]
        public void Single_Field_Equals_Contructs_Correct_Filter()
        {
            // arrange
            var builder = new FilterBuilder();

            // act
            builder.FieldEquals("name1", "value1");

            // assert
            List<string> filter = builder.Filter;
            Assert.AreEqual(1, filter.Count);
            Assert.IsTrue(filter.Contains("name1=value1"));
        }

        [Test]
        public void FieldEquals_With_DateTime_Produces_AtTask_Date_String()
        {
            // arrange
            var builder = new FilterBuilder();
            var date = new DateTime(2011, 1, 1);

            // act
            builder.FieldEquals("test1", date);

            // assert
            Assert.IsTrue(builder.Filter.Contains("test1=" + date.ToAtTaskDate()));
        }

        [Test]
        public void Builder_Handles_Filter_Composition_Correctly()
        {
            // arrange
            var builder = new FilterBuilder();
            builder.FieldEquals("name1", "value1");
            builder.FieldEquals("name2", "value2");

            // act
            List<string> filter = builder.Filter;
            // assert
            Assert.AreEqual(2, filter.Count);
            Assert.IsTrue(filter.Contains("name1=value1"));
            Assert.IsTrue(filter.Contains("name2=value2"));
        }

        [Test]
        public void Builder_Constructs_Proper_Date_Range_Operation()
        {
            // arrange
            var builder = new FilterBuilder();
            var startDate = new DateTime(2010, 12, 1);
            var endDate = new DateTime(2010, 12, 31);

            // act
            builder.DateRange("startDate", startDate, endDate);

            // assert
            List<string> filter = builder.Filter;
            Assert.AreEqual(2, filter.Count);
            Assert.IsTrue(filter.Contains("startDate=2010-12-01T13:27:29:999"));
            Assert.IsTrue(filter.Contains("startDate_Range=2010-12-31T13:27:29:999"));
        }

        [Test]
        public void Builder_Constructs_Proper_Date_Greater_Than_Operation()
        {
            var builder = new FilterBuilder();

            ExpectCorrectDateRangeFilter("startDate", "gte", (field, date) => {
                builder.GreaterThanOrEqual(field, date);
                return builder.Filter;
            });
        }

        [Test]
        public void Builder_Constructs_Proper_Date_Less_Than_Operation()
        {
            var builder = new FilterBuilder();

            ExpectCorrectDateRangeFilter("startDate", "lte", (field, date) => {
                builder.LessThanOrEqual(field, date);
                return builder.Filter;
            });
        }

        [Test]
        public void ContainsDateRange_Will_Return_True_If_Date_Range_Exists()
        {
            // arrange
            var builder = new FilterBuilder();
            var start = new DateTime(2010, 1, 1);
            var end = new DateTime(2010, 1, 15);
            builder.DateRange("endDate", start, end);

            // act, assert
            Assert.IsTrue(builder.ContainsDateRange);
            Assert.AreEqual(start, builder.StartDate);
            Assert.AreEqual(end, builder.EndDate);
        }

        [Test]
        public void ContainsDateRange_Will_Return_False_If_Date_Range_Doesnt_Exist()
        {
            // arrange
            var builder = new FilterBuilder();

            // act, assert
            Assert.IsFalse(builder.ContainsDateRange);
        }

        [Test]
        public void Given_Date_Equality_Date_Range_Will_Start_Week_Prior()
        {
            // arrange
            var builder = new FilterBuilder();
            var endDate = new DateTime(2011, 1, 16);
            // act
            builder.FieldEquals("endDate", endDate);

            // assert
            Assert.IsTrue(builder.ContainsDateRange);
            Assert.AreEqual(endDate, builder.EndDate);
            Assert.AreEqual(endDate.AddDays(-7), builder.StartDate);
        }

        [Test]
        public void Given_Two_Date_Equalities_Start_And_End_Date_Are_Extents()
        {
            // arrange
            var builder = new FilterBuilder();
            var endDate1 = new DateTime(2011, 1, 16);
            var endDate2 = new DateTime(2011, 1, 23);
            builder.FieldEquals("endDate", endDate1);
            builder.FieldEquals("OR:a:endDate", endDate2);

            // act, assert
            Assert.IsTrue(builder.ContainsDateRange);
            Assert.AreEqual(endDate2, builder.EndDate);
            Assert.AreEqual(endDate1.AddDays(-7), builder.StartDate);
        }

        [Test]
        public void Given_Only_EndDate_StartDate_Extent_Is_Week_Prior()
        {
            // arrange
            var builder = new FilterBuilder();
            var endDate = new DateTime(2011, 3, 27);
            builder.FieldEquals("endDate", endDate);

            // act, assert
            Assert.IsTrue(builder.ContainsDateRange);
            Assert.AreEqual(endDate, builder.EndDate);
            Assert.AreEqual(endDate.AddDays(-7), builder.StartDate);
        }

        private void ExpectCorrectDateRangeFilter(string field, string opcode, Func<string, DateTime, List<string>> operation)
        {
            var date = new DateTime(2010, 12, 15);
            string attaskDate = date.ToAtTaskDate();
            List<string> filter = operation(field, date);
            Assert.AreEqual(2, filter.Count);
            Assert.IsTrue(filter.Contains(String.Format("{0}={1}", field, attaskDate)));
            Assert.IsTrue(filter.Contains(String.Format("{0}_Mod={1}", field, opcode)));
        }

    }
}
