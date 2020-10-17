using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;

namespace LibraryManager
{
    class SQL
    {
        string uid;
        string pass;

        MySqlConnection LMconn;

        public SQL()
        {
            uid = App.GetUIDPASS()[0];
            pass = App.GetUIDPASS()[1];

            LMconn = new MySqlConnection(App.ConStr);

            LMconn.Open();
        }

        public List<string> GetClasses()
        {
            List<string> classes = new List<string>();

            List<Dictionary<string, string>> ClassList = GetDataList("Classes", new string[] { "Class" });

            foreach(Dictionary<string,string>i in ClassList)
            {
                classes.Add(i["Class"]);
            }

            return classes;
        }

        public Dictionary<string, Dictionary<string, string>> GetData(string tableName, string FindBy, string[] parameters)
        {
            try
            {
                Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();
                MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0}", tableName), LMconn);
                MySqlDataReader data = cmd.ExecuteReader();
                Dictionary<string, string> inCompleteData = new Dictionary<string, string>();
                while (data.Read())
                {
                    foreach (string i in parameters)
                    {
                        inCompleteData[i] = (string)data[i];
                    }
                    Data[(string)data[FindBy]] = (inCompleteData);
                    inCompleteData.Clear();
                }
                data.Close();
                return Data;
            }
            catch
            {
                return null;
            }

        }

