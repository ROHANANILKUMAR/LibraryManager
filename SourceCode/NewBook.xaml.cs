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
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for NewBook.xaml
    /// </summary>
    public partial class NewBook : Window
    {
        public NewBook()
        {
            InitializeComponent();
        }

        SQL sql = new SQL();

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string Response=Book.Add(Name.Text, Acc.Text, BookNumber.Text);
            if(Response.Contains("New Book Error"))
            {
                App.ErrorBox(Response);
            }
            else
            {
                App.SuccessBox(Response);
            }
        }
    }
}
