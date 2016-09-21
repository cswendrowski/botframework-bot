using PullRequestReviewService.Models;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    public class Whitespaces : SitecoreBaseRule
    {
        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

            // Check for extra whitespaces
            foreach (var changedLine in addedLines)
            {
                if (changedLine.Line.TrimStart().Contains("  "))
                {
                    messages.Add(new CodeComment
                    {
                        File = file,
                        Line = changedLine,
                        Comment = "Please remove the extra white spaces on this line, thanks!"
                    });

                    issueFound = true;
                }
            }

            return messages;
        }
    }
}
