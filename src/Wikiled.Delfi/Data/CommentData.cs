using System;
using System.Net;

namespace Wikiled.Delfi.Data
{
    public class CommentData
    {
        public string Id { get; set; }

        public string Author { get; set; }

        public bool IsAnonymous { get; set; }

        public IPAddress Address { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }

        public int UpVote { get; set; }

        public int DownVote { get; set; }

        public override string ToString()
        {
            return $"{Author}({Address})({Date}) - {Text}";
        }
    }
}
