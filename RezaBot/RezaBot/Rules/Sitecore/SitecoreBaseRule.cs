using RezaBot.Models;
using RezaBot.Rules.Sitecore;
using RezaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Rules
{
    public abstract class SitecoreBaseRule : Rule
    {
        public override List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();

            if (!file.FileName.Contains("GlassItems"))
            {
                messages.AddRange(Review(file, addedLines, removedLines, out issueFound));

                return messages;
            }

            // We didn't review the file, so no issues were found
            issueFound = false;

            return messages;
        }

        protected abstract List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, out bool issueFound);
    }
}
