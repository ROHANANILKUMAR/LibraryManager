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
    /// Interaction logic for EditStudentInfo.xaml
    /// </summary>
    public partial class EditStudentInfo : Window
    {
        SQL sql = new SQL();

        Student student;
        Main M;

        bool NoChange = true;

        List<string> OldData = new List<string>();

        public EditStudentInfo(Student student,Main m)
        {
            M = m;
            this.student = student;
            InitializeComponent();
            NameS.Text = student.Name;
            Class.Text = student.Class;
            Addm.Text = student.AdmissionNumber;
            RollNumber.Text = student.Roll;
        }

        private void Update(List<string> Classes, List<string> Addnos, List<string> RollNos)
        {
            try
            {
                sql.UpdateMultipleData("Students", new string[] { "Name", "AdmissionNumber", "Class", "Roll" }, new string[] { NameS.Text, Addm.Text, Class.Text, RollNumber.Text }, "AdmissionNumber", student.AdmissionNumber);
                sql.UpdateData("StudentBookData", "AdmissionNumber", Addm.Text, "AdmissionNumber", student.AdmissionNumber);
                sql.UpdateData("BookStudentData", "AdmissionNumber", Addm.Text, "AdmissionNumber", student.AdmissionNumber);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("exists")) { }
                else
                {
                    App.ErrorBox(ex.Message);
                }
            }

            App.SuccessBox("Success the student's info was updated");
            M.NewStudent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            List<string> Classes = sql.GetClasses();
            List<string> Addnos = new List<string>();
            List<string> RollNos = new List<string>();

            foreach(Dictionary<string,string> i in sql.GetDataList("students", "AdmissionNumber", Addm.Text,new string[]{"AdmissionNumber" }))
            {
                Addnos.Add(i["AdmissionNumber"]);
            }

            if (Classes.Contains(Class.Text) && !Addnos.Contains(Addm.Text))
            {
                Update(Classes,Addnos,RollNos);  
            }
            else if(!Classes.Contains(Class.Text))
            {
                App.ErrorBox("The class is not in the database");
            }
            else if (Addnos.Contains(Addm.Text))
            {
                if (NoChange)
                {
                    Update(Classes, Addnos, RollNos);
                }
                else
                {
                    App.ErrorBox("The there is already a student with the same AdmissionNumber");
                }
                
            }



        }

        private void CheckChange()
        {
            if (student.AdmissionNumber == Addm.Text)
            {
                NoChange = true;
            }
            else
            {
                NoChange = false;
            }
        }

        private void Addm_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckChange();
        }

        private void NameS_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Class_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void RollNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save_Click(this, new RoutedEventArgs());
            }
        }
    }
}
