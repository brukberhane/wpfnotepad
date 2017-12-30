using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.IO;

namespace PlsWork
{
    class DatabaseHelper
    {

        public void CreateDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=NotesDatabse.sqlite"))
            {
                conn.Open();
                string sql = "CREATE TABLE IF NOT EXISTS Notes (id INTEGER PRIMARY KEY AUTOINCREMENT , title string, content string, createdate string)";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                }
            }

        }


        public void AddNote(Note n)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=NotesDatabse.sqlite")) {
                conn.Open();
            string sql = "INSERT  INTO Notes (title, content, createdate) values ('"+n.Title+"', '"+n.Content+"', '"+n.CreationDate+"') ";
            using (SQLiteCommand command = new SQLiteCommand(sql, conn))
            {
                command.ExecuteNonQuery();
            }
        }
        }

        public ObservableCollection<Note> GetNotesList()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=NotesDatabse.sqlite"))
            {
                conn.Open();
                string sql = "SELECT * FROM Notes ORDER BY id DESC";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                ObservableCollection<Note> notes = new ObservableCollection<Note>();
                int tempINt = 1;
                while (reader.Read())
                {
                    notes.Add(new Note
                    {
                        Id = int.Parse(reader["id"].ToString()),
                        Title = reader["title"].ToString(),
                        Content = reader["content"].ToString(),
                        CreationDate = reader["createdate"].ToString()
                    });
                    tempINt++;
                }

                return notes;
            }
        }

        public void UpdateNote(Note n)
        {

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=NotesDatabse.sqlite"))
            {
                conn.Open();
                string sql = "UPDATE Notes SET title = '" + n.Title + "', content = '" + n.Content + "' WHERE id = " + n.Id;
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                }
                GetNotesList();
            }
        }

        public void DeleteNote(Note n)
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=NotesDatabse.sqlite"))
            {
                conn.Open();
                string sql = "DELETE FROM Notes WHERE id = " + n.Id;
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAllNotes()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=NotesDatabse.sqlite"))
            {
                conn.Open();
                string sql = "DELETE FROM Notes";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
