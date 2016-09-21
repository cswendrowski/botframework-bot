using PullRequestReviewService.Models;
using System.Collections.Generic;
using System.Linq;

namespace RezaBot.Rules
{
    public abstract class SitecoreBaseRule : Rule
    {
        public SitecoreBaseRule()
        {
            FileNamesToIgnore.Add("GlassItems.cs");
        }

        public override List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();

            var fileTypes = FileTypesToCheck.Select(x => x.ToLower());

            if (!FileNamesToIgnore.Contains(file.FileName) &&
                (fileTypes.Contains("any") || fileTypes.Contains(file.FileType)))
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
