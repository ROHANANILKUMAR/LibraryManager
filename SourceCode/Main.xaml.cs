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
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            
            Start();
            
        }
        private void MainClose(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        SQL sql = new SQL();
        Student student;
        Book book;

        public void SetVisibility(Grid g)
        {
            Grid[] Grids = new Grid[]
            {
                StudentInfoFrame,
                BookInfoGrid,
                IssueFrame,
                ReturnFrame,
                BookSuggestions,
                StudentFrame,
                StudentSuggestions
            };

            foreach(Grid i in Grids)
            {
                if (i == g)
                {
                    i.Visibility = Visibility.Visible;
                }
                else
                {
                    i.Visibility = Visibility.Hidden;
                }
            }
        }

        #region Load
        public void LoadClass()
        {
            ClassList.Items.Clear();
            Dictionary<string, List<string>> ClassDivSort = new Dictionary<string, List<string>>();

            foreach(string i in sql.GetClasses())
            {
                string Class = i.Split(' ')[0];
                try
                {
                    string div = i.Split(' ')[1];
                    if (ClassDivSort.ContainsKey(Class))
                    {
                        ClassDivSort[Class].Add(div);
                    }
                    else
                    {
                        ClassDivSort[Class] = new List<string> { div };
                    }
                }
                catch
                {
                    ClassDivSort.Add(Class, new List<string> { "1" });
                }
                
            }

            foreach(List<string> i in ClassDivSort.Values)
            {
                i.Sort();
            }

            List<string> ClassesToSort = new List<string>();

            foreach(string i in sql.GetClasses())
            {
                if (!ClassesToSort.Contains(i.Split(' ')[0]))
                {
                    ClassesToSort.Add(i.Split(' ')[0]);
                }
                
            }

            string[] ClassesSorted = ClassesToSort.ToArray();

            RomanNumbersComparer Classes = new RomanNumbersComparer();
           
            Array.Sort(ClassesSorted,Classes);

            foreach (string i in ClassesSorted)
            {
                if (ClassDivSort[i].Count == 0)
                {
                    ClassList.Items.Add(i);
                }
                else
                {
                    foreach (string Div in ClassDivSort[i])
                    {
                        if (Div == "1")
                        {
                            ClassList.Items.Add(i);
                        }
                        else
                        {
                            ClassList.Items.Add(i + " " + Div);
                        }
                        
                    }
                }      
            }
            ClassList.SelectionChanged += ClasslistSelectionChanged;
            StudentNameList.SelectionChanged += StudentListSelectionChanged;
            StudentRollList.SelectionChanged += StudentListSelectionChanged;
            StudentAddmList.SelectionChanged+= StudentListSelectionChanged;
        }

        public void LoadStudents()
        {
            if (ClassList.SelectedItem != null)
            {
                try
                {
                    int index = 0;

                    StudentNameList.Items.Clear();
                    StudentRollList.Items.Clear();
                    StudentAddmList.Items.Clear();
                    StudentsInCurrentClass.Clear();

                    List<Dictionary<string, string>> students = sql.GetDataList("Students", "Class", ClassList.SelectedItem.ToString(), "Roll asc", new string[] { "Roll", "AdmissionNumber", "Name" });
                    Dictionary<int, Dictionary<string, string>> StudentsSorted = new Dictionary<int, Dictionary<string, string>>();
                    List<int> RollNos = new List<int>();

                    foreach (Dictionary<string, string> i in students)
                    {
                        RollNos.Add(int.Parse(i["Roll"]));
                        StudentsSorted.Add(int.Parse(i["Roll"]), i);
                    }

                    RollNos.Sort();

                    foreach (int i in RollNos)
                    {
                        StudentRollList.Items.Add(StudentsSorted[i]["Roll"]);
                        StudentNameList.Items.Add(StudentsSorted[i]["Name"]);
                        StudentAddmList.Items.Add(StudentsSorted[i]["AdmissionNumber"]);
                        StudentsInCurrentClass.Add(index, StudentsSorted[i]["AdmissionNumber"]);
                        index++;
                    }

                    SetVisibility(StudentFrame);
                    index = 0;
                }
                catch { }
           
            }
        }
        #endregion

        #region Start
        public void Start()
        { 
            LoadClass();
            RoutedCommand NewClass = new RoutedCommand();
            NewClass.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));

            RoutedCommand NewStudent = new RoutedCommand();
            NewStudent.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            RoutedCommand NewBook = new RoutedCommand();
            NewBook.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control));

            CommandBindings.Add(new CommandBinding(NewClass, Class_Click));
            CommandBindings.Add(new CommandBinding(NewStudent, Student_Click));
            CommandBindings.Add(new CommandBinding(NewBook, Book_Click));
        }
        #endregion

        #region List Selection Changed
        //string AdmissionNumberOfCurrentStudent;
        private void StudentListSelectionChanged(object sender, EventArgs e)
        {
            
            try
            {
                if (StudentNameList.SelectedItem!=null || StudentRollList.SelectedItem != null || StudentAddmList.SelectedItems!=null)
                {
                    if (StudentAddmList.SelectedItem != null)
                    {
                        student = new Student(StudentAddmList.SelectedItem.ToString());
                    }
                    else if (StudentRollList.SelectedItem != null)
                    {
                        student = new Student(StudentsInCurrentClass[StudentRollList.SelectedIndex]);
                    }
                    else
                    {
                        student = new Student(StudentsInCurrentClass[StudentNameList.SelectedIndex]);
                    }
                    NameDisplay.Text = student.Name;
                    AdmissionNumberDisplay.Text = student.AdmissionNumber;
                    RollNumberDisplay.Text = (string)student.Roll;
                    ClassDisplay.Text = (string)student.Class;
                    FineAmount.Text = student.Fine;
                    
                    //string query = "Select BookName,Acc,BookNo,DateOfIssue,DateOfReturn from StudentBookData where AdmissionNumber='" + AdmissionNumberOfCurrentStudent + "';";
                    
                    StudentData.ItemsSource = sql.GetDataTable("studentbookdata","admissionnumber",student.AdmissionNumber,new string[] { "BookName", "Acc", "BookNo", "DateOfIssue", "DateOfReturn" }).DefaultView;
                    StudentInfoFrame.Visibility = Visibility.Visible;
                }
            }catch(Exception ex)
            {
                
            }

        }

        private static Dictionary<int, string> StudentsInCurrentClass = new Dictionary<int, string>();
        private void ClasslistSelectionChanged(object sender, EventArgs e)
        {
            LoadStudents();
        }
        
        private void BookListSelectionchange(object sender, RoutedEventArgs e)
        {
            string AccOfSelectedBook = null;

            try
            {
                AccOfSelectedBook = BookList.SelectedItem.ToString().Split('\t')[1];
            }
            catch { }
            if (AccOfSelectedBook!=null)
            {
               
                string CurrentlyIssuedAdmissionNumber = "";
                string BookNameCheck = "";

                book = new Book(AccOfSelectedBook);
                
                BookNameCheck = (string)book.Name;
                BookName.Text = (string)book.Name;
                BookAcc.Text = (string)book.Acc;
                BookNo.Text = (string)book.BookNo;

                CurrentlyIssuedAdmissionNumber = book.IssuedBy;

                if (CurrentlyIssuedAdmissionNumber==null)
                {
                    CurrentlyIssued.Text = "Nobody";
                }
                else
                {
                    CurrentlyIssued.Text = CurrentlyIssuedAdmissionNumber;
                }

                BookData.ItemsSource = sql.GetDataTable("BookStudentData","Acc",BookAcc.Text,new string[] { "AdmissionNumber", "StudentName", "DateOfIssue", "DateOfReturn" }).DefaultView;
                BookInfoGrid.Visibility = Visibility.Visible;
                AccOfSelectedBook = "";
            }
            
        }

        #endregion

        #region Menu Items
        private void LoginAttempts_Click(object sender, RoutedEventArgs e)
        {
            LoginAttempts L = new LibraryManager.LoginAttempts();
            L.Show();
        }

        private void Mass_Enter_Books_Click(object sender, RoutedEventArgs e)
        {
            //MassAddBooks MAB = new MassAddBooks(uid, pass);
            AddBooks MAB = new AddBooks(this);
            MAB.Show();
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            Credits c = new LibraryManager.Credits();
            c.Show();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help h = new Help();
            h.Show();
        }

        private void Issue_Click(object sender, RoutedEventArgs e)
        {
            StudentFrame.Visibility = Visibility.Hidden;
            SetupIssue();
        }

        private void Class_Click(object sender, RoutedEventArgs e)
        {
            NewClass NC = new NewClass(this);
            NC.Show();
            
        }

        private void Book_Click(object sender, RoutedEventArgs e)
        {
            AddBooks NB = new AddBooks(this);
            NB.Show();
        }

        private void Student_Click(object sender, RoutedEventArgs e)
        {
            AddStudents NS = new AddStudents(this);
            NS.Show();
        }
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            StudentFrame.Visibility = Visibility.Hidden;
            SetUpReturn();
        }
        private void Increment_Selected_Class_Click(object sender, RoutedEventArgs e)
        {
            IncrementSelectedClass ISC = new IncrementSelectedClass();
            ISC.Show();
        }

        private void Mass_Enter_Students_Click(object sender, RoutedEventArgs e)
        {
            AddStudents MAS = new AddStudents(this);
            //MassAddStudents MAS = new MassAddStudents();
            MAS.Show();
        }

        [STAThread]
        private void Change_Classname_Click(object sender, RoutedEventArgs e)
        {
            ChangeClassName CCN = new ChangeClassName(this);
            CCN.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        private void Change_Password_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword CP = new ChangePassword();
            CP.Show();
        }

        private void Delete_Class_Click(object sender, RoutedEventArgs e)
        {
            DeleteClass DC = new DeleteClass(this);
            DC.Show();
        }
        #endregion

        #region Frame Close Button

        private void CloseBookSuggestion_Click(object sender, RoutedEventArgs e)
        {
            BookSuggestions.Visibility = Visibility.Hidden;
        }

        private void CloseBookInfo_Click(object sender, RoutedEventArgs e)
        {
            BookInfoGrid.Visibility = Visibility.Hidden;
            BookName.Text = "";
            BookAcc.Text = "";
            CurrentlyIssued.Text = "";
            BookSuggestions.Visibility = Visibility.Hidden;
        }

        private void StudentInfoFrameClose_Click(object sender, RoutedEventArgs e)
        {
            StudentInfoFrame.Visibility = Visibility.Hidden;
            StudentNameList.UnselectAll();
            StudentRollList.UnselectAll();
            StudentAddmList.UnselectAll();
            NameDisplay.Text = "";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            IssueFrame.Visibility = Visibility.Hidden;
            AdmissionNumberForIssue.Text = "";
            AccNumberForIssue.Text = "";
            IStudentname.Text = "";
            IBookname.Text = "";
            try
            {
                StudentData.ItemsSource = sql.GetDataTable("studentbookdata","admissionnumber",student.AdmissionNumber,new string[]{ "BookName","Acc","DateOfIssue","DateOfReturn"}).DefaultView;
            }
            catch { }
        }

        private void CloseReturn_Click(object sender, RoutedEventArgs e)
        {
            ReturnFrame.Visibility = Visibility.Hidden;
            IBooknameReturn.Text = "";
            IStudentnameReturn.Text = "";
            AdmissionNumberForReturn.Text = "";

            try
            {
                StudentData.ItemsSource = StudentData.ItemsSource = sql.GetDataTable("studentbookdata", "admissionnumber", student.AdmissionNumber, new string[] { "BookName", "Acc", "DateOfIssue", "DateOfReturn" }).DefaultView;
            }
            catch { }

        }
        #endregion

        #region Issue
        private void SetupIssue()
        {
            string Day = Convert.ToString(DateTime.Now.Day);
            string Month = Convert.ToString(DateTime.Now.Month);
            string Year = Convert.ToString(DateTime.Now.Year);
            DateTextBox.Text = Day + "-" + Month + "-" + Year;
            IssueFrame.Visibility = Visibility.Visible;
        }

        private void IssueBook_Click(object sender, RoutedEventArgs e)
        {
            SetupIssue();
            AdmissionNumberForIssue.Text = student.AdmissionNumber;
        }

        

        private void Check_Student_Name_Click(object sender, RoutedEventArgs e)
        {
            student = new Student(AdmissionNumberForIssue.Text);

            IStudentname.Text = student.Name!=null?student.Name:"Error";
           
        }

        private void Check_Book_Name_Click(object sender, RoutedEventArgs e)
        {
            book = new Book(AccNumberForIssue.Text);
            IBookname.Text = "";
            
            IBookname.Text = book.Name!=null?book.Name:"Error";
                    
        }

        private void IssueButton_Click(object sender, RoutedEventArgs e)
        {
            book = new Book(AccNumberForIssue.Text);
            student = new Student(AdmissionNumberForIssue.Text);

            IBookname.Text = "";

            //Main Process
            if(student.Issue(book, DateTextBox.Text))
            {
                IBookname.Text = "";
                IBookname.Text = book.Name != null ? book.Name : "Error";
                IStudentname.Text = student.Name != null ? student.Name : "Error";
                IssueFrame.Visibility = Visibility.Hidden;

                AdmissionNumberForIssue.Text = "";
                AccNumberForIssue.Text = "";
                IStudentname.Text = "";
                IBookname.Text = "";
                try
                {
                    StudentData.ItemsSource = sql.GetDataTable("studentbookdata", "admissionnumber",student.AdmissionNumber, new string[] { "BookName", "Acc", "BookNo", "DateOfIssue", "DateOfReturn" }).DefaultView;
                }
                catch { }
            } 
        }
        #endregion

        #region Return

        private void SetUpReturn()
        {
            string Day = Convert.ToString(DateTime.Now.Day);
            string Month = Convert.ToString(DateTime.Now.Month);
            string Year = Convert.ToString(DateTime.Now.Year);
            DateTextBoxReturn.Text = Day + "-" + Month + "-" + Year;
            ReturnFrame.Visibility = Visibility.Visible;
        }
        private void ReturnBook_Click(object sender, RoutedEventArgs e)
        {
            SetUpReturn();
            AdmissionNumberForReturn.Text = student.AdmissionNumber;
        }
        private static string ReturnCurrentBookAcc;

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            SetUpReturnAccData();
            student = new Student(AdmissionNumberForReturn.Text);
            book = new Book(student.IssuedBook);

            if(student.Return(book, DateTextBoxReturn.Text))
            {
                ReturnCurrentBookAcc = "";
                FineChanged(student);
                ReturnFrame.Visibility = Visibility.Hidden;
                IBooknameReturn.Text = "";
                IStudentnameReturn.Text = "";
                AdmissionNumberForReturn.Text = "";

                try
                {
                    DataTable dt = new DataTable();

                    StudentData.ItemsSource = sql.GetDataTable("studentbookdata", "admissionnumber",student.AdmissionNumber, new string[] { "BookName", "Acc", "BookNo", "DateOfIssue", "DateOfReturn" }).DefaultView;
                }
                catch { }
            }
            else
            {
                ReturnFrame.Visibility = Visibility.Hidden;
            }

        }

        private void SetUpReturnAccData()
        {
            student = new Student(AdmissionNumberForReturn.Text);
            book = new Book(student.IssuedBook);

            IStudentnameReturn.Text = student.Name;
            ReturnCurrentBookAcc = student.IssuedBook;
            IBooknameReturn.Text = book.Name;

        }

        private void Check_Student_NameReturn_Click(object sender, RoutedEventArgs e)
        {
            SetUpReturnAccData();
            Dictionary<string, string> addm = sql.GetSingleColumn("CurrentBooks", "AdmissionNumber", AdmissionNumberForReturn.Text, new string[] { "Acc" });
            book = new Book(sql.GetSingleColumn("CurrentBooks", "AdmissionNumber", AdmissionNumberForReturn.Text, new string[] { "Acc" })["Acc"]);
            Student s = new Student(AdmissionNumberForReturn.Text);
        }


        #endregion

        #region Student & Book

        private void AddFine_Click(object sender, RoutedEventArgs e)
        {
            student = new Student(AdmissionNumberDisplay.Text);
            
            if (student.IssuedBook==null)
            {
                App.ErrorBox("This stundent did not issue any book currently, so he cannot be fined");
            }
            else
            {
                Fine fine = new Fine(this,student);
                fine.Show();
            }
        }

        private void IssueFromBookinfoFrame_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentlyIssued.Text.Equals("Nobody"))
            {
                SetupIssue();
                AccNumberForIssue.Text = BookAcc.Text;
                BookInfoGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                App.ErrorBox("This book is already issued by someone");
            }

        }

        private void ReurnFromBookInfoFrame_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentlyIssued.Text.Equals("Nobody"))
            {
                SetUpReturn();
                AdmissionNumberForReturn.Text = CurrentlyIssued.Text;
                BookInfoGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                App.ErrorBox("This book not currently issued and cannot be returned without issuing");
            }
        }

        private void SearchByStudentID_Click(object sender, RoutedEventArgs e)
        {
            NameDisplay.Text = "";
            StudentFrame.Visibility = Visibility.Hidden;
            ClassList.UnselectAll();

            try
            {

                student = new Student(StudentID.Text);

                if (student.Name != null)
                {
                    setStudentFrame(student);
                    
                }
                else
                {
                    App.ErrorBox("Student not found");
                }

               

            }
            catch(Exception ex)
            {
                if (ex.Message.Contains("exist"))
                {
                    App.ErrorBox("Could not find any student with the Admission number " + StudentID.Text);
                }
            }
        }

        private void SearchByBookID_Click(object sender, RoutedEventArgs e)
        {

            book = new Book(BookID.Text);
            
            BookName.Text = book.Name;
            BookAcc.Text = book.Acc;
            BookNo.Text = book.BookNo;

            if (book.IssuedBy==null)
            {
                CurrentlyIssued.Text = "Nobody";
            }
            else
            {
                CurrentlyIssued.Text = book.IssuedBy;
            }
            if (book.Name==null)
            {
                App.ErrorBox("Could not find any book with the Acc Number " + BookID.Text);
            }
            else
            {
                BookData.ItemsSource = sql.GetDataTable("BookStudentData", "Acc", BookAcc.Text, new string[] { "AdmissionNumber", "StudentName", "DateOfIssue", "DateOfReturn" }).DefaultView;

                SetVisibility(BookInfoGrid);
                ClassList.UnselectAll();
            }
        }

        

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Do you really want to delete this book? The old records cannot be changed!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                book = new Book(BookAcc.Text);
                if (book.IssuedBy != null)
                {
                    var student = new Student(book.IssuedBy);
                    App.ErrorBox(string.Format("This book is issued by {0} of class {1} with admission number {2}.\nPlease return this book to continue", student.Name, student.Class, student.AdmissionNumber));
                }
                else
                {
                    book.DelFromDB();

                    BookInfoGrid.Visibility = Visibility.Hidden;
                    BookName.Text = "";
                    BookAcc.Text = "";
                    CurrentlyIssued.Text = "";
                    App.SuccessBox("Book deleted!");
                }
                
                
            }
        }

        private void StudentInfoSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CurrentlyIssued.Text.Equals("Nobody"))
                {
                    BookInfoGrid.Visibility = Visibility.Hidden;

                    student = new Student(CurrentlyIssued.Text);

                    if(student.IsAvailable)
                    {
                        NameDisplay.Text = student.Name;
                        AdmissionNumberDisplay.Text = student.AdmissionNumber;
                        RollNumberDisplay.Text = student.Roll;
                        ClassDisplay.Text = student.Class;

                        if (student.Fine.Equals("Null"))
                        {
                            FineAmount.Text = "Null";
                        }
                        else
                        {
                            FineAmount.Text = student.Fine;
                        }

                        StudentData.ItemsSource = StudentData.ItemsSource = sql.GetDataTable("studentbookdata", "admissionnumber", student.AdmissionNumber, new string[] { "BookName", "Acc", "BookNo", "DateOfIssue", "DateOfReturn" }).DefaultView;

                        StudentInfoFrame.Visibility = Visibility.Visible;
                        StudentFrame.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        App.ErrorBox("Student not found");
                    }     
                }
                else
                {
                    App.ErrorBox("This book is not currently issued");
                }
            }
            catch
            {

            }
           
        }

        private void SearchByBookName_Click(object sender, RoutedEventArgs e)
        {
            BookList.Items.Clear();

            List<Dictionary<string,string>> books=sql.GetDataListUsingLike("books", "Name", BookNameTextBox.Text, "Acc", "desc",new string[] { "Name","Acc","BookNo"});

            foreach(Dictionary<string,string> i in books)
            {
                BookList.Items.Add(i["Name"] + "\t" + i["Acc"]);
            }

            BookList.SelectionChanged += BookListSelectionchange;

            SetVisibility(BookSuggestions);

            ClassList.UnselectAll();
        }

        private void EditStudent_Click(object sender, RoutedEventArgs e)
        {
            StudentInfoFrame.Visibility = Visibility.Hidden;
            student = new Student(AdmissionNumberDisplay.Text);
            EditStudentInfo ESI = new EditStudentInfo(student,this);
            ESI.Show();
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this student?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                student = new Student(AdmissionNumberDisplay.Text);
                student.DelFromDB();

                StudentInfoFrame.Visibility = Visibility.Hidden;
                ClasslistSelectionChanged(sender, e);
                App.SuccessBox("Student deleted!");
            }
        }
        #endregion

        public void FineChanged(Student s)
        {
            student = new Student(s.AdmissionNumber);

            if (student.Fine.Equals("Null"))
            {
                FineAmount.Text = "Null";
            }
            else
            {
                FineAmount.Text = student.Fine;
            }
        }
        public void NewClass()
        {
            LoadClass();
        }
        public void NewStudent()
        {
            LoadStudents();
        }

        private void LoadFindBookList(ListBox l,string BookName,List<string> currentBooks)
        {
            l.Items.Clear();

            List<Dictionary<string, string>> books = sql.GetDataListUsingLike("books", "Name", BookName, "Name", "asc", new string[] { "Name", "Acc", "BookNo" });
           

            foreach (Dictionary<string, string> i in books)
            {
                if (!currentBooks.Contains(i["Acc"]))
                {
                    l.Items.Add(new Book(i["Acc"]));
                }
                
            }
        }

        private void LoadFindStudentsList(ListBox l,string studentName,List<string> CurrentStudents, bool? Issue)
        {
            l.Items.Clear();

            List<Dictionary<string, string>> students = sql.GetDataListUsingLike("students", "Name", studentName, "Name","asc",new string[] { "Name", "AdmissionNumber" });

            foreach (Dictionary<string, string> i in students)
            {
                if (Issue == null)
                {
                    l.Items.Add(new Student(i["AdmissionNumber"]));
                }
                if (((bool)Issue) ? !CurrentStudents.Contains(i["AdmissionNumber"]):CurrentStudents.Contains(i["AdmissionNumber"]))
                {
                    l.Items.Add(new Student(i["AdmissionNumber"]));
                }
            }
        }

        private void FindBookIssue_TextChanged(object sender, TextChangedEventArgs e)
        {
            List< string> booksCurrent = sql.GetDataList("currentbooks","Acc");

            LoadFindBookList(FindBookListIssue, FindBookIssue.Text,booksCurrent);
        }

        private void FindBookListIssue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AccNumberForIssue.Text = ((Book)FindBookListIssue.SelectedItem).Acc;
                FindBookListIssue.UnselectAll();
            }
            catch { }
            
        }

        private void FindStudentIssue_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> studentsCurrent = sql.GetDataList("currentbooks", "AdmissionNumber");
            LoadFindStudentsList(FindStudentListIssue, FindStudentIssue.Text,studentsCurrent,true);
        }

        private void FindStudentListIssue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdmissionNumberForIssue.Text = ((Student)FindStudentListIssue.SelectedItem).AdmissionNumber;
        }

        private void FindStudentReturn_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> studentsCurrent = sql.GetDataList("currentbooks", "AdmissionNumber");
            LoadFindStudentsList(FindStudentListReturn, FindStudentReturn.Text,studentsCurrent,false);
        }

        private void FindStudentListReturn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdmissionNumberForReturn.Text = ((Student)FindStudentListReturn.SelectedItem).AdmissionNumber;
        }

        private void SearchByStudentName_Click(object sender, RoutedEventArgs e)
        {
            StudentList.Items.Clear();

            List<Dictionary<string, string>> books = sql.GetDataListUsingLike("Students", "Name", StudentNameTextBox.Text, "AdmissionNumber", "desc", new string[] { "Name", "AdmissionNumber"});

            foreach (Dictionary<string, string> i in books)
            {
                StudentList.Items.Add(i["Name"]+" with Admission Number "+i["AdmissionNumber"]);
            }

            StudentList.SelectionChanged += StudentList_SelectionChanged;

            SetVisibility(StudentSuggestions);

            ClassList.UnselectAll();
        }

        private void setStudentFrame(Student student)
        {
            NameDisplay.Text = "";
            StudentFrame.Visibility = Visibility.Hidden;
            ClassList.UnselectAll();

            try
            {

                if (student.Name != null)
                {
                    NameDisplay.Text = student.Name;
                    AdmissionNumberDisplay.Text = student.AdmissionNumber;
                    RollNumberDisplay.Text = student.Roll;
                    ClassDisplay.Text = student.Class;

                    StudentData.ItemsSource = sql.GetDataTable("studentbookdata", "admissionnumber", student.AdmissionNumber, new string[] { "BookName", "Acc", "BookNo", "DateOfIssue", "DateOfReturn" }).DefaultView;

                    SetVisibility(StudentInfoFrame);
                }
                else
                {
                    App.ErrorBox("Student not found");
                }

                if (student.Fine.Equals("Null"))
                {
                    FineAmount.Text = "Null";
                }
                else
                {
                    FineAmount.Text = student.Fine;
                }

            }
            catch { }
        }

        private void StudentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Student student = new Student(StudentList.SelectedItem.ToString().Split(' ')[StudentList.SelectedItem.ToString().Split(' ').Length - 1]);
                setStudentFrame(student);
            }
            catch { }
            
        }

        private void CloseStudentSuggestion_Click(object sender, RoutedEventArgs e)
        {
            StudentSuggestions.Visibility = Visibility.Hidden;
        }

        private void StudentFrameClose_Click(object sender, RoutedEventArgs e)
        {
            StudentFrame.Visibility = Visibility.Hidden;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
            switch (e.Key)
            {
                case Key.Down:
                    try
                    {
                        ClassList.SelectedIndex = (ClassList.Items.IndexOf(ClassList.SelectedItem)) + 1;
                    }
                    catch
                    {

                    }
                    
                    break;
                case Key.Up:

                    try
                    {
                        ClassList.SelectedIndex = (ClassList.Items.IndexOf(ClassList.SelectedItem)) - 1;
                    }
                    catch { }
                    break;


            }
            
        }
    }
}

