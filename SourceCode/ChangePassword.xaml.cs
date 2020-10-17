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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        SQL sql = new SQL();
        MySqlConnection LMConn;
        public ChangePassword()
        {
            InitializeComponent();
            LMConn = new MySqlConnection(App.ConStr);
            LMConn.Open();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {

            string oldPassword = sql.GetPassword();

            if (CurrentPass.Password.Equals(Encryption.Decrypt(oldPassword)))
            {
                if (NewPass.Password.Equals(RetypePass.Password))
                {
                    sql.InsertData("Passwords", new string[] { "Password" }, new string[] { Encryption.Encrypt(NewPass.Password) });
                    sql.DeleteData("Passwords", "Password", oldPassword);
                    App.SuccessBox("The password is updated");
                    this.Hide();
                }
                else
                {
                    App.ErrorBox("The passwords does not match ");
                }
            }
            else
            {
                App.ErrorBox("Current password is wrong");
            }
        }
    }
}
