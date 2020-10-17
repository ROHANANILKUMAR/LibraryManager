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

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for AddStudents.xaml
    /// </summary>
    public partial class AddStudents : Window
    {
        Main M;
        static SQL sql = new SQL();
        int TextFieldsNo = 0;
        List<UIElement[]> Data = new List<UIElement[]>();
        string CommonClassData = null;

        public void LoadCommonClass()
        {
            foreach(string i in Classes)
            {
                CommonClass.Items.Add(i);
            }
            CommonClass.Items.Add("<No Value>");
        }

        public AddStudents(Main m)
        {
            M = m;
            InitializeComponent();
            LoadCommonClass();
            AddTextFields();
        }

        string[] Classes = sql.GetClasses().ToArray();

        public void AddTextFields()
        {
            TextFieldsNo++;
            TextBox NameBox = new TextBox() {Name="Name"+TextFieldsNo,Uid=TextFieldsNo.ToString(),Text="",Margin= new Thickness(0,TextFieldsNo*22,0,0),Width= 245,HorizontalAlignment=HorizontalAlignment.Left,VerticalAlignment=VerticalAlignment.Top,Height=22};
            TextBox AdmissionBox = new TextBox() { Name = "Admission" + TextFieldsNo, Text = "", Margin = new Thickness(245, TextFieldsNo*22, 0, 0), Width = 135, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 22 };
            TextBox RollBox = new TextBox() { Name = "Roll" + TextFieldsNo, Text = "", Margin = new Thickness(380, TextFieldsNo * 22, 0, 0), Width = 127, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 22 };
            ComboBox ClassBox = new ComboBox() { Name = "Class" + TextFieldsNo, Margin = new Thickness(507, TextFieldsNo*22, -1, 0), Width = 127, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 22,ItemsSource=Classes,SelectedItem=CommonClassData };
            RollBox.TextChanged += ((object sender, TextChangedEventArgs e) => { if (RollBox.Name == "Roll" + TextFieldsNo) AddTextFields(); });
            DataGrid.Children.Add(NameBox);
            DataGrid.Children.Add(AdmissionBox);
            DataGrid.Children.Add(RollBox);
            DataGrid.Children.Add(ClassBox);
            Data.Add(new UIElement[] {NameBox,AdmissionBox,RollBox,ClassBox });
        }

        private void NewLine_Click(object sender, RoutedEventArgs e)
        {
            AddTextFields();
        }

        private void CommonClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommonClass.SelectedItem != null && !CommonClass.SelectedItem.Equals("<No Value>"))
            {
                foreach(UIElement[] i in Data)
                {
                    ComboBox ClassBox = (ComboBox)i[3];
                    ClassBox.SelectedItem = CommonClass.SelectedItem.ToString();
                }
                CommonClassData = CommonClass.SelectedItem.ToString();
            }
            else if (CommonClass.SelectedItem.Equals("<No Value>"))
            {
                foreach (UIElement[] i in Data)
                {
                    ComboBox ClassBox = (ComboBox)i[3];
                    ClassBox.SelectedItem = null;
                    CommonClassData = null;
                }
            }
        }

        List<string> AddedUI = new List<string>();

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            

            foreach(UIElement[] i in Data)
            {
                TextBox Name = (TextBox)i[0];
                TextBox AdmissionNumber= (TextBox)i[1];
                TextBox Roll= (TextBox)i[2];
                ComboBox Class = (ComboBox)i[3];

                if (!AddedUI.Contains(Name.Name))
                {
                    if (Name.Text.Equals("") || AdmissionNumber.Text.Equals("") || Roll.Text.Equals(""))
                    {
                        if (Name.Text.Equals("") && AdmissionNumber.Text.Equals("") && Roll.Text.Equals(""))
                        {
                            break;
                        }
                        App.ErrorBox("Some fields have not been entered");
                    }
                    else
                    {
                        if (Class.SelectedItem != null)
                        {
                            string Response = Student.AddStudent(Name.Text, AdmissionNumber.Text, Roll.Text, Class.Text);
                            if (Response.Contains("New Student Error"))
                            {
                                App.ErrorBox(Response);
                                break;
                            }
                            else
                            {
                                DataGrid.Children.Remove(Name);
                                DataGrid.Children.Remove(AdmissionNumber);
                                DataGrid.Children.Remove(Roll);
                                DataGrid.Children.Remove(Class);
                                AddedUI.Add(Name.Name);
                            }
                            M.NewStudent();
                        }
                    }
                }
                
                
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Enter_Click(this, new RoutedEventArgs());
            }
        }
    }
}
