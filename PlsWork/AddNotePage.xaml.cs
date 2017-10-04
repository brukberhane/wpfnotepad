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

            //Inset the Note
            //tempNote = ((App)Application.Current).TransferedNote;
            //db.AddNote(tempNote);

            //Prepare temp file location
            //path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "test1.bruk");
            path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test1.bruk");

            //this.NavigationService.Navigating += NavigationService_Navigating;

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
            tempNote.Content = SaveXamlPackage(path);

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
            tempNote.Content = SaveXamlPackage(path);
            db.UpdateNote(tempNote);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();

            //var btn = sender as Button;
            //btn.Visibility = Visibility.Collapsed;
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;

                titleEditText.RaiseEvent(new RoutedEventArgs(RichTextBox.TextChangedEvent));
                //db.AddNote(tempNote);
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
            //MessageBox.Show("Herro?");
        }

        private void titleEditText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTitleEmpty(titleEditText, titleTextBlock);

            //trange = new TextRange(titleEditText.Document.ContentStart, titleEditText.Document.ContentEnd);
            //tempNote.Title = trange.Text.Trim();
            //TextRange crange = new TextRange(contentTextBox.Document.ContentStart, contentTextBox.Document.ContentEnd);
            //tempNote.Content = crange.Text.Trim();

            //db.UpdateNote(tempNote);

            //savingButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            //tempNote.Content = SaveXamlPackage(path);
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
            //SaveXamlPackage("B:\\test.xaml");
            //tempNote.Content = LoadXamlPackage("B:\\test.xaml");
        }

        string SaveXamlPackage(string _fileName)
        {
            TextRange range;
            FileStream fStream;
            range = new TextRange(contentTextBox.Document.ContentStart, contentTextBox.Document.ContentEnd);
            fStream = new FileStream(_fileName, FileMode.Create);
            //range.Text = Regex.Replace(range.Text, "[\']", "\'\'");
            this.Dispatcher.Invoke(() => { range.Save(fStream, DataFormats.Rtf); });

            fStream.Close();


            
            //LoadFile(_fileName);
            LoadXamlPackage(_fileName);
            File.Delete(_fileName);

            TempContent = Regex.Replace(TempContent, "[\']", "\'\'");

            return TempContent;
        }

        void LoadXamlPackage(string _fileName)
        {
            TextRange range;
            FileStream fStream;
            if (File.Exists(_fileName))
            {
                try
                {
                    range = new TextRange(routerbox.Document.ContentStart, routerbox.Document.ContentEnd);
                    fStream = new FileStream(_fileName, FileMode.OpenOrCreate);
                    StreamReader reader = new StreamReader(fStream);
                    TempContent = reader.ReadToEnd();

                    this.Dispatcher.Invoke(() => { range.Load(fStream, DataFormats.Text); });
                    //MessageBox.Show(TempContent);
                    byte[] byteArray = Encoding.UTF8.GetBytes(TempContent);
                    MemoryStream stream = new MemoryStream(byteArray);
                    this.Dispatcher.Invoke(() => { range.Load(stream, DataFormats.Rtf); });
                    fStream.Close();
                } catch (System.ExecutionEngineException eee)
                {
                    Console.WriteLine("Error in AddNotePage, \n" + eee.Message);
                }
            }
        }

        private void LoadFile(string path)
        {
            TextRange range;
            FileStream fStream;
            if (File.Exists(path))
            {
                range = new TextRange(contentTextBox.Document.ContentStart, contentTextBox.Document.ContentEnd);
                fStream = new FileStream(path, FileMode.OpenOrCreate);
                StreamReader reader = new StreamReader(fStream);
                TempContent = reader.ReadToEnd();

                range.Load(fStream, DataFormats.Text);
                //MessageBox.Show(TempContent);
                //byte[] byteArray = Encoding.UTF8.GetBytes(TempContent);
                //MemoryStream stream = new MemoryStream(byteArray);
                //range.Load(stream, DataFormats.Rtf);
                fStream.Close();
            }
        }

        private void savingButton_Click(object sender, RoutedEventArgs e)
        {
            tempNote.Title = trange.Text.Trim();
            tempNote.Content = SaveXamlPackage(path);
            db.UpdateNote(tempNote);
        }
    }
}
