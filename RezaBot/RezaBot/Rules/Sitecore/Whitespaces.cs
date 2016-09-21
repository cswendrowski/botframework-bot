using PullRequestReviewService.Models;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    public class Whitespaces : SitecoreBaseRule
    {
        public Whitespaces() : base()
        {
            FileTypesToCheck.Add("cs");
            FileTypesToCheck.Add("cshtml");
            FileTypesToCheck.Add("js");
            FileTypesToCheck.Add("scss");
        }

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
                else if (changedLine.Line.Contains(" ;"))
                {
                    messages.Add(new CodeComment
                    {
                        File = file,
                        Line = changedLine,
                        Comment = "Please remove extra white spaces before the `;`, thanks!"
                    });

                    issueFound = true;
                }
                else if (changedLine.Line.Contains("; "))
                {
                    messages.Add(new CodeComment
                    {
                        File = file,
                        Line = changedLine,
                        Comment = "Please remove extra white spaces after the `;`, thanks!"
                    });

                    issueFound = true;
                }
            }

            return messages;
        }
    }
}
