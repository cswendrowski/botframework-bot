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
        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

            if (addedLines.Any(x => x.Line.Contains("No newline at end of file")))
            {
                messages.Add(new CodeComment
                {
                    File = file,
                    Line = file.ChangedLines.Last(),
                    Comment = "Please add a newline at the End of the File, thanks!"
                });

                issueFound = true;
            }
            else if (file.ChangedLines.Where(x => x.WasDeleted).Any(x => x.Line.Contains("No newline at end of file")))
            {
                messages.Add(new CodeComment
                {
                    File = file,
                    Line = file.ChangedLines.Last(),
                    Comment = "Thanks for adding a newline at the End of File!"
                });
            }

            return messages;
        }
    }
}
