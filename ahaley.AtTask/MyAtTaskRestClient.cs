using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtTaskRestExample;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace ahaley.AtTask
{
    public class MyAtTaskRestClient : AtTaskRestClient, IMyAtTaskRestClient
    {
        public MyAtTaskRestClient(string apiPath)
            : base(apiPath)
        {
        }

        public static IMyAtTaskRestClient Create()
        {
            var settings = ConfigurationManager.AppSettings;
            string username = settings.Get("Username");
            if (null == username) {
                throw new Exception("The AtTask username must be defined in the application settings");
            }
            string password = settings.Get("Password");
            if (null == password) {
                throw new Exception("The AtTask password must be defined in the application settings");
            }
            string atTaskUrl = settings.Get("AtTaskUrl");
            if (null == atTaskUrl) {
                throw new Exception("The AtTask URL must be defined in the application settings");
            }

            var client = new MyAtTaskRestClient(atTaskUrl);
            client.DebugUrls = true;
            client.Login(username, password);
            return client;
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
