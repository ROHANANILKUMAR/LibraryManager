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
    /// Interaction logic for IncrementSelectedClass.xaml
    /// </summary>
    public partial class IncrementSelectedClass : Window
    {
        string ClassName;
        MySqlConnection LMConn;
        public IncrementSelectedClass()
        {
            try
            {
                InitializeComponent();
                LMConn = new MySqlConnection(App.ConStr);
                LMConn.Open();
                MySqlCommand checkclasscmd = new MySqlCommand("select * from Classes;", LMConn);
                MySqlDataReader ClassesData = checkclasscmd.ExecuteReader();
                while (ClassesData.Read())
                {
                    Classes.Add((string)ClassesData["Class"]);
                    ClassList.Items.Add((string)ClassesData["Class"]);
                }
                ClassesData.Close();
                ClassList.SelectionChanged += Selectionchanged;
            }
            catch(Exception ex)
            {
                App.ErrorBox(ex.Message);
            }
        }
        List<string> Classes = new List<string>();
        private void Selectionchanged(object sender, RoutedEventArgs e)
        {
            ClassName = ClassList.SelectedItem.ToString(); 
        }


        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            
            List<string> AddNos = new List<string>();

            if (!Classes.Contains(NewClassName.Text))
            {
                MySqlCommand createclasscmd = new MySqlCommand("insert into Classes(Class)values('"+NewClassName.Text+"');", LMConn);
                createclasscmd.ExecuteNonQuery();
            }

            MySqlCommand SetStudentClasscmd = new MySqlCommand("update Students set Class='"+NewClassName.Text+"' Where Class='"+ClassName+"';", LMConn);
            SetStudentClasscmd.ExecuteNonQuery();
            LMConn.Close();
            App.SuccessBox("Students were promoted!");
            App.NewClassAdded = true;
         

        }
    }
}
