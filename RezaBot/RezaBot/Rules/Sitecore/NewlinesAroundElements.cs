using PullRequestReviewService.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RezaBot.Rules
{
    public class NewlinesAroundElements : SitecoreBaseRule
    {
        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

            foreach (var changedLine in file.ChangedLines)
            {
                // Only check newlines around open / closing tags in written files
                if (file.FileType != "item" && changedLine.IsNewline)
                {
                    var changedLineIndex = file.ChangedLines.IndexOf(changedLine);

                    // Newline after opening tag ( { or <html tag> )
                    if (changedLineIndex > 0)
                    {
                        var lastLine = file.ChangedLines[changedLineIndex - 1];

                        if (lastLine.WasAdded && (lastLine.Line.Contains("{") || Regex.IsMatch(lastLine.Line, @"<[A-z]+>(?!\(\))")))
                        {
                            messages.Add(new CodeComment
                            {
                                File = file,
                                Line = changedLine,
                                Comment = "Please remove this extra newline after an opening tag, thanks!"
                            });

                            issueFound = true;
                        }
                    }

                    // Newline before closing tag ( } or </html tag> )
                    if (changedLineIndex + 1 < file.ChangedLines.Count)
                    {
                        var nextLine = file.ChangedLines[changedLineIndex + 1];

                        if (nextLine.WasAdded && (nextLine.Line.Contains("}") || Regex.IsMatch(nextLine.Line, @"</[A-z]+>(?!\(\))")))
                        {
                            messages.Add(new CodeComment
                            {
                                File = file,
                                Line = changedLine,
                                Comment = "Please remove this extra newline before a closing tag, thanks!"
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
