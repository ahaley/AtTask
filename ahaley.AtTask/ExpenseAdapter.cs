using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using AtTaskRestExample;

namespace ahaley.AtTask
{
    public class ExpenseAdapter
    {
        public ExpenseAdapter(IGateway gateway)
        {
            _gateway = gateway;
        }

        private static readonly string InChargeExpenseFields = "fields=expenseTypeID,actualUnitAmount,effectiveDate,DE:Expense Owner";
        private readonly IGateway _gateway;

        public JArray GetExpenses(DateTime startDate, DateTime endDate)
        {
            var expenseFilterBuilder = CreateExpenseFields(startDate, endDate);
            return _gateway.Client.Search(ObjCode.EXPENSE, expenseFilterBuilder).Value<JArray>("data");
        }

        private List<string> CreateExpenseFields(DateTime startDate, DateTime endDate)
        {
            var expenseBuilder = new FilterBuilder();
            expenseBuilder.ShortDateRange("effectiveDate", startDate, endDate);
            List<string> expenseFields = expenseBuilder.Filter;
            expenseFields.Add(InChargeExpenseFields);
            return expenseFields;
        }
    }
}
