using RezaBot.Models;
using RezaBot.Services;
using System.Collections.Generic;

namespace RezaBot.Rules.Sitecore
{
    public abstract class Rule : IRule
    {
        public IGitService GitService { get; set; }

        public abstract bool Evaluate(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines);
    }
}
