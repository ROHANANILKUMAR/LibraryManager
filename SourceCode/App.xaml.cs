using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MySqlConnection conn;
        public static string Password;
        public App()
        {
                Home = @"C:\Program Files (x86)\LibraryManagerV2\LibraryManagerV2\Data\";
        }
        
        public static string[] GetUIDPASS()
        {
            string uid = "";
            string pass = "";
            string port = "";
            string Read = File.ReadAllText(App.Home + "Connection.Lib");
            uid = Encryption.Decrypt(Read.Split(':')[0]);
            pass = Encryption.Decrypt(Read.Split(':')[1]);
            port= Encryption.Decrypt(Read.Split(':')[2]);

            return new string[] { uid, pass, port };
        }

        public static string Home;
        public static bool NewClassAdded;
        public static bool NewStudentAdded;
        public static bool FineChanged;
        
        public static string ConStr;

        public static void ErrorBox(string contend)
        {
            MessageBox.Show(contend, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void SuccessBox(string contend)
        {
            MessageBox.Show(contend, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static bool Ask(string contend,string Caption)
        {
            if(MessageBox.Show(contend, Caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        public List<string> SortInt(List<string> list,char separator,int index)
        {
            List<string> returnList = new List<string>();
            Dictionary<string, string> TempData = new Dictionary<string, string>();
            List<int> indexlist= new List<int>(); 
            foreach(string i in list)
            {
                TempData.Add(i.Split(separator)[index], i);
                indexlist.Add(int.Parse(i.Split(separator)[index]));
            }

            indexlist.Sort();

            foreach (int i in indexlist)
            {
                returnList.Add(TempData[Convert.ToString(i)]);
            }

            return returnList;
        }
    }
    
}
