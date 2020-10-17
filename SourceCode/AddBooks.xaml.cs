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
    /// Interaction logic for AddBooks.xaml
    /// </summary>
    public partial class AddBooks : Window
    {

        Main M;
        static SQL sql = new SQL();
        int TextFieldsNo = 0;
        List<UIElement[]> Data = new List<UIElement[]>();

        public AddBooks(Main m)
        {
            M = m;
            InitializeComponent();
            AddTextFields();
        }

        public void AddTextFields()
        {
            TextFieldsNo++;
            TextBox NameBox = new TextBox() { Name = "Name" + TextFieldsNo, Text = "", Margin = new Thickness(0, TextFieldsNo * 22, 0, 0), Width = 252, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 22 };
            TextBox AccBox = new TextBox() { Name = "Acc" + TextFieldsNo, Text = "", Margin = new Thickness(252, TextFieldsNo * 22, 0, 0), Width = 134, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 22 };
            TextBox BookNoBox = new TextBox() { Name = "BookNo" + TextFieldsNo, Text = "", Margin = new Thickness(386, TextFieldsNo * 22, -15, 0), Width = 134, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 22 };
            BookNoBox.TextChanged += ((object sender, TextChangedEventArgs e) => { if (BookNoBox.Name == "BookNo" + TextFieldsNo) AddTextFields(); });
            DataGrid.Children.Add(NameBox);
            DataGrid.Children.Add(AccBox);
            DataGrid.Children.Add(BookNoBox);
            Data.Add(new UIElement[] { NameBox, AccBox, BookNoBox });
        }

        private void NewLine_Click(object sender, RoutedEventArgs e)
        {
            AddTextFields();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            List<string> AddedUI = new List<string>();
            foreach (UIElement[] i in Data)
            {
                TextBox Name = (TextBox)i[0];
                TextBox AccNumber = (TextBox)i[1];
                TextBox BookNo = (TextBox)i[2];

                if (!AddedUI.Contains(Name.Name))
                {
                    if (Name.Text.Equals("") || AccNumber.Text.Equals("") || BookNo.Text.Equals(""))
                    {
                        if (Name.Text.Equals("") && AccNumber.Text.Equals("") && BookNo.Text.Equals(""))
                        {
                            break;
                        }
                        App.ErrorBox("Some fields have not been entered");
                    }
                    else
                    {
                        
                            string Response = Book.Add(Name.Text,AccNumber.Text,BookNo.Text);
                            if (Response.Contains("New Book Error"))
                            {
                                App.ErrorBox(Response);
                                break;
                            }
                            else
                            {
                                DataGrid.Children.Remove(Name);
                                DataGrid.Children.Remove(AccNumber);
                                DataGrid.Children.Remove(BookNo);
                                AddedUI.Add(Name.Name);
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
