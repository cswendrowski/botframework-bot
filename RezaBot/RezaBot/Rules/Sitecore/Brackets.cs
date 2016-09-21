using PullRequestReviewService.Models;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    /// <summary>
    /// Check for EOL newline
    /// </summary>
    public class Brackets : SitecoreBaseRule
    {
        public Brackets() : base()
        {
            FileTypesToCheck.Add("cs");
        }

        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

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

            return messages;
        }
    }
}
