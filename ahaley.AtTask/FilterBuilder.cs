using System;
using System.Collections.Generic;
using System.Linq;

namespace ahaley.AtTask
{
    public class FilterBuilder : IEquatable<FilterBuilder>
    {
        public FilterBuilder()
        {
            _filters = new List<string>();
            ContainsDateRange = false;
        }

        public List<string> Filter
        {
            get { return _filters; }
        }

        private readonly List<string> _filters;

        public bool ContainsDateRange { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public bool Equals(FilterBuilder builder)
        {
            var left = _filters;
            var right = builder._filters;
            if (left.Count != right.Count)
                return false;
            return left.All(l => right.Any(r => l == r));
        }

        public FilterBuilder FieldEquals(string field, string value)
        {
            _filters.Add(String.Format("{0}={1}", field, value));
            return this;
        }

        public FilterBuilder ApplyOperation(string field, string value, string opcode)
        {
            FieldEquals(field, value);
            FieldEquals(field + "_Mod", opcode);
            return this;
        }

        public FilterBuilder FieldEquals(string field, DateTime date)
        {
            if (ContainsDateRange) {
                if (date > EndDate)
                    EndDate = date;
                var weekPrior = date.AddDays(-7);
                if (weekPrior < StartDate)
                    StartDate = weekPrior;
            }
            else {
                EndDate = date;
                StartDate = date.AddDays(-7);
                ContainsDateRange = true;
            }
            return FieldEquals(field, date.ToAtTaskDate());
        }

        public FilterBuilder NotEquals(string field, string value)
        {
            return ApplyOperation(field, value, "ne");
        }

        public FilterBuilder DateRange(string field, DateTime startDate, DateTime endDate)
        {
            ContainsDateRange = true;
            StartDate = startDate;
            EndDate = endDate;
            FieldEquals(field, startDate.ToAtTaskDate());
            FieldEquals(field + "_Range", endDate.ToAtTaskDate());
            return this;
        }

        public FilterBuilder ShortDateRange(string field, DateTime startDate, DateTime endDate)
        {
            ContainsDateRange = true;
            StartDate = startDate;
            EndDate = endDate;
            FieldEquals(field, startDate.ToShortDateString());
            FieldEquals(field + "_Range", endDate.ToShortDateString());
            return this;
        }

        public FilterBuilder GreaterThanOrEqual(string field, string value)
        {
            return ApplyOperation(field, value, "gte");
        }

        public FilterBuilder GreaterThanOrEqual(string field, DateTime date)
        {
            return GreaterThanOrEqual(field, date.ToAtTaskDate());
        }

        public FilterBuilder LessThanOrEqual(string field, string value)
        {
            return ApplyOperation(field, value, "lte");
        }

        public FilterBuilder LessThanOrEqual(string field, DateTime date)
        {
            return LessThanOrEqual(field, date.ToAtTaskDate());
        }
    }
}
