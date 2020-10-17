using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for NewClass.xaml
    /// </summary>
    public partial class NewClass : Window
    {
        SQL sql = new SQL();
        Main M;

        public NewClass(Main m)
        {
            M = m;
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (Class.Text.Equals(""))
            {
                App.ErrorBox("Class Name not entered");
            }
            else
            {
                List<string> classes = sql.GetClasses();
                try
                {
                    if (classes.Contains(Class.Text))
                    {
                        App.ErrorBox("A class with the same name exists");
                    }
                    else
                    {
                        sql.InsertData("Classes", new string[] { "Class" }, new string[] { Class.Text });

                        Class.Clear();
                        M.NewClass();
                    }


                }

                catch (Exception ex)
                {
                    App.ErrorBox(ex.Message);
                }
            }
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Add_Click(this, new RoutedEventArgs());
            }
        }
    }
}
