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
using MySql.Data.MySqlClient;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for ConnectionWork.xaml
    /// </summary>
    public partial class ConnectionWork : Window
    {
        public ConnectionWork()
        {
            InitializeComponent();
            Username.Text = "root";
            Port.Text = "3306";
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            App.ConStr = "server=localhost;Database=Librarymanager;uid=" + Username.Text + ";password=" + Password.Text + ";Port=" + Port.Text;

            try
            {
                
                MySqlConnection tempconn = new MySqlConnection("server=localhost;uid=" + Username.Text + ";password=" + Password.Text + ";Port=" + Port.Text);
                tempconn.Open();
                MySqlCommand tempcmd = new MySqlCommand("Create database LibraryManager;", tempconn);
                tempcmd.ExecuteNonQuery();
                tempconn.Close();

               

                MySqlConnection conn = new MySqlConnection(App.ConStr);
                conn.Open();
                File.WriteAllText(App.Home + "Connection.Lib", Encryption.Encrypt(Username.Text) + ":" + Encryption.Encrypt(Password.Text)+":"+Encryption.Encrypt(Port.Text));

               

                MySqlCommand CreateAllTables = new MySqlCommand("Create table Fine(AdmissionNumber varchar(10),Acc varchar(10),Amount varchar(10));Create table BookStudentData(Acc varchar(10),StudentName varchar(50),AdmissionNumber varchar(50),DateOfIssue varchar(20),DateOfReturn varchar(20));Create table StudentBookData(AdmissionNumber varchar(50),BookName varchar(50),Acc varchar(10),BookNo varchar(10),DateOfIssue varchar(20),DateOfReturn varchar(20));Create table CurrentBooks(AdmissionNumber varchar(10),Acc varchar(10),DateOfIssue varchar(20));Create table Passwords(Password varchar(50));Create table Books(Name varchar(50),Acc varchar(10),BookNo varchar(10));Create table Classes(Class varchar(10));Create table Students(Name varchar(50),AdmissionNumber varchar(10), Class varchar(10),Roll varchar(10));insert into Passwords(Password)values('PfuRJEKbzEM=');", conn);
                CreateAllTables.ExecuteNonQuery();

                
            }
            catch (MySqlException ex)
            {
                
                if (ex.Message.Equals("Can't create database 'librarymanager'; database exists"))
                {

                    MySqlConnection conn = new MySqlConnection(App.ConStr);
                    conn.Open();
                    File.WriteAllText(App.Home + "Connection.Lib", Encryption.Encrypt(Username.Text) + ":" + Encryption.Encrypt(Password.Text)+":"+Encryption.Encrypt(Port.Text));

                    MySqlCommand ClassModify = new MySqlCommand("alter table classes modify column class varchar(10); ",conn);
                    ClassModify.ExecuteNonQuery();

                    MySqlCommand cmd = new MySqlCommand("select * from Passwords;", conn);
                    MySqlDataReader data = cmd.ExecuteReader();

                    

                    while (data.Read())
                    {
                        App.Password = (string)data["Password"];
                    }

                    conn.Close();
                    
                    App.SuccessBox("Connected");
                    this.Hide();
                    MainWindow MN = new MainWindow();
                    MN.Show();
                }
                else
                {
                    App.ErrorBox(ex.Message);
                }
            }
            
            
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Connect_Click(this, new RoutedEventArgs());
            }
        }
    }
}
