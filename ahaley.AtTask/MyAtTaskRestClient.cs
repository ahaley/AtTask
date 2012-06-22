using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtTaskRestExample;
using Newtonsoft.Json.Linq;

namespace ahaley.AtTask
{
    public class MyAtTaskRestClient : AtTaskRestClient
    {
        public MyAtTaskRestClient(string apiPath)
            : base(apiPath)
        {
        }

        public virtual JToken Search(ObjCode objcode, List<string> parameters)
        {
            VerifySignedIn();
            parameters.Add("sessionID=" + SessionID);
            JToken json = client.DoGet(string.Format("/{0}/search", objcode), parameters.ToArray());
            return json;
        }

        public JToken Create(ObjCode objcode, List<string> parameters)
        {
            VerifySignedIn();
            parameters.Add("sessionID=" + SessionID);
            JToken json = client.DoPost(string.Format("/{0}", objcode), parameters.ToArray());
            return json;
        }
        public JToken Update(ObjCode objcode, List<string> parameters)
        {
            VerifySignedIn();
            parameters.Add("sessionID=" + SessionID);
            JToken json = client.DoPut(string.Format("/{0}", objcode), parameters.ToArray());
            return json;
        }

        public JToken Delete(ObjCode objcode, List<string> parameters)
        {
            VerifySignedIn();
            parameters.Add("sessionID=" + SessionID);
            JToken json = client.DoDelete(string.Format("/{0}", objcode), parameters.ToArray());
            return json;
        }

    }
}
