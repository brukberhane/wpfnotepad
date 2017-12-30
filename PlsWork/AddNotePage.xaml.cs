using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;

namespace PlsWork
{
    /// <summary>
    /// Interaction logic for AddNotePage.xaml
    /// </summary>
    public partial class AddNotePage : Page
    {

        public Note tempNote = new Note();
        private DatabaseHelper db = new DatabaseHelper();
        private string TempContent;
        private MainWindow mw;
        private TextRange trange;
        private string path;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public AddNotePage()
        {
            InitializeComponent();

            path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test1.bruk");

            MessageBox.Show(tempNote.Title + "\n" + tempNote.Id);

            CheckTitleEmpty(titleEditText, titleTextBlock);
            CheckTitleEmpty(contentTextBox, contentTextBlock);

        }

        public AddNotePage(Note note)
        {
            InitializeComponent();

            path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test1.bruk");

            tempNote = note;

            mw = Application.Current.MainWindow as MainWindow;
            mw.backButton.Click += BackButton_Click;

            trange = new TextRange(titleEditText.Document.ContentStart, titleEditText.Document.ContentEnd);
            trange.Text = tempNote.Title;
            tempNote.Content = GetContent();

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            
            timedSave();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            db.UpdateNote(tempNote);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            trange = new TextRange(titleEditText.Document.ContentStart, titleEditText.Document.ContentEnd);
            tempNote.Title = (Regex.Replace(trange.Text, "[\']", "\'\'")).Trim();
            tempNote.Content = GetContent();
            db.UpdateNote(tempNote);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;

                worker.RunWorkerAsync();
            }
        }

        void timedSave()
        {
            var dispatchTimer = new DispatcherTimer();
            dispatchTimer.Tick += DispatchTimer_Tick;
            dispatchTimer.Interval = TimeSpan.FromSeconds(5);
            dispatchTimer.Start();
        }

        private void DispatchTimer_Tick(object sender, EventArgs e)
        {
            worker.RunWorkerAsync();
        }

        private void titleEditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTitleEmpty(titleEditText, titleTextBlock);
        }

        private void CheckTitleEmpty(RichTextBox rtb, TextBlock txtblk)
        {
            if (IsEmpty(rtb))
            {
                txtblk.Visibility = Visibility.Visible;
            }
            else
            {
                txtblk.Visibility = Visibility.Hidden;
            }
        }

        private bool IsEmpty(RichTextBox rtb)
        {
            string text = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(text) == false)
                return false;
            else
            {
                if (rtb.Document.Blocks.OfType<BlockUIContainer>()
                    .Select(c => c.Child).OfType<Image>()
                    .Any())
                    return false;
            }
            return true;
        }

        private void contentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTitleEmpty(contentTextBox, contentTextBlock);
        }

        string GetContent()
        {
            string text;
            var crange = new TextRange(contentTextBox.Document.ContentStart, contentTextBox.Document.ContentEnd);
            using (var stream = new MemoryStream())
            {
                this.Dispatcher.Invoke(() => { crange.Save(stream, DataFormats.Rtf); });
                return text = Regex.Replace(Encoding.UTF8.GetString(stream.ToArray()), "[\']", "\'\'");
            }
        }

        private void savingButton_Click(object sender, RoutedEventArgs e)
        {
            tempNote.Title = trange.Text.Trim();
            tempNote.Content = GetContent();
            db.UpdateNote(tempNote);
        }
    }
}
