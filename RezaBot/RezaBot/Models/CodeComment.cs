using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Models
{
    public class CodeComment
    {
        public ChangedFile File { get; set; }

        public CodeLine Line { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            return string.Format("File {0} Line {1}: {2}", File.FileName, Line.LineNumber, Comment);
        }
    }
}
