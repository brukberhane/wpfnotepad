using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlsWork
{
    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {

        private ObservableCollection<Note> notes;
        private DatabaseHelper db;
        private ObservableCollection<Note> filtered;
        private MainWindow mw;
        private bool gv = false;

        public TestPage()
        {
            InitializeComponent();
            
            // initiate link to database with helper class
            db = new DatabaseHelper();
            // Check whether the databse exists and if not create
            db.CreateDatabase();
            RefreshNotesList();

            autoCompleteBox.ItemsSource = notes;
            lelele.ItemsSource = notes;

            mw = Application.Current.MainWindow as MainWindow;
            mw.backButton.Visibility = Visibility.Collapsed;

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var stack = btn.Parent as StackPanel;
            var note = stack.DataContext as Note;

            db.DeleteNote(note);
            notes.Remove(note);

            RefreshNotesList();

        }

        private void RefreshNotesList()
        {
            //retrieving notes;
            notes = db.GetNotesList();
            // Setting the list to ListView
            lelele.ItemsSource = notes;
        }

        private void lelele_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var sNote = lelele.SelectedItem as Note;
            this.NavigationService.Navigate(new NoteDetails(sNote));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mw.backButton.Visibility = Visibility.Collapsed;
        }

        private void autoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void autoCompleteBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }
    }
}
