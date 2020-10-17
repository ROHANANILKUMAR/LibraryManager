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
using MySql.Data.MySqlClient;
using System.Windows.Shapes;

namespace LibraryManager
{
    
    /// <summary>
    /// Interaction logic for Fine.xaml
    /// </summary>
    public partial class Fine : Window
    {
        Main M;

        Student student;
        Book book;

        SQL sql = new SQL();

        public Fine(Main m,Student s)
        {
            student = s;
            M = m;

            book = new Book(student.IssuedBook);
            InitializeComponent();
            
            AdmissionNumberShow.Text = student.AdmissionNumber;
            AccShow.Text = book.Acc;
            StudentNameShow.Text = student.Name;
            BookNameShow.Text = book.Name;
        }

       

        private void AddFineButton_Click(object sender, RoutedEventArgs e)
        {
            if (student.Fine.Equals("Null"))
            {
                try
                {
                    
                    float.Parse(Amount.Text);
                    if (!Amount.Equals(""))
                    {
                        sql.InsertData("Fine", new string[] { "admissionnumber", "acc", "amount" }, new string[] { student.AdmissionNumber, book.Acc, Amount.Text });

                        M.FineChanged(student);

                        this.Hide();

                        student = new Student(student.AdmissionNumber);
                        App.SuccessBox("Fined");
                    }
                    else
                    {
                        App.ErrorBox("Please enter fine amount");
                    }
                }
                catch
                {
                    App.ErrorBox("Invalid Amount");
                }
                
                
            }
            else
            {
                App.ErrorBox("This student has already been fined");
            }
        }

        private void AddFine_Click(object sender, RoutedEventArgs e)
        {
            Amountlbl.Visibility = Visibility.Visible;
            Amount.Visibility = Visibility.Visible;
            AddFineButton.Visibility = Visibility.Visible;
            PayFineButton.Visibility = Visibility.Hidden;
        }

        private void PayFine_Click(object sender, RoutedEventArgs e)
        {
            Amountlbl.Visibility = Visibility.Hidden;
            Amount.Visibility = Visibility.Hidden;
            AddFineButton.Visibility = Visibility.Hidden;
            PayFineButton.Visibility = Visibility.Visible;
        }

        private void PayFineButton_Click(object sender, RoutedEventArgs e)
        {
            if (student.Fine.Equals("Null"))
            {
                App.ErrorBox("This Student was not fined");
            }
            else
            {
                student.PayFine();
                student = new Student(student.AdmissionNumber);
                M.FineChanged(student);
            }
        }
    }
}
