using System;
using Newtonsoft.Json.Linq;

namespace ahaley.AtTask
{
    public interface IPayrollMapper
    {
        Payroll[] MapAggregateJsonToPayroll(JToken payrollAggregateJson, JToken expensesJson);
        Payroll MapJsonToPayroll(JToken payrollJson, JToken expensesJson);
    }
}
