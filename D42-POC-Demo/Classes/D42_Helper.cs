using D42_POC_Demo.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using System.Xml;

namespace D42_POC_Demo
{
   
    public class D42_Helper : Wizard_Helper
    {
        private const string DEFAULT_XML_CONFIG_NAME = "config.xml";
        private const string DEFAULT_CUSTOM_DOQL_PATH = "Custom_Saved_DOQL";
        private const string DEFAULT_DOQL_PATH = "D42_POC_DOQL_READ_ONLY";
        private const string DEFAULT_REPORT_PATH = "D42_WIZ_REPORTS";
        private const string DEFAULT_DOQL_ZIP_NAME = "d42_poc_wizard_doql_tmp.zip";
        private const string DEFAULT_DOQL_GITHUB = "https://github.com/M6thrXeat/DOQL_scripts_examples/archive/master.zip";
        private const string DEFAULT_DATA_DICT_NAME = "DataDictionary.xml";
        private string csvResponseString;
        private HttpClientHandler httpClientHandler;
        private HttpClient client;
        private readonly Dictionary<string, string> _XML_CONFIG;
        private string _status;
        private int _https;
        private int _ssl;

        public  D42_Helper()
        {
            Username = "";
            Password = new SecureString();
            _XML_CONFIG = new Dictionary<string, string>();
            DataDictionary = new Dictionary<string, List<Column>>();

            //<DIRECTORY CHECKS>
            if (Directory.Exists(DEFAULT_DOQL_PATH))
                ImportDoqlFiles(DEFAULT_DOQL_PATH);
            else
                Directory.CreateDirectory(DEFAULT_DOQL_PATH);

            if (Directory.Exists(DEFAULT_CUSTOM_DOQL_PATH))
                ImportDoqlFiles(DEFAULT_CUSTOM_DOQL_PATH);
            else
                Directory.CreateDirectory(DEFAULT_CUSTOM_DOQL_PATH);

            if (Directory.Exists(DEFAULT_REPORT_PATH))
            {
                //DO NOTHING
            }
            else
                Directory.CreateDirectory(DEFAULT_REPORT_PATH);
            //</DIRECTORY CHECKS>

            SortDoqlQueries();
            LoadDataDictionary(DEFAULT_DATA_DICT_NAME);
            LoadXMLConfigFile(DEFAULT_XML_CONFIG_NAME);
            SetSSL(_ssl == 0);
        }

        //<PROPERTIES>
        public Dictionary<string, List<Column>> DataDictionary { get; }
        public DataTable DoqlDataTable { get; private set; } = new DataTable();
        public string Status { get => $"{DateTime.Now.ToLongTimeString()} | {Username} | {Url} | {_status}"; private set => _status = value; } 
        public string Url { get; set; }
        public string Username { get; set; }
        public int Https { get => _https; set => _https = value; }
        public int Ssl { get => _ssl; set => _ssl = value; }
        public SecureString Password { get; set; }
        public List<KeyValuePair<string, string>> DOQL_QUERIES { get;} = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("advanced_discovery_scores_RO", resources.advanced_discovery_scores_RO),
            new KeyValuePair<string, string>("all_devices_RO", resources.all_devices_RO),
            new KeyValuePair<string, string>("app_comps_with_service_communication_RO", resources.app_comps_with_service_communication_RO),
            new KeyValuePair<string, string>("app_comp_and_software_RO", resources.app_comp_and_software_RO),
            new KeyValuePair<string, string>("connections_in_past_60_days_RO", resources.connections_in_past_60_days_RO),
            new KeyValuePair<string, string>("connections_in_past_90_days_RO", resources.connections_in_past_90_days_RO),
            new KeyValuePair<string, string>("count_for_detailed_discovery_data_RO", resources.count_for_detailed_discovery_data_RO),
            new KeyValuePair<string, string>("DevicesByAffinityGroup_RO", resources.DevicesByAffinityGroup_RO),
            new KeyValuePair<string, string>("Discovery_Scores_RO", resources.Discovery_Scores_RO),
            new KeyValuePair<string, string>("ignore_hidden_services_RO", resources.ignore_hidden_services_RO),
            new KeyValuePair<string, string>("Impact_List_enh_RO", resources.Impact_List_enh_RO),
            new KeyValuePair<string, string>("include_ipv6_connections_RO", resources.include_ipv6_connections_RO),
            new KeyValuePair<string, string>("no_client_os_devices_RO", resources.no_client_os_devices_RO),
            new KeyValuePair<string, string>("resource_utilization_RO", resources.resource_utilization_RO),
            new KeyValuePair<string, string>("service_communication_RO", resources.service_communication_RO),
            new KeyValuePair<string, string>("storage-totals_RO", resources.storage_totals_RO),
            new KeyValuePair<string, string>("tagged_devices_only_RO", resources.tagged_devices_only_RO),
            new KeyValuePair<string, string>("unused_servers_RO", resources.unused_servers_RO),
        };
        //</PROPERTIES>

