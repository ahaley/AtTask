 /*
 * Copyright (c) 2011 AtTask, Inc.
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
using System.Linq;

namespace AtTaskRestExample
{
    public class AtTaskRestClientDemo
    {
        static void Main(string[] args)
        {
            string url = "<AtTask URL>";
            using (AtTaskRestClient client = new AtTaskRestClient(url)) {
                client.Login("<AtTask Username>", "<AtTask Password>");
                //				client.DebugUrls = true; //If true, then every URL requested will be printed
                DoDemo(client);
            }
        }
        static void DoDemo(AtTaskRestClient client)
        {
            //Get user object of the user that is signed in
            Console.WriteLine("** Retrieving user...");
            JToken user = client.Get(ObjCode.USER, client.UserID, "homeGroupID");
            string userGroupID = user["data"].Value<string>("homeGroupID");
            Console.WriteLine(user);
            Console.WriteLine("** Done");
            Console.WriteLine();
            //Search all projects
            Console.WriteLine("** Searching projects...");
            JToken projects = client.Search(ObjCode.PROJECT, new { groupID = userGroupID });
            foreach (var j in projects["data"].Children()) {
                Console.WriteLine("Name: {0}", j.Value<string>("name"));
            }
            Console.WriteLine("** Done");
            Console.WriteLine();
            //Create a new project
            Console.WriteLine("** Creating project...");
            JToken project = client.Create(ObjCode.PROJECT, new { name = "OldProjectName", groupID = userGroupID });
            Console.WriteLine(project);
            Console.WriteLine("Done");
            Console.WriteLine();
            //Edit an existing project
            Console.WriteLine("** Editing project...");
            string projectID = project["data"].Value<string>("ID");
            project = client.Update(ObjCode.PROJECT, new { id = projectID, name = "NewProjectName" });
            Console.WriteLine(project);
            Console.WriteLine("Done");
            Console.WriteLine();
            //Add new issues to an existing project
            Console.WriteLine("** Adding issues to project...");
            for (int i = 1; i <= 3; i++) {
                string issueName = "issue " + i.ToString();
                client.Create(ObjCode.ISSUE, new { name = issueName, projectID = projectID });
            }
            project = client.Search(ObjCode.ISSUE, new { projectID = projectID, fields = "projectID" });
            Console.WriteLine(project);
            Console.WriteLine("Done");
            Console.WriteLine("");
            //Delete an existing project
            Console.WriteLine("Deleting project...");
            JToken deleted = client.Delete(ObjCode.PROJECT, new { id = projectID });
            Console.WriteLine(deleted);
            Console.WriteLine("Done");
            Console.WriteLine();
        }
    }
}

