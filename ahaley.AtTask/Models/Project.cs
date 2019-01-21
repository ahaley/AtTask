using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ahaley.AtTask
{
    public class Task
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string ObjCode { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? ActualCompletionDate { get; set; }

        public decimal ActualCost { get; set; }

        public decimal ActualDuration { get; set; }

        public int ActualDurationMinutes { get; set; }

        public decimal ActualExpenseCost { get; set; }
        
        public decimal ActualLaborCost { get; set; }
        
        public decimal ActualRevenue { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? ActualStartDate { get; set; }
        
        public double ActualWork { get; set; }
        
        public int ActualWorkRequired { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? ApprovalEstStartDate { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? ApprovalPlannedStartDate { get; set; }

        public string ApprovalProcessID { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? ApprovalProjectedStartDate { get; set; }
        
        public string AssignedToID { get; set; }

        public decimal BillingAmount { get; set; }

        public string BillingRecordID { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? CompletionPendingDate { get; set; }

        public decimal CostAmount { get; set; }
        
        public string CostType { get; set; }

        public string Description { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? EstCompletionDate { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? EstStartDate { get; set; }

        public double Estimate { get; set; }

        public double Work { get; set; }

        public int WorkRequired { get; set; }
    }

    public class Project
    {
        public static readonly string[] Fields = new string[] {
            "ID", "name", "objCode", "actualStartDate", "percentComplete", "plannedCompletionDate", "plannedStartDate",
            "priority", "projectedCompletionDate", "tasks:*"
        };

        public string ID { get; set; }

        public string Name { get; set; }

        public string ObjCode { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime? ActualStartDate { get; set; }

        public decimal PercentComplete { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime PlannedCompletionDate { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime PlannedStartDate { get; set; }

        public int Priority { get; set; }

        [JsonConverter(typeof(AtTaskDateConverter))]
        public DateTime ProjectedCompletionDate { get; set; }

        public string Status { get; set; }

        public IEnumerable<Task> Tasks { get; set; }
    }
}
