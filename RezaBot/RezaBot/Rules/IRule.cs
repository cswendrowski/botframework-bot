﻿using RezaBot.Models;
using RezaBot.Services;
using System.Collections.Generic;

namespace RezaBot.Rules
{
    public interface IRule
    {
        List<CodeComment> Evaluate(ChangedFile file, List<CodeLine> addedLines, out bool issueFound);
    }
}
