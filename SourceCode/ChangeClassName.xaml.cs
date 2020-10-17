using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for ChangeClassName.xaml
    /// </summary>
    public partial class ChangeClassName : Window
    {
        SQL sql = new SQL();
        Main M;

        public ChangeClassName(Main m)
        {
            M = m;
            try
            {
                InitializeComponent();

                Classes = sql.GetClasses();

                foreach (string i in Classes)
                {
                    ClassList.Items.Add(i);
                }
                ClassList.SelectionChanged += Selectionchanged;
            }
            catch (Exception ex)
            {
                App.ErrorBox(ex.Message);
            }
        }
        string ClassName="";
        
        
        List<string> Classes = new List<string>();
        private void Selectionchanged(object sender, RoutedEventArgs e)
        {
            try { ClassName = ClassList.SelectedItem.ToString(); } catch { }
        }
    
    private void Change_Click(object sender, RoutedEventArgs e)
        {
            List<string> AddNos = new List<string>();
            if (!ClassName.Equals(""))
            {
                if (!Classes.Contains(NewClassName.Text))
                {
                    sql.UpdateData("Classes", "Class", NewClassName.Text, "Class", ClassName);
                    sql.UpdateData("Students", "Class", NewClassName.Text, "Class", ClassName);

                    M.NewClass();

                    App.SuccessBox("The class name was changed");

                    ClassList.Items.Clear();
                    Classes = sql.GetClasses();
                    foreach (string i in Classes)
                    {
                        ClassList.Items.Add(i);
                    }
                }
                else
                {
                    App.ErrorBox("There is a class with the same name");
                }
            }
            else
            {
                App.ErrorBox("No class has been selected");
            }
        }
        }
}
