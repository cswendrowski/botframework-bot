namespace PullRequestReviewService.Models
{
    public class CodeLine
    {
        public bool WasAdded { get; set; }

        public bool WasDeleted { get; set; }

        public bool WasChanged { get; set; }

        public bool IsNewline { get; set; }

        public int LineNumber { get; set; }

        public string Line { get; set; }

        public CodeLine(string line)
        {
            if (line[0] == '+')
            {
                WasAdded = true;
                WasChanged = true;
                Line = line.Substring(1, line.Length - 1);
            }
            else if (line[0] == '-')
            {
                WasDeleted = true;
                WasChanged = true;
                Line = line.Substring(1, line.Length - 1);
            }
            else
            {
                WasChanged = false;
                WasAdded = false;
                Line = line;
            }

            if (string.IsNullOrEmpty(Line))
            {
                IsNewline = true;
            }
        }
    }
}
