using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for MassAddBooks.xaml
    /// </summary>
    public partial class MassAddBooks : Window
    {
        MySqlConnection LMConn;
        public MassAddBooks(string uid, string pass)
        {
            
            InitializeComponent();
            try
            {
                LMConn = new MySqlConnection(App.ConStr);
                LMConn.Open();
            }
            catch (Exception ex)
            {
                App.ErrorBox(ex.Message);
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            List<string> Acc = new List<string>();
            List<string> BookNo = new List<string>();

            MySqlCommand cmd = new MySqlCommand("select * from Books;", LMConn);
            MySqlDataReader data = cmd.ExecuteReader();
            while (data.Read())
            {
                Acc.Add((string)data["Acc"]);
                BookNo.Add((string)data["BookNumber"]);
            }
            data.Close();

            bool check = true;


            string[] BookInfo = textRange.Text.Replace("\n", "").Split(';');

            foreach (string i in BookInfo)
            {
                App.SuccessBox(i);
                if (!i.Equals(""))
                {
                   
                    try
                    {
                       
                        if (Acc.Contains(i.Split(':')[1]))
                        {
                            App.ErrorBox("A Book with the same Acc number exists");
                            check = false;
                            break;
                        }
                        if (BookNo.Contains(i.Split(':')[2]))
                        {
                            App.ErrorBox("A Book with the same Book number exists");
                            check = false;
                            break;
                        }

                        try
                        {
                            MySqlCommand BookTableInsert = new MySqlCommand("insert into Books(Name,Acc,BookNo)values('" + i.Split(':')[0] + "','" + i.Split(':')[1] +"','"+i.Split(':')[2]+ "');", LMConn);
                            BookTableInsert.ExecuteNonQuery();
                            
                        }
                        catch (Exception ex)
                        {
                            App.ErrorBox(ex.Message);
                        }

                    }
                    catch 
                    {
                        App.ErrorBox("The format has some errors, please check it");
                        break;
                    }

                }
            }
            if (check)
            {
                App.SuccessBox("Operation Completed succesfully! Bookss have been added");
            }
        }
    }
}
