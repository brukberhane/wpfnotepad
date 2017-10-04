using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlsWork
{
    public class Note
    {
        [SQLite.Net.Attributes.PrimaryKey, SQLite.Net.Attributes.AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreationDate { get; set; }

        public Note()
        {

        }

        public Note(string title, string content)
        {
            Title = title;
            Content = content;
            CreationDate = DateTime.Now.ToString();
        }

    }
}
