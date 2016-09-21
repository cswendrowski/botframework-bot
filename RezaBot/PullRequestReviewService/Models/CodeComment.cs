namespace PullRequestReviewService.Models
{
    public class CodeComment
    {
        public ChangedFile File { get; set; }

        public CodeLine Line { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            var toReturn = string.Empty;

            if (File != null)
            {
                toReturn += "File " + File.FileName;
            }

            if (Line != null)
            {
                toReturn += " Line " + Line.LineNumber;
            }

            // No issues found comment will trigger this
            if (string.IsNullOrEmpty(toReturn))
            {
                return Comment;
            }

            return toReturn + ": " + Comment;
        }
    }
}
