using RezaBot.Models;
using RezaBot.Services;
using System.Collections.Generic;

namespace RezaBot.Rules
{
    public interface IRule
    {
        IGitService GitService { get; set; }

        bool Evaluate(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines);
    }
}
