using RezaBot.Models;
using RezaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Rules.Sitecore
{
    /// <summary>
    /// Check for EOL newline
    /// </summary>
    public class EndOfLine : SitecoreBaseRule
    {
        public EndOfLine(IGitService gitService) : base(gitService)
        {
        }

        protected override bool Review(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines)
        {
            if (addedLines.Any(x => x.Line.Contains("No newline at end of file")))
            {
                _gitService.WriteComment(file, file.ChangedLines.Last(), "Please add a newline at the End of the File, thanks!", prNumber);

                return true;
            }
            else if (removedLines.Any(x => x.Line.Contains("No newline at end of file")))
            {
                _gitService.WriteComment(file, file.ChangedLines.Last(), "Thanks for adding a newline at the End of File!", prNumber);
            }

            return false;
        }
    }
}
