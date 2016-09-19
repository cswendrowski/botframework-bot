using RezaBot.Models;
using RezaBot.Services;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    public abstract class Rule : IRule
    {
        public abstract List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, out bool issueFound);
    }
}
