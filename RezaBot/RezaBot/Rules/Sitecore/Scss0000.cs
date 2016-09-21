using PullRequestReviewService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RezaBot.Rules.Sitecore
{
    public class Scss0000 : SitecoreBaseRule
    {
        protected override List<CodeComment> Review(ChangedFile file, List<CodeLine> addedLines, out bool issueFound)
        {
            var messages = new List<CodeComment>();
            issueFound = false;

            if (file.FileType == "scss")
            {
                foreach (var changedLine in addedLines)
                {
                    if (changedLine.Line.Contains("0 0 0 0"))
                    {
                        messages.Add(new CodeComment
                        {
                            File = file,
                            Line = changedLine,
                            Comment = "Please simplify this rule from `0 0 0 0` down to `0`, thanks!"
                        });

                        issueFound = true;
                    }
                    else if (changedLine.Line.Contains("0 0 0"))
                    {
                        messages.Add(new CodeComment
                        {
                            File = file,
                            Line = changedLine,
                            Comment = "Please simplify this rule from `0 0 0` down to `0`, thanks!"
                        });

                        issueFound = true;
                    }
                    else if (changedLine.Line.Contains("0 0"))
                    {
                        messages.Add(new CodeComment
                        {
                            File = file,
                            Line = changedLine,
                            Comment = "Please simplify this rule from `0 0` down to `0`, thanks!"
                        });

                        issueFound = true;
                    }
                }
            }

            return messages;
        }
    }
}