        //<PRIVATE METHODS>
        //Sort the list of DOQL queries by query name
        private void SortDoqlQueries()
        {
            DOQL_QUERIES.Sort((x, y) => x.Key.CompareTo(y.Key));
        }
        //Disables or enables certificate validation check
        private void SetSSL(bool sslEnabled)
        {
            httpClientHandler = new HttpClientHandler();
            if (!sslEnabled)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            }
            else
            {
                //Do nothing
            }
            client = new HttpClient(httpClientHandler);
        }
        //Loads the XML Data Dictionary file
        private void LoadDataDictionary(string fileName)
        {
            if(!string.IsNullOrEmpty(fileName) && File.Exists(Path.GetFullPath(fileName)))
            {
                XmlDocument objDoc = new XmlDocument();
                objDoc.Load(fileName);

                foreach (XmlNode node in objDoc.SelectNodes("root/element"))
                {
                    string viewName = node.SelectNodes("view").Item(0).InnerText;
                    List<Column> columns = new List<Column>();
                    foreach (XmlNode n in node.SelectNodes("columns/element"))
                    {
                        Column column = new Column();
                        column.name = n.SelectNodes("column").Item(0).InnerText;
                        column.type = n.SelectNodes("data_type").Item(0).InnerText;
                        column.descr = n.SelectNodes("desciption").Item(0).InnerText;
                        columns.Add(column);
                    }
                    DataDictionary.Add(viewName, columns);
                }
            }
        }
        //Loads the XML config file
        private void LoadXMLConfigFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && File.Exists(Path.GetFullPath(fileName)))
            {
                XmlDocument objDoc = new XmlDocument();
                objDoc.Load(fileName);

                foreach (XmlNode node in objDoc.SelectNodes("appSettings/add"))
                {
                    string key = node.Attributes.GetNamedItem("key").Value;
                    string value = node.Attributes.GetNamedItem("value").Value;
                    _XML_CONFIG.Add(key, value);
                }

                int.TryParse(_XML_CONFIG["Default_Https"], out _https);
                int.TryParse(_XML_CONFIG["Default_SSL"], out _ssl);
                Url = _XML_CONFIG["Default_URL"];
                Username = _XML_CONFIG["Default_Username"];
                string pw = _XML_CONFIG["Default_Password"];
                Password = new SecureString();
                foreach (char c in pw)
                {
                    Password.AppendChar(c);
                }
            }
            else
            {
                _https = 0;
                _ssl = 0;
                Url = "";
                Username = "";
                Password = new SecureString();
            }
        }
        //<Helper methods for UpdateDOQL>
        //Downloads DOQL zip file from github
        private async Task GetDOQL(string path, string fileName)
        {
            client.DefaultRequestHeaders.Clear();
            HttpResponseMessage response = await client.GetAsync(path);
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var fileInfo = new FileInfo(fileName);
                using (var fileStream = fileInfo.OpenWrite())
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
        //Extracts the DOQL zip file to the default DOQL directory and imports them to the DOQL queries list
        private void ExtractDoql(string fileName)
        {
            ExtractFilesToDirectory(fileName, DEFAULT_DOQL_PATH, ".sql", "_RO");
            ImportDoqlFiles(DEFAULT_DOQL_PATH);
        }
        //Checks to see if the filename exists in the list, if it does it updates the value. Adds to the list if not.
        private void ImportDoqlFiles(string directory)
        {
            AddDirectoryFileContentToList(directory, DOQL_QUERIES);
            SortDoqlQueries();
        }
        //</Helper methods for UpdateDOQL>
        //</PRIVATE METHODS>

        //<PUBLIC METHODS>
        //Gets latest DOQL from github, extracts only .SQL files, and updates DOQL Queries list
        public async Task UpdateDoql()
        {
            await GetDOQL(DEFAULT_DOQL_GITHUB, DEFAULT_DOQL_ZIP_NAME);
            ExtractDoql(DEFAULT_DOQL_ZIP_NAME);
        }
        //Updates the config
        public void UpdateConfig(string username, SecureString password, string url, int httpsIndex, int sslIndex)
        {
            Username = username;
            Password = password;
            Url = url;
            _https = httpsIndex;
            _ssl = sslIndex;
            if (_ssl == 0)
                SetSSL(true);
            else
                SetSSL(false);
        }
        //Saves DOQL Results httpresponse contents to csv
        public void SaveDoqlResultsAsCSV(string fileName)
        {
            SaveTextToCSVFile(csvResponseString, fileName, DEFAULT_REPORT_PATH);
        }
        //Saves a .sql DOQL query to the custom DOQL folder and adds to the list of queries
        public void SaveDoqlQuery(string query, string fileName)
        {
            SaveTextToFIle(query, fileName, DEFAULT_CUSTOM_DOQL_PATH, ".sql");
            AddFileContentToList($"{ DEFAULT_CUSTOM_DOQL_PATH}\\{fileName}", DOQL_QUERIES);
            SortDoqlQueries();
        }
        //Saves a DOQL query to the D42 instance
        public async Task PostDoqlQUery(string query, string queryName)
        {
            if (Https == 0 || Https == 1)
            {
                if (!string.IsNullOrWhiteSpace(query) && !string.IsNullOrWhiteSpace(queryName))
                {
                    try
                    {
                        Status = $"Sending http request to: {Url}...";
                        HttpResponseMessage response = await D42_API.PostSavedDoqlQuery(Username, ToUnsecureString(Password), Https == 0, Url, client, queryName, query);
                        if (response.IsSuccessStatusCode)
                        {
                            Status = $"Http response: {response.StatusCode}\t";
                        }
                        else
                        {
                            Status = $"Http response: {response.StatusCode}\t";
                        }
                    }
                    catch(Exception ex)
                    {
                        Status = ex.Message;
                    }
                }
            }
        }
        //Queries the D42 instance with the supplied DOQL query
        public async Task QueryD42(string query)
        {
            if (!string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(query))
            {
                if (_https >= 0 && _https < D42_API.SSL_CHOICES.Length)
                {
                    try
                    {
                        Status = $"Sending http request to: {Url}...";
                        HttpResponseMessage response = await D42_API.PostDoqlQuery(Username, ToUnsecureString(Password), Https == 0, Url, client, query);
                        if (response.IsSuccessStatusCode)
                        {
                            csvResponseString = await response.Content.ReadAsStringAsync();
                            DoqlDataTable = CsvReader.GenerateDataTable(csvResponseString, true);
                            Status = $"Http response: {response.StatusCode}\t";
                        }
                        else
                        {
                            Status = $"Http response: {response.StatusCode}\t";
                        }
                    }
                    catch(Exception ex)
                    {
                        Status = ex.Message;
                    }
                }
            }
        }
        public async Task CreateDiscoveryJob(Dictionary<string,string> payload)
        {
            if (!string.IsNullOrWhiteSpace(Url))
            {
                if (_https >= 0 && _https < D42_API.SSL_CHOICES.Length)
                {
                    try
                    {
                        Status = $"Sending http request to: {Url}...";
                        HttpResponseMessage response = await D42_API.PostVMwareJob(Username, ToUnsecureString(Password), Https == 0, Url, client, payload);
                        if (response.IsSuccessStatusCode)
                        {
                            Status = $"Http response: {response.StatusCode}\t";
                        }
                        else
                        {
                            Status = $"Http response: {response.StatusCode}\t";
                        }
                    }
                    catch (Exception ex)
                    {
                        Status = ex.Message;
                    }
                }
                else
                {
                    Status = "Error: Https out of range";
                }
            }
            else
            {
                Status = "Error: URL is null";
            }
        }
        //</PUBLIC METHODS>
    }
}
