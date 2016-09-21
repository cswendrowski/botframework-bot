using PullRequestReviewService.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace PullRequestReviewService.Models
{
    public abstract class Rule : IRule
    {
        protected List<string> FileNamesToIgnore = new List<string>();

        protected List<string> FileTypesToCheck = new List<string>();

        public List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
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
