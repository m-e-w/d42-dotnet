using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;

namespace D42_POC_Demo.Classes
{
    public static class D42_API 
    {
        private static readonly string DOQL_ENDPOINT = "/services/data/v1.0/query/";
        private static readonly string SAVED_DOQL_ENDPOINT = "/api/1.0/saved_doql_queries/";
        private static readonly string VSERVER_ENDPOINT = "/api/1.0/auto_discovery/vserver/";
        public static readonly string[] HTTP_CHOICES = {
            "https://",
            "http://"
        };
        public static readonly string[] SSL_CHOICES  = {
            "True",
            "False"
        };
        public static Dictionary<string, string> vServerProperties = new Dictionary<string, string>()
        {
            { "name", "" }
        };

        private static async Task<HttpResponseMessage> Post(string user, string password, bool httpsTrue,  string path, string endpoint, HttpClient client, Dictionary<string, string> payload)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{user}:{password}")));
            FormUrlEncodedContent content = new FormUrlEncodedContent(payload);
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            if(httpsTrue)
            {
                path = $"{HTTP_CHOICES[0]}{path}";
            }
            else
            {
                path = $"{HTTP_CHOICES[1]}{path}";
            }
            return await client.PostAsync($"{path}{endpoint}", content);
        }
        public static async Task<HttpResponseMessage> PostSavedDoqlQuery(string user, string password, bool httpsTrue, string path, HttpClient client, string queryName, string query)
        {
            queryName = queryName.Replace(" ", "_");
            Dictionary<string, string> payload = new Dictionary<string, string>
            {
                {"name", $"{queryName}"},
                {"saved_query", $"{query}"}
            };
            return await Post(user, password, httpsTrue, path, SAVED_DOQL_ENDPOINT, client, payload);
        }

        public static async Task<HttpResponseMessage> PostDoqlQuery(string user, string password, bool httpsTrue, string path, HttpClient client, string query)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>
            {
                { "query", $"{query}"},
                { "header", "yes" }
            };
            return await Post(user, password, httpsTrue, path, DOQL_ENDPOINT, client, payload);
        }

        public static async Task<HttpResponseMessage> PostVMwareJob(string user, string password, bool httpsTrue, string path, HttpClient client, Dictionary<string,string> payload)
        {
            payload.Add("platform", "vmware");
            //var lines = payload.Select(kvp => kvp.Key + ": " + kvp.Value.ToString());
            //MessageBox.Show(string.Join(Environment.NewLine, lines));
            return await Post(user, password, httpsTrue, path, VSERVER_ENDPOINT, client, payload);
        }
    }
}
