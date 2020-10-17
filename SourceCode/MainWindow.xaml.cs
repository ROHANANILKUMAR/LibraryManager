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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.IO;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection conn;
        public MainWindow()
        {
            InitializeComponent();
           
            try
            {
                string TimeRead = File.ReadAllText(App.Home + "Login.Lib");
                string ReadS = File.ReadAllText(App.Home + "Settings.Lib");
                ReadS = Encryption.Decrypt(ReadS);
                state = ReadS.Split(':')[0];
                ReadWA = Convert.ToInt32(ReadS.Split(':')[1]);
                ReadTime = Convert.ToInt32(ReadS.Split(':')[2]);


                string uid = "";
                string pass = "";
                string Port = "";
                string Read = File.ReadAllText(App.Home + "Connection.Lib");
                if (Read.Length > 0)
                {
                    uid = Encryption.Decrypt(Read.Split(':')[0]);
                    pass = Encryption.Decrypt(Read.Split(':')[1]);
                    Port= Encryption.Decrypt(Read.Split(':')[2]);
                    App.ConStr = "server=localhost;database=LibraryManager;uid=" + uid + ";password=" + pass + ";port=" + Port;
                    conn = new MySqlConnection(App.ConStr);
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from Passwords;", conn);
                    MySqlDataReader data = cmd.ExecuteReader();
                    while (data.Read())
                    {
                        App.Password = (string)data["Password"];
                    }

                }
                else
                {
                    this.Hide();
                    ConnectionWork CW = new ConnectionWork();
                    CW.Show();
                }

                if (!TimeRead.Equals(""))
                {
                    currentTime = Convert.ToInt32(Encryption.Decrypt(TimeRead));
                    Login.Visibility = Visibility.Hidden;
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();
                }
            }
            catch(Exception ex)
            {
                if(ex.Message.Contains("Authentication to host"))
                {
                    ConnectionWork CW = new ConnectionWork();
                    CW.Show();
                    this.Hide();
                }
                else
                {
                    App.ErrorBox(ex.Message + " login error");
                }
                
            }
            LoginPass.Focus();
        }

        static string Password;
        static string state; 
        static int WrongAttempts=0;
        static int ReadWA;
        static int ReadTime;
        static int currentTime=0;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        private void TryLogin()
        {
            Password = App.Password;
            if (LoginPass.Password == Encryption.Decrypt(Password))
            {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Hide();
                Main m = new Main();
                m.Show();
            }
            else
            {
                App.ErrorBox("Wrong Password!");
                if (state.Equals("ON"))
                {
                    WrongAttempts += 1;
                    if (WrongAttempts == ReadWA)
                    {
                        string gr;
                        if (ReadTime == 1)
                        {
                            gr = " minute";
                        }
                        else
                        {
                            gr = " minutes";
                        }
                        App.ErrorBox("App is locked for " + Convert.ToString(ReadTime) + gr);
                        Login.Visibility = Visibility.Hidden;
                        dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                        dispatcherTimer.Start();
                    }
                }
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            TryLogin();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs E)
        {
            if (currentTime != ReadTime * 60)
            {
                currentTime += 1;
                File.WriteAllText(App.Home+"Login.Lib",Encryption.Encrypt(Convert.ToString(currentTime)));

            }
            else if(currentTime == ReadTime * 60)
            {
                currentTime = 0;
                WrongAttempts = 0;
                Login.Visibility = Visibility.Visible;
                dispatcherTimer.Stop();
                File.WriteAllText(App.Home + "Login.Lib", "");
            }
            
        }

        private void EditConnection_Click(object sender, RoutedEventArgs e)
        {
            ConnectionWork CW = new ConnectionWork();
            CW.Show();
            this.Hide();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryLogin();
            }
        }
    }
}
