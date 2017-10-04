using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PlsWork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Note TransferedNote;

        public static string DB_PATH = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "NoteManager.sqlite"));

    }
}
