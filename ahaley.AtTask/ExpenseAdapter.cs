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
            this.gateway = gateway;
        }

        static readonly string InChargeExpenseFields = "fields=expenseTypeID,actualUnitAmount,effectiveDate,DE:Expense Owner";
        readonly IGateway gateway;

        public JArray GetExpenses(DateTime startDate, DateTime endDate)
        {
            var expenseParameters = CreateExpenseParameters(startDate, endDate);
            return gateway.Client.Search(ObjCode.EXPENSE, expenseParameters).Value<JArray>("data");
        }

        List<string> CreateExpenseParameters(DateTime startDate, DateTime endDate)
        {
            var expenseBuilder = new FilterBuilder();
            expenseBuilder.ShortDateRange("effectiveDate", startDate, endDate);
            List<string> expenseFields = expenseBuilder.Filter;
            expenseFields.Add(InChargeExpenseFields);
            return expenseFields;
        }
    }
}
