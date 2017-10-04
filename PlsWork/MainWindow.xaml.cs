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
using System.Data.SQLite;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace PlsWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private DatabaseHelper db;
        private ObservableCollection<Note> notes;
        private SQLiteConnection conn;

        public MainWindow()
        {
            InitializeComponent();

            myFrame.Navigate(new TestPage());

            conn = new SQLiteConnection("Data Source=NotesDatabse.sqlite");
            conn.Open();
            db = new DatabaseHelper();
            db.CreateDatabase();
            //db.AddNote(new Note { Title = "lel", Content = "meow", CreationDate=DateTime.Now });
            notes = db.GetNotesList();
            //lelele.ItemsSource = notes;
            string temsql = "select * from Notes order by id desc";
            SQLiteCommand command = new SQLiteCommand(temsql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("Title: "+reader["title"].ToString()+", Id: "+reader["id"].ToString());
            //Important codeso refresh
            //db.UpdateNote(new Note { Title = "lel", Content = "meow", Id = 2 });
            notes = db.GetNotesList();
            Note note = notes.Where(i => i.Title == "mlem").FirstOrDefault();
            //lelele.ItemsSource = notes;
            
        }

        private void myFrame_Navigated(object sender, NavigationEventArgs e)
        {
            CheckBackButton();

            backButton.Click += backButton_Click;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (addnote.IsSelected)
            {
                //Create a new note to pass on to the page
                db.AddNote(new Note("", ""));
                //Get the list of notes by the Id's descending order.
                notes = db.GetNotesList();
                //Pass created note parameter (it's the first element because of it's ID)
                myFrame.Navigate(new AddNotePage(notes[0]));
                //Remove selection
                addnote.IsSelected = false;
                
            }
            else if (deleteall.IsSelected)
            {

                if (!myFrame.CanGoBack)
                {
                    
                }
                else
                {
                    MessageBox.Show("You have to go to the main page to delete all your notes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    deleteall.IsSelected = false;
                }

                //Probably add something here to clear the observablecollection

            }
            else if (HamburglerButton.IsSelected)
            {
                mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
                HamburglerButton.IsSelected = false;
            }
        }

        private void saveStuff()
        {

            string fileText = "file";

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, fileText);
            }
        }

        public void CheckBackButton()
        {
            if (myFrame.CanGoBack)
            {
                backButton.Visibility = Visibility.Visible;
            } else
            {
                backButton.Visibility = Visibility.Collapsed;
            }
        }

        private void saveStuff1()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text file|*.txt";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, "New Note");
            }
        }

        public void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (myFrame.CanGoBack)
            {
                myFrame.Navigate(new TestPage());
            }
            CheckBackButton();
        }
    }
}
