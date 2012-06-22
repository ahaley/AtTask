 /*
 * Copyright (c) 2010 AtTask, Inc.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
 * Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtTaskRestExample
{
    public class AtTaskRestClient : IDisposable
    {
        protected JToken sessionResponse;
        protected RestClient client;
        /// <summary>
        /// Gets if the instance of the AtTaskRestClient has successfully logged in
        /// </summary>
        public bool IsSignedIn
        {
            get {
                return sessionResponse != null; }
        }
        /// <summary>
        /// Gets the session id returned by the login command
        /// </summary>
        public string SessionID
        {
            get {
                return sessionResponse == null ? null : sessionResponse.Value<string>("sessionID"); }
        }

        /// <summary>
        /// Gets the ID of the user that is currently logged in
        /// </summary>
        public string UserID
        {
            get {
                return sessionResponse == null ? null : sessionResponse.Value<string>("userID"); }
        }
        /// <summary>
        /// If true, every request sent will print the full URL being requested to the console
        /// </summary>
        public bool DebugUrls
        {
            get {
                return client.DebugUrls; }
            set {
                client.DebugUrls = value; }
        }
        /// <summary>
        /// Creates a new AtTaskRestClient.
        /// </summary>
        /// <param name="apiPath">
        /// The URL to the api of the AtTask API.
        /// For example:
        /// "http://yourcompany.attask-ondemand.com/attask/api"
        /// </param>
        public AtTaskRestClient(string apiPath)
        {
            if (string.IsNullOrEmpty(apiPath)) {
                throw new AtTaskException("apiPath cannot be null or empty");
            }
            if (apiPath.EndsWith("/")) {
                apiPath = apiPath.Substring(0, apiPath.Length - 1);
            }
            this.client = new RestClient(apiPath);
        }
        /// <summary>
        /// Logs in as the given user. <see cref="AtTaskRestExample.AtTaskRestClient"> tracks the session for you so
        /// you do not need to specify the sessionID in the parameters of other commands called by this
        ///<see cref="AtTaskRestExample.AtTaskRestClient">.
        /// Throws an AtTaskException if you are already logged in.
        /// </see>
        ///<param name="username">
        /// A <see cref="System.String"/>
        /// </param>
        ///<param name="password">
        /// A <see cref="System.String"/>
        /// </param>
        /// </see>  </summary>
        public void Login(string username, string password)
        {
            if (IsSignedIn) {
                throw new AtTaskException("Cannot sign in: already signed in.");
            }
            JToken json = client.DoPost("/login", "username=" + username, "password=" + password);
            sessionResponse = json["data"];
        }
        /// <summary>
        /// Clears your current session.
        /// Throws an AtTaskException if you are not logged in.
        /// </summary>
        public void Logout()
        {
            if (!IsSignedIn) {
                throw new AtTaskException("Cannot log out: not signed in.");
            }
            client.DoPost("/logout", "sessionID=" + SessionID);
            sessionResponse = null;
        }
        /// <summary>
        /// If you are logged in, Dispose will attempt to log you out
        /// </summary>
        public void Dispose()
        {
            if (IsSignedIn) {
                this.Logout();
            }
        }
        /// <summary>
        /// Gets the object of the given ObjCode and the given id
        /// </summary>
        /// <param name="objcode">
        /// A <see cref="ObjCode"/> representing the type of object you are getting
        /// </param>
        /// <param name="id">
        /// A <see cref="System.String"/> representing the ID of the object you are getting
        /// </param>
        /// <param name="fieldsToInclude">
        /// The name of the fields to include in the results
        /// </param>
        /// <returns>
        /// A <see cref="JToken"/>
        /// </returns>
        public JToken Get(ObjCode objcode, string id, params string[] fieldsToInclude)
        {
            VerifySignedIn();
            List<string> parameters = new List<string>();
            StringBuilder sb = new StringBuilder();
            if (fieldsToInclude != null && fieldsToInclude.Length > 0) {
                fieldsToInclude.ToList().ForEach(s => sb.Append(s).Append(","));
                sb.Remove(sb.Length - 1, 1);
                string fields = "fields=" + sb.ToString();
                parameters.Add(fields);
            }
            parameters.Add("sessionID=" + SessionID);
            JToken json = client.DoGet(string.Format("/{0}/{1}", objcode, id), parameters.ToArray());
            return json;
        }
        /// <summary>
        /// Searches on the given ObjCode.
        /// </summary>
        /// <param name="objcode">
        /// A <see cref="ObjCode"/>
        /// </param>
        /// <param name="parameters">
        /// A <see cref="System.String[]"/>. Parameters included in the search.
        /// For example:
        /// "name=MyTask"
        /// </param>
        /// <returns>
        /// A <see cref="JToken"/>
        /// </returns>
        public JToken Search(ObjCode objcode, object parameters)
        {
            VerifySignedIn();
            string[] p = parameterObjectToStringArray(parameters, "sessionID=" + SessionID);
            JToken json = client.DoGet(string.Format("/{0}/search", objcode), p);
            return json;
        }

        /// <summary>
        /// Creates a new object of the given type.
        /// </summary>
        /// <param name="objcode">
        /// The <see cref="ObjCode"/> of the object to create
        /// </param>
        /// <param name="parameters">
        /// Additional parameters to be included. Depending on the object type certain parameters are required.
        /// For example, a Task requires a name to be given.
        /// </param>
        /// <returns>
        /// A <see cref="JToken"/>
        /// </returns>
        public JToken Create(ObjCode objcode, object parameters)
        {
            VerifySignedIn();
            string[] p = parameterObjectToStringArray(parameters, "sessionID=" + SessionID);
            JToken json = client.DoPost(string.Format("/{0}", objcode), p);
            return json;
        }
        /// <summary>
        /// Updates an object that already exists.
        /// </summary>
        /// <param name="objcode">
        /// The <see cref="ObjCode"/> of the object to update
        /// </param>
        /// <param name="parameters">
        /// Additional parameters of the object to update.
        /// </param>
        /// <returns>
        /// A <see cref="JToken"/>
        /// </returns>
        public JToken Update(ObjCode objcode, object parameters)
        {
            VerifySignedIn();
            string[] p = parameterObjectToStringArray(parameters, "sessionID=" + SessionID);
            JToken json = client.DoPut(string.Format("/{0}", objcode), p);
            return json;
        }
        /// <summary>
        /// Deletes an object.
        /// </summary>
        /// <param name="objcode">
        /// The <see cref="ObjCode"/> of the object to delete
        /// </param>
        /// <param name="parameters">
        /// A <see cref="System.String[]"/>
        /// </param>
        /// <returns>
        /// A <see cref="JToken"/>
        /// </returns>
        public JToken Delete(ObjCode objcode, object parameters)
        {
            VerifySignedIn();
            string[] p = parameterObjectToStringArray(parameters, "sessionID=" + SessionID);
            JToken json = client.DoDelete(string.Format("/{0}", objcode), p);
            return json;
        }
        /// <summary>
        /// Throws an exception if the client isn't logged in
        /// </summary>
        protected void VerifySignedIn()
        {
            if (!IsSignedIn) {
                throw new AtTaskException("You must be signed in");
            }
        }
        /// <summary>
        /// Converts an <see cref="System.Object"/> to a <see cref="System.String[]"/>.
        /// Reflects on all the property names in the given object
        /// and returns a <see cref="System.String[]"/> representation that can be used
        /// with the <see cref="RestClient">/</see>  </summary>
        private string[] parameterObjectToStringArray(object parameters, params string[] toAdd)
        {
            var properties = parameters.GetType().GetProperties();
            List<string> p = new List<string>(properties.Length);
            p.AddRange(toAdd);
            foreach (var prop in properties) {
                string line = string.Format("{0}={1}", prop.Name, prop.GetValue(parameters, null));
                p.Add(line);
            }
            return p.ToArray();
        }
    }
}
