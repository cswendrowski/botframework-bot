using PullRequestReviewService.Models;
using System.Collections.Generic;

namespace RezaBot.Rules
{
    public abstract class SitecoreBaseRule : Rule
    {
        public override List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();

            if (!file.FileName.Contains("GlassItems"))
            {
                messages.AddRange(Review(file, addedLines, out issueFound));

                return messages;
            }

            // We didn't review the file, so no issues were found
            issueFound = false;

            return messages;
        }

        protected abstract List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound);
    }
}
