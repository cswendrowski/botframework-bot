using PullRequestReviewService.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RezaBot.Rules.Sitecore
{
    public class Scss0px : SitecoreBaseRule
    {
        public Scss0px() : base()
        {
            FileTypesToCheck.Add("scss");
        }

        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

            foreach (var changedLine in addedLines)
            {
                if (Regex.IsMatch(changedLine.Line, @"\b0px"))
                {
                    messages.Add(new CodeComment
                    {
                        File = file,
                        Line = changedLine,
                        Comment = "Please update `0px` to be `0` in this SCSS rule, thanks!"
                    });

                    issueFound = true;
                }
            }

            return messages;
        }
    }
}
