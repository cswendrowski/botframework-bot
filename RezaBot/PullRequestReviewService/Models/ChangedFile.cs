using System;
using System.Collections.Generic;
using System.Linq;

namespace PullRequestReviewService.Models
{
    public class ChangedFile
    {
        public string SHA { get; set; }

        public string FileName { get; set; }

        public string Status { get; set; }

        public string Patch { get; set; }

        public List<CodeLine> ChangedLines = new List<CodeLine>();

        public string FileType
        {
            get
            {
                var split = FileName.Split('.');
                return split[split.Count() - 1];
            }
        }

        public void ConvertPatchToChangedLines()
        {
            if (Patch == null)
            {
                Console.WriteLine(string.Format("File {FileName} has no Patch - probably too large"));
                return;
            }

            // First line is skipped since it is just a list of lines deleted and added
            int x = 0;
            foreach (var line in Patch.Split('\n'))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var changedLine = new CodeLine(line);

                    changedLine.LineNumber = x;

                    if (changedLine.WasChanged)
                    {
                        ChangedLines.Add(changedLine);
                    }
                }
                x++;
            }

            ChangedLines = ChangedLines.Skip(1).ToList();
        }
    }
}
