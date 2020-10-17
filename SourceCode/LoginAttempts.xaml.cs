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
using System.IO;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for LoginAttempts.xaml
    /// </summary>
    public partial class LoginAttempts : Window
    {
        string RState;
        string NumberOfWrongAttempts;
        string WaitTime;

        public LoginAttempts()
        {
            InitializeComponent();
            string Read=File.ReadAllText(App.Home + "Settings.Lib");
            Read = Encryption.Decrypt(Read);
            RState = Read.Split(':')[0];
            NumberOfWrongAttempts= Read.Split(':')[1];
            WaitTime = Read.Split(':')[2];
            if (RState.Equals("ON"))
            {
                State.Text = "ON";
                ONGRID.Visibility = Visibility.Visible;
                StateChange.Content = "OFF";
                NumberOfAttempts.Text = NumberOfWrongAttempts;
                Time.Text = WaitTime;
            }
            else if (RState.Equals("OFF"))
            {
                State.Text = "OFF";
                ONGRID.Visibility = Visibility.Hidden;
                StateChange.Content = "ON";
                NumberOfAttempts.Text = NumberOfWrongAttempts;
                Time.Text = WaitTime;
            }
        }

        private void StateChange_Click(object sender, RoutedEventArgs e)
        {
            string state="";
            if(ONGRID.Visibility == Visibility.Hidden)
            {
                state = "ON";
                StateChange.Content = "OFF";
                string ForModi=File.ReadAllText(App.Home + "Settings.Lib");
                ForModi = Encryption.Decrypt(ForModi);
                string[] ForModiArray = ForModi.Split(':');
                ForModiArray[0] = "ON";
                State.Text = "ON";
                File.WriteAllText(App.Home + "Settings.Lib", Encryption.Encrypt(string.Join(":", ForModiArray)));
                NumberOfAttempts.Text = NumberOfWrongAttempts;
                Time.Text = WaitTime;
                ONGRID.Visibility = Visibility.Visible;
            }
            else if (ONGRID.Visibility == Visibility.Visible)
            {
                state = "OFF";
                StateChange.Content = "ON";
                State.Text = "OFF";
                string ForModi = File.ReadAllText(App.Home + "Settings.Lib");
                ForModi = Encryption.Decrypt(ForModi);
                string[] ForModiArray = ForModi.Split(':');
                ForModiArray[0] = "OFF";
                File.WriteAllText(App.Home + "Settings.Lib", Encryption.Encrypt(string.Join(":", ForModiArray)));
                ONGRID.Visibility = Visibility.Hidden;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string ForModi = File.ReadAllText(App.Home + "Settings.Lib");
            ForModi = Encryption.Decrypt(ForModi);
            string[] ForModiArray = ForModi.Split(':');
            ForModiArray[1] = NumberOfAttempts.Text;
            ForModiArray[2] = Time.Text;
            File.WriteAllText(App.Home + "Settings.Lib", Encryption.Encrypt(string.Join(":", ForModiArray)));

            string Read = File.ReadAllText(App.Home + "Settings.Lib");
            Read = Encryption.Decrypt(Read);
            RState = Read.Split(':')[0];
            NumberOfWrongAttempts = Read.Split(':')[1];
            WaitTime = Read.Split(':')[2];

            App.SuccessBox("Saved");
        }
    }
}
