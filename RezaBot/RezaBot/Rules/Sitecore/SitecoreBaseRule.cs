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
        public SitecoreBaseRule(IGitService gitService) : base(gitService)
        {
        }

        public override bool Evaluate(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines)
        {
            if (!file.FileName.Contains("GlassItems"))
            {
                return Review(prNumber, file, addedLines, removedLines, allLines);
            }

            // We didn't review the file, so no issues were found
            return false;
        }

        protected abstract bool Review(int prNumber, ChangedFile file, List<CodeLine> addedLines, List<CodeLine> removedLines, List<CodeLine> allLines);
    }
}
