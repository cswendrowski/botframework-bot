using RezaBot.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RezaBot.Rules.Sitecore
{
    public class ExtraNewLine : SitecoreBaseRule
    {
        protected override bool Review(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines)
        {
            // Check for extra newlines
            var lastWasNewline = false;
            foreach (var changedLine in allLines)
            {
                // We don't want to check for 2 newlines seperated by patches, so we reset whenever we hit a delete
                // Patches are always in the format of DELETES -> ADDS
                if (changedLine.WasDeleted)
                {
                    lastWasNewline = false;
                }

                // Double newline
                else if (changedLine.IsNewline && lastWasNewline)
                {
                    GitService.WriteComment(file, changedLine, "Please remove this extra newline, thanks!", prNumber);
                    return true;
                }
                else
                {
                    lastWasNewline = changedLine.IsNewline;
                }

                if (CheckNewlinesAroundTags(prNumber, file, changedLine, allLines))
                {
                    // Issues found in sub-method, return true
                    return true;
                }
            }

            return false;
        }

        private bool CheckNewlinesAroundTags(int prNumber, ChangedFile file, CodeLine changedLine, List<CodeLine> allLines)
        {
            // Only check newlines around open / closing tags in written files
            if (file.FileType != "item" && changedLine.IsNewline)
            {
                var changedLineIndex = allLines.IndexOf(changedLine);

                // Newline after opening tag ( { or <html tag> )
                if (changedLineIndex > 0)
                {
                    var lastLine = allLines[changedLineIndex - 1];

                    if (lastLine.WasAdded && (lastLine.Line.Contains("{") || Regex.IsMatch(lastLine.Line, @"<[A-z]+>(?!\(\))")))
                    {
                        GitService.WriteComment(file, changedLine, "Please remove this extra newline after an opening tag, thanks!", prNumber);
                        return true;
                    }
                }

                // Newline before closing tag ( } or </html tag> )
                if (changedLineIndex + 1 < allLines.Count)
                {
                    var nextLine = allLines[changedLineIndex + 1];

                    if (nextLine.WasAdded && (nextLine.Line.Contains("}") || Regex.IsMatch(nextLine.Line, @"</[A-z]+>(?!\(\))")))
                    {
                        GitService.WriteComment(file, changedLine, "Please remove this extra newline before a closing tag, thanks!", prNumber);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
