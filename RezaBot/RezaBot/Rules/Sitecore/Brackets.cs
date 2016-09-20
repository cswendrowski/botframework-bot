using RezaBot.Models;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    /// <summary>
    /// Check for EOL newline
    /// </summary>
    public class Brackets : SitecoreBaseRule
    {
        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

            // Check for brackets that aren't on a new line in non-excluded files
            if (file.FileType != "cshtml" && file.FileType != "js" && file.FileType != "item" && !file.FileType.Contains("proj"))
            {
                foreach (var changedLine in addedLines)
                {
                    if (changedLine.Line.Contains("{") || changedLine.Line.Contains("}"))
                    {
                        if (!string.IsNullOrWhiteSpace(changedLine.Line.Replace("{", "").Replace("}", "")))
                        {
                            messages.Add(new CodeComment
                            {
                                File = file,
                                Line = changedLine,
                                Comment = "Please update Brackets (`{` and `}`) to be on a new line, thanks!"
                            });

                            issueFound = true;
                        }
                    }
                }
            }

            return messages;
        }
    }
}
