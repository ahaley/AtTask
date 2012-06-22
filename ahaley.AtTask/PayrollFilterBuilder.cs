using System;
using System.Collections.Generic;
using System.Linq;

namespace ahaley.AtTask
{
    public class PayrollFilterBuilder
    {
        public PayrollFilterBuilder()
        {
            Builder = new FilterBuilder();
        }

        public FilterBuilder Builder { get; set; }

        public void ApplyPayrollFilter(DateTime weekEnding, string prefix = "")
        {
            string endDate = string.Format("{0}endDate", prefix);
            string categoryID = string.Format("{0}user:categoryID", prefix);
            Builder.FieldEquals(endDate, weekEnding.ToAtTaskDate());
            Builder.NotEquals(categoryID, PayrollAdapter.ContractorCategory);
        }

        public void ApplyPayrollFilterForApproverList(string[] approverList, string prefix = "")
        {
            if (approverList == null)
                return;
            string field = string.Format("{0}approverID", prefix);
            foreach (string approver in approverList) {
                Builder.FieldEquals(field, approver);
            }
        }
    }
}