        public Dictionary<string, string> GetSingleColumn(string tableName, string FindBy,string FindColumn, string[] parameters)
        {
            try
            {
                Dictionary<string, string> Data = new Dictionary<string, string>();
                MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0} where {1} = '{2}'", tableName, FindBy, FindColumn), LMconn);
                //App.SuccessBox(string.Format("select * from {0} where {1} = '{2}'", tableName, FindBy, FindColumn));
                MySqlDataReader data = cmd.ExecuteReader();
                while (data.Read())
                {
                    foreach (string i in parameters)
                    {
                        Data[i] = (string)data[i];
                    }

                }
                data.Close();
                return Data;
            }
            catch
            {
                return null;
            }
            
        }

        public Dictionary<string, Dictionary<string, string>> GetDataDict(string tableName ,string FindBy,string FindData ,string[] parameters)
        {
            try
            {
                Dictionary<string, Dictionary<string, string>> Data = new Dictionary<string, Dictionary<string, string>>();
                MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0} where {1} ={2}", tableName, FindBy, FindData), LMconn);
                MySqlDataReader data = cmd.ExecuteReader();
                Dictionary<string, string> inCompleteData = new Dictionary<string, string>();
                while (data.Read())
                {
                    foreach (string i in parameters)
                    {
                        inCompleteData[i] = (string)data[i];
                    }
                    Data[(string)data[FindBy]] = (inCompleteData);
                    inCompleteData.Clear();
                }
                data.Close();
                return Data;
            }
            catch
            {
                return null;
            }
            
        }

        public List<Dictionary<string, string>> GetDataList(string tableName, string FindBy, string FindData, string[] parameters)
        {
            try
            {
                List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();
                MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0} where {1}= '{2}';", tableName, FindBy, FindData), LMconn);
                //MySqlCommand cmd = new MySqlCommand("select * from students where class='XI';", LMconn);
                //MySqlCommand cmd = new MySqlCommand("Select * from Students where Class='" +"XI" + "' order by Roll asc;", LMconn);
                MySqlDataReader data = cmd.ExecuteReader();



                while (data.Read())
                {
                    Dictionary<string, string> inCompleteData = new Dictionary<string, string>();
                    foreach (string i in parameters)
                    {
                        inCompleteData[i] = (string)data[i];
                    }
                    Data.Add(inCompleteData);

                }
                data.Close();
                return Data;
            }
            catch
            {
                return null;
            }
        }

        public List<string> GetDataList(string tableName, string parameters)
        {
            try
            {
                List<string> Data = new List<string>();
                MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0};", tableName), LMconn);
                //MySqlCommand cmd = new MySqlCommand("select * from students where class='XI';", LMconn);
                //MySqlCommand cmd = new MySqlCommand("Select * from Students where Class='" +"XI" + "' order by Roll asc;", LMconn);
                MySqlDataReader data = cmd.ExecuteReader();



                while (data.Read())
                {
                    Dictionary<string, string> inCompleteData = new Dictionary<string, string>();
                    
                    Data.Add((string)data[parameters]);
                  
                }
                data.Close();
                return Data;
            }
            catch
            {
                return null;
            }
        }

        public List<Dictionary<string, string>> GetDataList(string tableName, string FindBy, string FindData, string OrderBy, string[] parameters)
        {
            try
            {
                List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();
                MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0} where {1}= '{2}' order by {3};", tableName, FindBy, FindData, OrderBy), LMconn);
                //MySqlCommand cmd = new MySqlCommand("select * from students where class='XI';", LMconn);
                //MySqlCommand cmd = new MySqlCommand("Select * from Students where Class='" +"XI" + "' order by Roll asc;", LMconn);
                MySqlDataReader data = cmd.ExecuteReader();



                while (data.Read())
                {
                    Dictionary<string, string> inCompleteData = new Dictionary<string, string>();
                    foreach (string i in parameters)
                    {
                        inCompleteData[i] = (string)data[i];
                    }
                    Data.Add(inCompleteData);

                }
                data.Close();
                return Data;
            }
            catch
            {
                return null;
            }
        }

        public List<Dictionary<string, string>> GetDataList(string tableName,string[] parameters)
        {
            try
            {
                List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();
                MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0};", tableName), LMconn);
                //MySqlCommand cmd = new MySqlCommand("select * from students where class='XI';", LMconn);
                //MySqlCommand cmd = new MySqlCommand("Select * from Students where Class='" +"XI" + "' order by Roll asc;", LMconn);
                MySqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Dictionary<string, string> inCompleteData = new Dictionary<string, string>();
                    foreach (string i in parameters)
                    {
                        inCompleteData[i] = (string)data[i];
                    }
                    Data.Add(inCompleteData);

                }
                data.Close();
                return Data;
            }
            catch
            {
                return null;
            }
        }


        public List<Dictionary<string, string>> GetDataListUsingLike(string tableName, string FindBy, string FindData, string OrderBy,string ascdesc,string[] parameters)
            {
                try
                {
                    List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();
                    MySqlCommand cmd = new MySqlCommand(string.Format("select * from {0} where {1} like  '{2}' order by {3};", tableName, FindBy, "%"+FindData+"%", OrderBy), LMconn);
                    //MySqlCommand cmd = new MySqlCommand("select * from students where class='XI';", LMconn);
                    //MySqlCommand cmd = new MySqlCommand("Select * from Students where Class='" +"XI" + "' order by Roll asc;", LMconn);
                    MySqlDataReader data = cmd.ExecuteReader();



                    while (data.Read())
                    {
                        Dictionary<string, string> inCompleteData = new Dictionary<string, string>();
                        foreach (string i in parameters)
                        {
                            inCompleteData[i] = (string)data[i];
                        }
                        Data.Add(inCompleteData);

                    }
                    data.Close();
                    return Data;
                }
                catch
                {
                    return null;
                }

            }

        public DataTable GetDataTable(string table,string FindBy,string Find,string[]columns)
        {
            DataTable dt = new DataTable();
            MySqlDataAdapter DataA = new MySqlDataAdapter(string.Format("select {0} from {1} where {2}='{3}';",string.Join(",",columns),table,FindBy,Find), LMconn);
            DataA.Fill(dt);

            return dt;
        }

        public void InsertData(string TableName,string[] Parameters,string[] Values)
        {
            MySqlCommand insertcmd = new MySqlCommand(string.Format("insert into {0}({1})values({2});",TableName, string.Join(",", Parameters), "'"+string.Join("','",Values)+"'"), LMconn);
            insertcmd.ExecuteNonQuery();
        }

        public void DeleteData(string TableName, string FindBy,string Find)
        {
            MySqlCommand Delcmd = new MySqlCommand(string.Format("Delete from {0} where {1} = '{2}'", TableName, FindBy,Find), LMconn);
            Delcmd.ExecuteNonQuery();
        }

        public void UpdateData(string TableName, string SetColumn, string SetData,string FindBy,string Find)
        {
            MySqlCommand Updatecmd = new MySqlCommand(string.Format("update {0} set {1} = '{2}' where {3}='{4}'", TableName,SetColumn,SetData, FindBy, Find), LMconn);
            Updatecmd.ExecuteNonQuery();
        }

        public void UpdateMultipleData(string TableName, string[] SetColumns,string[] SetColumnsData,string FindBy,string FindData)
        {
            StringBuilder s = new StringBuilder(string.Format("update {0} set ", TableName));
            for(int i = 0; i < SetColumns.Length; i++)
            {
                if (i == SetColumns.Length - 1)
                {
                    s.Append(string.Format("{0}='{1}' ", SetColumns[i], SetColumnsData[i]));
                }
                else
                {
                    s.Append(string.Format("{0}='{1}', ", SetColumns[i], SetColumnsData[i]));
                }
            }
            s.Append(string.Format("where {0}='{1}'", FindBy, FindData));

            MySqlCommand Updatecmd = new MySqlCommand(s.ToString(), LMconn);
            Updatecmd.ExecuteNonQuery();
        }

        public void UpdateMultipleData(string TableName, string[] SetColumns, string[] SetColumnsData, string[] FindBy, string[] FindData)
        {
            StringBuilder s = new StringBuilder(string.Format("update {0} set ", TableName));
            for (int i = 0; i < SetColumns.Length; i++)
            {
                if (i == SetColumns.Length - 1)
                {
                    s.Append(string.Format("{0}='{1}' ", SetColumns[i], SetColumnsData[i]));
                }
                else
                {
                    s.Append(string.Format("{0}='{1}', ", SetColumns[i], SetColumnsData[i]));
                }
            }
            s.Append("where ");
            for (int i = 0; i < FindData.Length; i++)
            {
                if (i == FindData.Length - 1)
                {
                    s.Append(string.Format("{0}='{1}';", FindBy[i], FindData[i]));
                }
                else
                {
                    s.Append(string.Format("{0}='{1}' && ", FindBy[i], FindData[i]));
                }
                
            }
            MySqlCommand Updatecmd = new MySqlCommand(s.ToString(), LMconn);
            Updatecmd.ExecuteNonQuery();
        }

        public string GetPassword()
        {
            string password=null;
            MySqlCommand ReadPasswordcmd = new MySqlCommand("select * from passwords;", LMconn);
            MySqlDataReader PasswordData = ReadPasswordcmd.ExecuteReader();
            while (PasswordData.Read())
            {
                password = (string)PasswordData["Password"];
            }
            PasswordData.Close();
            return password;
        }

        public void Close()
        {
            LMconn.Close();
        }
    }
}
