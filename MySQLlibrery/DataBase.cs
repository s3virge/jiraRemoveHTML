
using MySql.Data.MySqlClient;
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
                Connection.Open();
            }

            return true;
        }

        public void Close()
        {
            Connection.Close();
            Connection.Dispose();
            //Connection = null;
        }

        //public static List<DataGridItem> SelectAllCategories()
        //{
        //    List<DataGridItem> result = new List<DataGridItem>();
        //    var dbCon = Instance();
        //    if (dbCon.Connect())
        //    {
        //        //suppose col0 and col1 are defined as VARCHAR in the DB
        //        string query = "SELECT * FROM akvasouz.as_jshopping_categories";
        //        var cmd = new MySqlCommand(query, dbCon.Connection);
        //        var reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            // All as_jshopping_categories table columns names
        //            //category_id, category_image, category_parent_id, category_publish, category_template, ordering, category_add_date,
        //            //products_page, products_row, access,
        //            //name_en-GB, alias_en-GB, short_description_en-GB, description_en-GB, meta_title_en-GB, meta_description_en-GB, meta_keyword_en-GB,
        //            //(17)name_ru-RU, alias_ru-RU, short_description_ru-RU, description_ru-RU, meta_title_ru-RU, meta_description_ru-RU, meta_keyword_ru-RU,
        //            //category_ordertype,
        //            //(25)name_uk-UA, alias_uk-UA, short_description_uk-UA, description_uk-UA, meta_title_uk-UA, meta_description_uk-UA, meta_keyword_uk-UA
        //            string id = reader.GetString(0);
        //            string NameRu = reader.GetString(17);
        //            string aliasRu = reader.GetString(reader.GetOrdinal("alias_ru-RU"));
        //            string short_description_ru = reader.GetString(19);
        //            string description_ru = reader.GetString(20);
        //            string meta_title_ru = reader.GetString(21);
        //            string meta_description_ru = reader.GetString(22);
        //            string meta_keyword_ru = reader.GetString(23);

        //            string name_uk = reader.GetString(25);
        //            string alias_uk = reader.GetString(26);
        //            string short_description_uk = reader.GetString(27);
        //            string description_uk = reader.GetString(28);
        //            string meta_title_uk = reader.GetString(29);
        //            string meta_description_uk = reader.GetString(30);
        //            string meta_keyword_uk = reader.GetString(31);

        //            var catItem = new DataGridItem()
        //            {
        //                Category_ID = id,
        //                Name_ru = NameRu,
        //                Alias_ru = aliasRu,
        //                Short_description_ru = short_description_ru,
        //                Description_ru = description_ru,
        //                Meta_title_ru = meta_title_ru,
        //                Meta_description_ru = meta_description_ru,
        //                Meta_keyword_ru = meta_keyword_ru,
        //                Name_UK = name_uk,
        //                Alias_UK = alias_uk,
        //                Short_description_UK = short_description_uk,
        //                Description_UK = description_uk,
        //                Meta_title_UK = meta_title_uk,
        //                Meta_description_UK = meta_description_uk,
        //                Meta_keyword_UK = meta_keyword_uk
        //            };

        //            result.Add(catItem);
        //        }
        //        dbCon.Close();
        //    }
        //    return result;
        //}
    }
}