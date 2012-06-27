using System;

namespace ahaley.AtTask
{
    public interface IGateway
    {
        Payroll[] GetTimesheetsByFilter(FilterBuilder builder);
        Employee GetEmployee(string id);
        IMyAtTaskRestClient Client { get; }
    }
}
