﻿
using MySql.Data.MySqlClient;
using MySQLibrary;
using System.Data;

namespace MySQLlibrary
{
    public class DataBase
    {
        #region Read/Write DB Connection Settings----------------------------

        static readonly string dbConnectionName = "jiraDBConnection";
        private void ReadConnectionParametersFromFile()
        {
            string[] lines = File.ReadAllLines(dbConnectionName);
            Server = lines[0];
            DatabaseName = lines[1];
            UserName = lines[2];
            Password = lines[3];
        }

        public static async void WriteConnectionParametersToFile(string server, string database, string user, string password)
        {
            string[] lines = { server, database, user, password };

            await File.WriteAllLinesAsync(dbConnectionName, lines);
        }

        /// <summary>
        /// Static method for external use
        /// </summary>        
        public static void ReadConnectionParametersFromEvironment(out string server, out string dataBaseName, out string port, out string user, out string password)
        {
            server = dataBaseName = port = user = password = "";
            string? envValue = Environment.GetEnvironmentVariable(dbConnectionName);

            if (envValue is null)
            {
                return;
            }

            Dictionary<string, string> connectionStringParts = envValue.Split(';')
                .Select(t => t.Split(new char[] { '=' }, 2))
                .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase);

            // server=localhost;database=world;port=3306;user=root;password=******
            server = connectionStringParts["server"];
            dataBaseName = connectionStringParts["database"];
            port = connectionStringParts["port"];
            user = connectionStringParts["user"];
            password = connectionStringParts["password"];
        }

        private string? ReadConnectionParametersFromEvironment()
        {
            string? envValue = Environment.GetEnvironmentVariable(dbConnectionName);

            if (envValue is null)
            {
                return null;
            }

            Dictionary<string, string> connectionStringParts = envValue.Split(';')
                .Select(t => t.Split(new char[] { '=' }, 2))
                .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase);

            // server=localhost;database=world;port=3306;user=root;password=******
            Server = connectionStringParts["Server"];
            DatabaseName = connectionStringParts["database"];
            Port = connectionStringParts["port"];
            UserName = connectionStringParts["user"];
            Password = connectionStringParts["password"];

            return envValue;
        }

        /// <summary>
        /// Write connectionString string like server=localhost;database=world;port=3306;user=root;password=******
        /// to Environment 
        /// </summary>
        /// <param name="connectionString"></param>
        public static void WriteConnectionParametersToEvironment(string connectionString)
        {
            Environment.SetEnvironmentVariable(dbConnectionName, connectionString, EnvironmentVariableTarget.User);
        }

        public static void RemoveConnectionParametersFromEvironment()
        {
            Environment.SetEnvironmentVariable(dbConnectionName, null, EnvironmentVariableTarget.User);
        }

        #endregion Read/Write DB Connection Settings

        private static string _connectionStrgin = string.Empty;
        public DataBase()
        {
            try
            {
                _connectionStrgin = ReadConnectionParametersFromEvironment();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public MySqlConnection Connection { get; set; }

        private static DataBase _instance = null;
        public static DataBase Instance()
        {
            if (_instance == null)
            {
                _instance = new DataBase();
            }
            return _instance;
        }

        public bool Connect()
        {
            if (Connection == null || Connection.State == System.Data.ConnectionState.Closed)
            {
                if (String.IsNullOrEmpty(DatabaseName))
                    return false;
                //string connstring = $"Server={Server}; database={DatabaseName}; UID={UserName}; password={Password}";
                Connection = new MySqlConnection(_connectionStrgin);

                try
                {
                    Connection.Open();
                }
                catch (Exception connEx)
                {
                    throw new Exception($"Cannot connect to databese. {connEx.Message}"); ;
                }
            }

            return true;
        }

        public void Close()
        {
            Connection.Close();
            Connection.Dispose();
        }

        public static List<DescriptionModel> SelectIssueDescription()
        {
            List<DescriptionModel> result = new();
            var dbCon = Instance();
            if (dbCon.Connect())
            {
                string query = "SELECT * from jira434.jiraissue WHERE PROJECT = '10156' and DESCRIPTION like '%<%';";
                //string query = "SELECT ID,DESCRIPTION from jira434.jiraissue WHERE PROJECT = '10156' and DESCRIPTION like '%<%';";
                query = "SELECT ID,DESCRIPTION from jira434.jiraissue WHERE PROJECT = '10156' and DESCRIPTION like '%<ul%';";
                //query = "SELECT ID,DESCRIPTION from jira434.jiraissue WHERE PROJECT = '10156' and DESCRIPTION like '%\\'%';";
                //query = "SELECT ID,DESCRIPTION from jira434.jiraissue WHERE PROJECT = '10156' and DESCRIPTION like '%<b>%';";
                //query = "SELECT ID,DESCRIPTION from jira434.jiraissue WHERE PROJECT = '10156' and DESCRIPTION like '%<h%';";
                query = "SELECT ID,DESCRIPTION from jira434.jiraissue WHERE PROJECT = '10156' and ID = 570291;";

                using var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    string description = reader.GetString(reader.GetOrdinal("Description"));

                    var item = new DescriptionModel()
                    {
                        ID = id,
                        Description = description
                    };

                    result.Add(item);
                }
                dbCon.Close();
            }
            return result;
        }

        public static int UpdateDescription(string id, string descriptionText)
        {
            var dbCon = Instance();
            if (dbCon.Connect())
            {
                string query = "UPDATE jira434.jiraissue " +
                                 $"SET `description` = '{descriptionText}' " +
                                 $"WHERE id = {id}";
                using var cmd = new MySqlCommand(query, dbCon.Connection);
                int rowsUpdated = cmd.ExecuteNonQuery();
                dbCon.Close();
                return rowsUpdated;
            }
            return 0;
        }
    }
}