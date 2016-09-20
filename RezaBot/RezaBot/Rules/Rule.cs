using RezaBot.Models;
using RezaBot.Services;
using System.Collections.Generic;

namespace RezaBot.Rules
{
    public abstract class Rule : IRule
    {
        public abstract List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound);
    }
}
