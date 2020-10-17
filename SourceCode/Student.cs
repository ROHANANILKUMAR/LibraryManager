using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManager
{
    public class Student
    {
        public string Name;
        public string Class;
        public string AdmissionNumber;
        public string Roll;
        public string Fine;
        public string IssuedBook;
        public bool IsAvailable;

        static SQL sql = new SQL();
        public Student(string admissionNumber)
        {
            try
            {
                Dictionary<string, string> student = sql.GetSingleColumn("students", "admissionnumber", admissionNumber, new string[] { "Name", "Class", "AdmissionNumber", "Roll" });
                Dictionary<string, string> Finedata = sql.GetSingleColumn("fine", "admissionnumber", admissionNumber, new string[] { "AdmissionNumber", "Amount" });
                Dictionary<string, string> Transact = sql.GetSingleColumn("CurrentBooks", "admissionnumber", admissionNumber, new string[] { "Acc" });

                Name = student.ContainsKey("Name") ? student["Name"] : null;
                Class = student.ContainsKey("Class") ? student["Class"] : null;
                AdmissionNumber = student.ContainsKey("AdmissionNumber") ? student["AdmissionNumber"] : null;
                Roll = student.ContainsKey("Roll") ? student["Roll"] : null;
                Fine = Finedata.Keys.Contains("Amount") ? Finedata["Amount"] : "Null";
                IssuedBook = Transact.ContainsKey("Acc") ? Transact["Acc"] : null;

                IsAvailable=(Name != null)?true:false;
            }
            catch
            {
                Name = null;
                Class = null;
                AdmissionNumber = null;
                Roll = null;
                Fine = null;
                IssuedBook = null;
            }

        }

        public bool Issue(Book book, string Date)
        {
            if (book.Name == null)
            {
                App.ErrorBox("No Book Selected or acc number is invalid");
            }
            else if (this.Name == null)
            {
                App.ErrorBox("No Student Selected or Admission Number number is invalid");
            }
            else
            {

                if (this.IssuedBook != null)
                {
                    App.ErrorBox("This student has not returned his book.");
                }
                else if (book.IssuedBy != null)
                {
                    App.ErrorBox("This book is already issued by someone");
                }
                else
                {
                    if (Date.Split('-').Length == 3)
                    {

                        sql.InsertData("StudentBookData", new string[] { "AdmissionNumber", "BookName", "Acc", "BookNo", "DateOfIssue" }, new string[] { this.AdmissionNumber, book.Name, book.Acc, book.BookNo, Date });
                        sql.InsertData("BookStudentData", new string[] { "Acc", "StudentName", "AdmissionNumber", "DateOfIssue" }, new string[] {book.Acc, Name, AdmissionNumber, Date });
                        sql.InsertData("CurrentBooks", new string[] { "AdmissionNumber", "Acc", "DateOfIssue" }, new string[] { this.AdmissionNumber, book.Acc, Date });

                        App.SuccessBox("Book Issued");

                        return true;
                    }
                    else
                    {
                        App.ErrorBox("Date is not in the correct format. Use '-' to seperate between numbers");
                    }

                }

            }
            return false;
        }

        public bool Return(Book book,string Date)
        {
            string fine = "";
            string IssueDate = sql.GetSingleColumn("StudentBookData", "Acc", book.Acc, new string[] { "DateOfIssue" })["DateOfIssue"];

            if (!book.IsAvailable)
            {
                App.ErrorBox("This Student did not issue any book");
                return false;
            }
            else
            {
                
                if (Fine.Equals("Null"))
                {
                    if(Date.Split('-').Length == 3)
                    {
                        sql.DeleteData("CurrentBooks", "Acc", book.Acc);
                        sql.UpdateMultipleData("StudentBookData", new string[] { "DateOfReturn" }, new string[] { Date }, new string[] { "Acc", "DateOfIssue", "AdmissionNumber" }, new string[] { book.Acc, IssueDate, AdmissionNumber });
                        sql.UpdateMultipleData("BookStudentData", new string[] { "DateOfReturn" }, new string[] { Date }, new string[] { "Acc", "DateOfIssue", "AdmissionNumber" }, new string[] { book.Acc, IssueDate, AdmissionNumber });
                        App.SuccessBox("Book Returned");
                        return true;
                    }
                    else
                    {
                        App.ErrorBox("Date not given in correct format");
                        return false;
                    }

                }
                else
                {
                    if (App.Ask("This Student have to pay an amount of Rupees " + Fine + "/- as fine. Do you wish to pay this now?", "Fine"))
                    {
                        sql.DeleteData("CurrentBooks", "Acc", book.Acc);
                        sql.UpdateMultipleData("StudentBookData", new string[] { "DateOfReturn" }, new string[] { Date }, new string[] { "Acc", "DateOfIssue", "AdmissionNumber" }, new string[] { book.Acc, IssueDate, AdmissionNumber });
                        sql.UpdateMultipleData("BookStudentData", new string[] { "DateOfReturn" }, new string[] { Date }, new string[] { "Acc", "DateOfIssue", "AdmissionNumber" }, new string[] { book.Acc, IssueDate, AdmissionNumber });


                        PayFine();

                        App.SuccessBox("Fine payed and bood returned");
                        return true;
                    }
                    else
                    {
                        App.ErrorBox("Returning aborted");
                        return false;
                    }
                }
            }
        }

        public bool AddFine(string Amount)
        {
            if (Fine != null)
            {
                return true;
            }
            return false;
        }

        public void PayFine()
        {
            sql.DeleteData("fine", "admissionNumber", AdmissionNumber);
            App.SuccessBox("Fine paid");

        }

        public bool DelFromDB()
        {
            if (Name != null)
            {
                sql.DeleteData("Students", "AdmissionNumber", AdmissionNumber);
                sql.DeleteData("CurrentBooks", "AdmissionNumber", AdmissionNumber);
                sql.DeleteData("StudentBookData", "AdmissionNumber", AdmissionNumber);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static string AddStudent(string Name,string AdmissionNumber,string Roll,string Class)
        {
            Student Trial = new Student(AdmissionNumber);
            if (Trial.Name == null)
            {
                Dictionary<string, string> RollCheck = sql.GetSingleColumn("Students", "Roll", Roll, new string[] { "Class" });
                if (sql.GetClasses().Contains(Class))
                {
                    if (RollCheck.ContainsValue(Class))
                    {
                        return ("New Student Error : "+"A student with the same Roll number exists in the same class");
                    }
                    else
                    {
                        sql.InsertData("Students", new string[] { "Name", "AdmissionNumber", "Class", "Roll" }, new string[] { Name, AdmissionNumber, Class, Roll });
                        return "Student Added";
                    }
                }
                else
                {
                    return "New Student Error :  Class not found in DataBase";
                }
               

            }
            else
            {
                return ("New Student Error : "+string.Format("A student with the Admission Number {0}({1} of class {2}) already exists", Trial.AdmissionNumber, Trial.Name, Trial.Class));
            }
            
        }
        public override string ToString()
        {
            return Name+" of class "+Class;
        }
    }
}
