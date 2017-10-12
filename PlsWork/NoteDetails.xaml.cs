using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for NoteDetails.xaml
    /// </summary>
    public partial class NoteDetails : Page
    {

        private Note note;
        private string TempContent;
        private TextRange trange;
        private TextRange crange;
        private string path;
        private DatabaseHelper db = new DatabaseHelper();
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public NoteDetails()
        {
            InitializeComponent();
        }

        public NoteDetails(Note n)
        {
            InitializeComponent();
            note = n;
            setData();

            MainWindow mw = Application.Current.MainWindow as MainWindow;
            mw.backButton.Click += BackButton_Click;

            path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "test1.bruk");

            trange = new TextRange(titleRichTextBox.Document.ContentStart, titleRichTextBox.Document.ContentEnd);
            crange = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);

            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            AutoSave(); //Starting the autosaving timer.
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("It has been saved!");
            db.UpdateNote(note);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            trange = new TextRange(titleRichTextBox.Document.ContentStart, titleRichTextBox.Document.ContentEnd);
            note.Title = (Regex.Replace(trange.Text, "[\']", "\'\'")).Trim();
            note.Content = getContent();
            db.UpdateNote(note);
        }

        void AutoSave()
        {
            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //note.Title = trange.Text.Trim();
            worker.RunWorkerAsync();
            //db.UpdateNote(note);
            //note.Content = SaveXamlPackage(path);  // await Task.Run(()=> getContent());;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            /*note.Title = trange.Text.Trim();
            note.Content = SaveXamlPackage(path);
            db.UpdateNote(note);*/
            worker.RunWorkerAsync();
        }

        string getContent()
        {
            //return SaveXamlPackage(path);

            string rtfText;
            TextRange crange = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
            using (var stream = new MemoryStream())
            {
                this.Dispatcher.Invoke(() => { crange.Save(stream, DataFormats.Rtf); });
                return rtfText = Regex.Replace(Encoding.UTF8.GetString(stream.ToArray()), "[\']", "\'\'");
            }

        }

        private void setData()
        {
            TextRange trange = new TextRange(titleRichTextBox.Document.ContentStart, titleRichTextBox.Document.ContentEnd);
            TextRange crange = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);

            trange.Text = note.Title;

            byte[] byteArray = Encoding.UTF8.GetBytes(note.Content);
            MemoryStream stream = new MemoryStream(byteArray);
            try
            {
                crange.Load(stream, DataFormats.Rtf);
            } catch (System.ArgumentException)
            {
                MessageBox.Show("The file wasn't Rtf Format");
                crange.Text = note.Content;
            }
        }

        private void titleRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void contentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LoadFromDb()
        {
            TextRange crange = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
            byte[] byteArray = Encoding.UTF8.GetBytes(note.Content);
            MemoryStream stream = new MemoryStream(byteArray);
            crange.Load(stream, DataFormats.Rtf);
            stream.Close();
        }

        /*string SaveXamlPackage(string _fileName)
        {
            TextRange range;
            FileStream fStream;
            range = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
            try
            {
                fStream = new FileStream(_fileName, FileMode.Create);
                //range.Text = Regex.Replace(range.Text, "[\']", "\'\'");
                this.Dispatcher.Invoke(() => { range.Save(fStream, DataFormats.Rtf); });
                fStream.Close();
            } catch (IOException ioe)
            {
                fStream = new FileStream("test11.bruk", FileMode.Create);
                this.Dispatcher.Invoke(() => { range.Save(fStream, DataFormats.Rtf); });
                fStream.Close();
            }


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
                range = new TextRange(routerbox.Document.ContentStart, routerbox.Document.ContentEnd);
                try
                {
                    fStream = new FileStream(_fileName, FileMode.OpenOrCreate);
                    StreamReader reader = new StreamReader(fStream);
                    TempContent = reader.ReadToEnd();

                    this.Dispatcher.Invoke(() => { range.Load(fStream, DataFormats.Rtf); });
                    //MessageBox.Show(TempContent);
                    byte[] byteArray = Encoding.UTF8.GetBytes(TempContent);
                    MemoryStream stream = new MemoryStream(byteArray);
                    this.Dispatcher.Invoke(() => { range.Load(stream, DataFormats.Rtf); });
                    fStream.Close();
                } catch (IOException ioe)
                {
                    Console.WriteLine("NoteDetails: " + ioe);
                    fStream = new FileStream("test11.bruk", FileMode.OpenOrCreate);
                    StreamReader reader = new StreamReader(fStream);
                    TempContent = reader.ReadToEnd();

                    this.Dispatcher.Invoke(() => { range.Load(fStream, DataFormats.Rtf); });
                    //MessageBox.Show(TempContent);
                    byte[] byteArray = Encoding.UTF8.GetBytes(TempContent);
                    MemoryStream stream = new MemoryStream(byteArray);
                    this.Dispatcher.Invoke(() => { range.Load(stream, DataFormats.Rtf); });
                    fStream.Close();
                }
            }
        }*/
    }
}
