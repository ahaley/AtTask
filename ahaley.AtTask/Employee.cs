using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ahaley.AtTask
{
    public class Employee
    {
        public int ID { get; set; }

        public string GUID { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Company { get; set; }

        public string Telephone { get; set; }

        public string FaxNumber { get; set; }

        public string PagerNumber { get; set; }

        public string CellPhone { get; set; }

        public string AltPhone { get; set; }

        public string Email { get; set; }

        public string WebPage { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public Nullable<DateTime> CreationDate { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public Nullable<DateTime> EditDate { get; set; }

        public string EditedByWho { get; set; }

        public string Type { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public Nullable<DateTime> HireDate { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public Nullable<DateTime> TerminationDate { get; set; }

        public bool Payroll { get; set; }

        public bool Active { get; set; }
    }
}
