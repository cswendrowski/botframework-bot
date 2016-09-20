using RezaBot.Models;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    public class ExtraNewLine : SitecoreBaseRule
    {
        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

            // Check for extra newlines
            var lastWasNewline = false;

            foreach (var changedLine in file.ChangedLines)
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
                    messages.Add(new CodeComment
                    {
                        File = file,
                        Line = changedLine,
                        Comment = "Please remove this extra newline, thanks!"
                    });

                    issueFound = true;
                }
                else
                {
                    lastWasNewline = changedLine.IsNewline;
                }
            }

            return messages;
        }
    }
}
