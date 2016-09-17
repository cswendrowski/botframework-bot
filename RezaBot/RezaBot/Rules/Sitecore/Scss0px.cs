using RezaBot.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RezaBot.Rules.Sitecore
{
    public class Scss0px : SitecoreBaseRule
    {
        protected override bool Review(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines)
        {
            // Check for 0px in .scss files
            if (file.FileType == "scss")
            {
                foreach (var changedLine in addedLines)
                {
                    if (Regex.IsMatch(changedLine.Line, @"\b0px"))
                    {
                        GitService.WriteComment(file, changedLine, "Please update `0px` to be `0` in this SCSS rule, thanks!", prNumber);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
