using System;
using System.Collections.Generic;
using AtTaskRestExample;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace ahaley.AtTask
{
    public class Gateway : IDisposable, IGateway
    {
        static readonly string TimesheetFields = "fields=userID,regularHours,totalHours,overtimeHours,endDate,user,user:firstName,user:lastName,hours:hours,hours:hourTypeID";
        readonly IMyAtTaskRestClient client;
        readonly IPayrollMapper mapper;

        readonly string[] userFields = new string[] {
            "ID", "name", "firstName", "lastName", "title", "address", "city",
            "state", "postalCode", "company:name", "phoneNumber", "mobilePhoneNumber",
            "emailAddr", "entryDate"
        };

        public Gateway(IPayrollMapper mapper, IMyAtTaskRestClient client = null)
        {
            this.mapper = mapper;
            this.client = client ?? MyAtTaskRestClient.Create();
        }

        public Gateway()
            : this(new PayrollMapper())
        {
        }

        public IMyAtTaskRestClient Client
        {
            get
            {
                return client;
            }
        }

        public void Dispose()
        {
            Client.Logout();
        }

        public Payroll[] GetTimesheetsByFilter(FilterBuilder builder)
        {
            List<string> timesheetParams = builder.Filter;
            timesheetParams.Add(TimesheetFields);
            JArray timesheets = Client.Search(ObjCode.TIMESHEET, timesheetParams).Value<JArray>("data");

            JArray expenses = null;
            if (builder.ContainsDateRange) {
                ExpenseAdapter adapter = new ExpenseAdapter(this);
                expenses = adapter.GetExpenses(builder.StartDate.AddDays(1), builder.EndDate);
            }

            return mapper.MapAggregateJsonToPayroll(timesheets, expenses);
        }

        public Employee GetEmployee(string id)
        {
            JObject user = Client.Get(ObjCode.USER, id, userFields).Value<JObject>("data");

            return new Employee() {
                GUID = user.Value<string>("ID"),
                Name = user.Value<string>("name"),
                FirstName = user.Value<string>("firstName"),
                LastName = user.Value<string>("lastName"),
                Title = user.Value<string>("title"),
                Address = user.Value<string>("address"),
                City = user.Value<string>("city"),
                State = user.Value<string>("state"),
                Zip = user.Value<string>("postalCode"),
                Company = user.Value<JObject>("company").Value<string>("name"),
                Telephone = user.Value<string>("phoneNumber"),
                CellPhone = user.Value<string>("mobilePhoneNumber"),
                Email = user.Value<string>("emailAddr"),
                HireDate = user.Value<string>("entryDate").FromAtTaskDate()
            };
        }

        public Project GetProject(string id)
        {
            JToken token  = Client.Get(ObjCode.PROJECT, id, Project.Fields);
            JObject projectObject = token.Value<JObject>("data");
            Project project = projectObject.ToObject<Project>();




            return project;
        }
    }
}
