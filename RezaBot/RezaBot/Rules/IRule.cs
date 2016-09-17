using RezaBot.Models;
using System.Collections.Generic;

namespace RezaBot.Rules
{
    public interface IRule
    {
        bool Evaluate(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines);
    }
}
