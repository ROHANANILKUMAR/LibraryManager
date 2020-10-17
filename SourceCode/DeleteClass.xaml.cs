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
    /// Interaction logic for DeleteClass.xaml
    /// </summary>
    public partial class DeleteClass : Window
    {
        Main M;
        static SQL sql = new SQL();
        public DeleteClass(Main m)
        {
            M = m;
            InitializeComponent();
            try
            {
                foreach(string i in Classes)
                { 
                    ClassList.Items.Add(i);
                }
            }
            catch (Exception ex)
            {
                App.ErrorBox(ex.Message);
            }
        }

        List<string> Classes=sql.GetClasses();

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            
            if (ClassList.SelectedItems != null)
            {
                string[] DeleteClasses= new string[ClassList.SelectedItems.Count];

                ClassList.SelectedItems.CopyTo(DeleteClasses, 0);

                foreach (string i in DeleteClasses)
                {
                    

                    if (MessageBox.Show(string.Format("Do you really want to delete this class {0}? The students in the class will also be deleted", i), "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        sql.DeleteData("classes", "class", i);
                        sql.DeleteData("students", "class", i);
                        App.SuccessBox("The class is deleted");

                        ClassList.Items.Clear();
                        Classes = sql.GetClasses();
                        foreach (string x in Classes)
                        {
                            ClassList.Items.Add(x);
                        }

                        M.NewClass();

                    }
                }
            }
            
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Delete_Click(this, new RoutedEventArgs());
            }
        }
    }
}
