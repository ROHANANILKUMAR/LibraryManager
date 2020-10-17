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
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for NewStudent.xaml
    /// </summary>
    public partial class NewStudent : Window
    {
        SQL sql = new SQL();
        Main M;
        public NewStudent(Main m)
        {
            M = m;
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            List<string> Addnos = new List<string>();
            List<string> Rollnos = new List<string>();
            List<string> Classes = new List<string>();
            try
            {
                List<Dictionary<string, string>> ClassData = sql.GetDataList("students", "Class", Class.Text, new string[] { "Roll" });
                List<Dictionary<string, string>> AddmNo = sql.GetDataList("students", new string[] { "AdmissionNumber" });

                foreach(string i in sql.GetClasses())
                {
                    Classes.Add(i);
                }

                foreach(Dictionary<string,string> i in ClassData)
                {
                    Rollnos.Add(i["Roll"]);
                }

                foreach(Dictionary<string,string> i in AddmNo)
                {
                    Addnos.Add(i["AdmissionNumber"]);
                }

                if (Addnos.Contains(Addm.Text))
                {
                    App.ErrorBox("A Student with the same admission number exists");
                }
                else if (!Classes.Contains(Class.Text))
                {
                    App.ErrorBox("There is no class named :" + Class.Text + " in the database");
                }
                else if (Rollnos.Contains(RollNumber.Text))
                {
                    App.ErrorBox("A Student with the same Roll number in the same class exists");
                }
                
                
                else
                {
                    sql.InsertData("students", new string[] { "Name", "AdmissionNumber", "Class", "Roll" }, new string[] { Name.Text, Addm.Text, Class.Text, RollNumber.Text });

                    M.NewStudent();
                    App.SuccessBox("Student Added");
                    Name.Text = "";
                    RollNumber.Text = "";
                    Addm.Text = "";
                    Class.Text = "";
                    
                }
            }
            catch (Exception ex)
            {
                App.ErrorBox(ex.Message);
            }



        }
    }
}
