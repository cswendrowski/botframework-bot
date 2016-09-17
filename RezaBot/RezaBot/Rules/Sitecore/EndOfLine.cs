using RezaBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace RezaBot.Rules.Sitecore
{
    /// <summary>
    /// Check for EOL newline
    /// </summary>
    public class EndOfLine : SitecoreBaseRule
    {
        protected override bool Review(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines)
        {
            if (addedLines.Any(x => x.Line.Contains("No newline at end of file")))
            {
                GitService.WriteComment(file, file.ChangedLines.Last(), "Please add a newline at the End of the File, thanks!", prNumber);

                return true;
            }
            else if (removedLines.Any(x => x.Line.Contains("No newline at end of file")))
            {
                GitService.WriteComment(file, file.ChangedLines.Last(), "Thanks for adding a newline at the End of File!", prNumber);
            }

            return false;
        }
    }
}
