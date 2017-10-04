using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlsWork
{
    public class DBHelper
    {
        private bool CheckFileExists(string fileName)
        {
                //var store = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                if (File.Exists(fileName))
                {
                    return true;
                }
            else
            {
                return false;
            }
        }

        public void CreateDatabase(string DB_PATH)
        {
            if (!CheckFileExists(DB_PATH))
            {
                using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new
                    SQLite.Net.Platform.Win32.SQLitePlatformWin32(), DB_PATH))
                {
                    conn.CreateTable<Note>();
                }
            }
        }

        public void InsertNote(Note note)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new 
                SQLite.Net.Platform.Win32.SQLitePlatformWin32(), App.DB_PATH))
            {
                conn.RunInTransaction(() =>
                {
                    conn.Insert(note);
                });
            }
        }

        public Note ReadNote(int noteId)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new 
                SQLite.Net.Platform.Win32.SQLitePlatformWin32(), App.DB_PATH))
            {
                var existingNote = conn.Query<Note>
                    ("select * from Notes where Id =" + noteId).FirstOrDefault();
                return existingNote;
            }
        }

        public ObservableCollection<Note> ReadAllNotes()
        {
            try
            {
                using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new 
                    SQLite.Net.Platform.Win32.SQLitePlatformWin32(), App.DB_PATH))
                {
                    List<Note> myCollection = conn.Table<Note>().ToList();
                    ObservableCollection<Note> notes = new ObservableCollection<Note>(myCollection);
                    return notes;
                }
            }
            catch
            {
                return null;
            }
        }

        public void UpdateNote(Note note)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new
                SQLite.Net.Platform.Win32.SQLitePlatformWin32(), App.DB_PATH))
            {
                var existingNote = conn.Query<Note>
                    ("select * from Notes where Id =" + note.Id).FirstOrDefault();
                if (existingNote != null)
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Update(note);
                    });
                }
            }
        }

        public void DeleteNote(int noteId)
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new
                SQLite.Net.Platform.Win32.SQLitePlatformWin32(), App.DB_PATH))
            {
                var existingNote = conn.Query<Note>
                    ("select * from Notes where Id =" + noteId).FirstOrDefault();
                if (existingNote != null)
                {
                    conn.RunInTransaction(() =>
                    {
                        conn.Delete(existingNote);
                    });
                }
            }
        }

        public void DeleteAllNotes()
        {
            using (SQLite.Net.SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new
                SQLite.Net.Platform.Win32.SQLitePlatformWin32(), App.DB_PATH))
            {
                conn.DropTable<Note>();
                conn.CreateTable<Note>();
                conn.Dispose();
                conn.Close();
            }
        }

    }
}
