using RezaBot.Models;
using RezaBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Rules.Sitecore
{
    public abstract class Rule : IRule
    {
        protected IGitService _gitService;

        public Rule(IGitService gitService)
        {
            _gitService = gitService;
        }

        public abstract bool Evaluate(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines);
    }
}
