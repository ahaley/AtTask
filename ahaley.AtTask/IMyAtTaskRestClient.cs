using System;
using System.Collections.Generic;
using System.Linq;
using AtTaskRestExample;
using Newtonsoft.Json.Linq;

namespace ahaley.AtTask
{
    public interface IMyAtTaskRestClient
    {
        JToken Get(ObjCode objcode, string id, params string[] fieldsToInclude);
        JToken Search(ObjCode objcode, List<string> parameters);
        JToken Create(ObjCode objcode, List<string> parameters);
        JToken Update(ObjCode objcode, List<string> parameters);
        JToken Delete(ObjCode objcode, List<string> parameters);
        void Logout();
    }
}
