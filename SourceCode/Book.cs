using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManager
{
    public class Book
    {
        public string Name;
        public string Acc;
        public string BookNo;
        public string IssuedBy;
        public bool IsAvailable;

        static SQL sql = new SQL();
        public Book(string acc)
        {
            try
            {
                Dictionary<string, string> book = sql.GetSingleColumn("books", "acc", acc, new string[] { "Name", "Acc", "BookNo" });
                Dictionary<string, string> Transact = sql.GetSingleColumn("CurrentBooks", "acc", acc, new string[] { "AdmissionNumber" });
                Name = book.ContainsKey("Name") ? book["Name"] : null;
                Acc = book.ContainsKey("Acc") ? book["Acc"] : null;
                BookNo = book.ContainsKey("BookNo") ? book["BookNo"] : null;
                IssuedBy = Transact.ContainsKey("AdmissionNumber") ? Transact["AdmissionNumber"] : null;

            }
            catch
            {
                Name = null;
                Acc = null;
                BookNo =  null;
                IssuedBy = null;
            }
            if (Name == null)
            {
                IsAvailable = false;
            }
            else
            {
                IsAvailable = true;
            }
            
        }

        public bool DelFromDB()
        {
            if (Name != null)
            {
                sql.DeleteData("Books", "Acc", this.Acc);
                sql.DeleteData("CurrentBooks", "Acc", this.Acc);
                sql.DeleteData("StudentBookData", "Acc", this.Acc);

                return true;
            }
            else
            {
                return false;
            }
        }
        public static string Add(string Name, string Acc,string BookNo)
        {
            Book book = new Book(Acc);
            if (!book.IsAvailable)
            {
                if(sql.GetSingleColumn("Books","BookNo",BookNo,new string[] { "BookNo" }).Count > 0)
                {
                    return "New Book Error : Book with same Book no. exists";
                }
                else
                {
                    sql.InsertData("Books", new string[] { "Name", "Acc", "BookNo" }, new string[] { Name, Acc, BookNo });
                    return "Book Added";
                }
            }
            else
            {
                return "New Book Error : Book with same Acc no. exists";
            }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
