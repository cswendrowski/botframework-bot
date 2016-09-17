using RezaBot.Models;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    public class Whitespaces : SitecoreBaseRule
    {
        protected override bool Review(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines)
        {
            // Check for extra whitespaces
            foreach (var changedLine in addedLines)
            {
                if (changedLine.Line.TrimStart().Contains("  "))
                {
                    GitService.WriteComment(file, changedLine, "Please remove the extra white spaces on this line, thanks!", prNumber);
                    return true;
                }
            }

            return false;
        }
    }
}
