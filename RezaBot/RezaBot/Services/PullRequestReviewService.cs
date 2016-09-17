using RezaBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RezaBot.Services
{
    public class PullRequestReviewService
    {
        private GithubService _gitService;

        public PullRequestReviewService()
        {
            _gitService = new GithubService();
        }

        public void ReviewPullRequest(int prNumber)
        {
            var files = _gitService.DownloadPrFiles(prNumber);

            NitpickFiles(files, prNumber);
        }

        private void NitpickFiles(List<ChangedFile> files, int prNumber)
        {
            var issueWasFound = false;

            foreach (var file in files.Where(x => !x.FileName.Contains("GlassItems")))
            {
                if (file.ChangedLines == null || !file.ChangedLines.Any())
                {
                    Console.WriteLine("File has no changed lines");
                    continue;
                }

                var addedLines = file.ChangedLines.Where(x => x.WasAdded);
                var removedLines = file.ChangedLines.Where(x => x.WasDeleted);

                // Check for EOL newline
                if (addedLines.Any(x => x.Line.Contains("No newline at end of file")))
                {
                    _gitService.WriteComment(file, file.ChangedLines.Last(), "Please add a newline at the End of the File, thanks!", prNumber);
                    issueWasFound = true;
                }
                else if (removedLines.Any(x => x.Line.Contains("No newline at end of file")))
                {
                    _gitService.WriteComment(file, file.ChangedLines.Last(), "Thanks for adding a newline at the End of File!", prNumber);
                }

                // Check for brackets that aren't on a new line in non-excluded files
                if (file.FileType != "cshtml" && file.FileType != "js" && file.FileType != "item" && !file.FileType.Contains("proj"))
                {
                    foreach (var changedLine in addedLines)
                    {
                        if (changedLine.Line.Contains("{") || changedLine.Line.Contains("}"))
                        {
                            if (!string.IsNullOrWhiteSpace(changedLine.Line.Replace("{", "").Replace("}", "")))
                            {
                                _gitService.WriteComment(file, changedLine, "Please update Brackets (`{` and `}`) to be on a new line, thanks!", prNumber);
                                issueWasFound = true;
                            }
                        }
                    }
                }

                // Check for 0px in .scss files
                if (file.FileType == "scss")
                {
                    foreach (var changedLine in addedLines)
                    {
                        if (Regex.IsMatch(changedLine.Line, @"\b0px"))
                        {
                            _gitService.WriteComment(file, changedLine, "Please update `0px` to be `0` in this SCSS rule, thanks!", prNumber);
                            issueWasFound = true;
                        }
                    }
                }

                // Check for extra whitespaces
                foreach (var changedLine in addedLines)
                {
                    if (changedLine.Line.TrimStart().Contains("  "))
                    {
                        _gitService.WriteComment(file, changedLine, "Please remove the extra white spaces on this line, thanks!", prNumber);
                        issueWasFound = true;
                    }
                }

                // Check for extra newlines
                var lastWasNewline = false;
                foreach (var changedLine in file.ChangedLines)
                {
                    // We don't want to check for 2 newlines seperated by patches, so we reset whenever we hit a delete
                    // Patches are always in the format of DELETES -> ADDS
                    if (changedLine.WasDeleted)
                    {
                        lastWasNewline = false;
                    }

                    // Double newline
                    else if (changedLine.IsNewline && lastWasNewline)
                    {
                        _gitService.WriteComment(file, changedLine, "Please remove this extra newline, thanks!", prNumber);
                        issueWasFound = true;
                    }
                    else
                    {
                        lastWasNewline = changedLine.IsNewline;
                    }

                    // Only check newlines around open / closing tags in written files
                    if (file.FileType != "item" && changedLine.IsNewline)
                    {
                        var changedLineIndex = file.ChangedLines.IndexOf(changedLine);

                        // Newline after opening tag ( { or <html tag> )
                        if (changedLineIndex > 0)
                        {
                            var lastLine = file.ChangedLines[changedLineIndex - 1];

                            if (lastLine.WasAdded && (lastLine.Line.Contains("{") || Regex.IsMatch(lastLine.Line, @"<[A-z]+>(?!\(\))")))
                            {
                                _gitService.WriteComment(file, changedLine, "Please remove this extra newline after an opening tag, thanks!", prNumber);
                                issueWasFound = true;
                            }
                        }

                        // Newline before closing tag ( } or </html tag> )
                        if (changedLineIndex + 1 < file.ChangedLines.Count)
                        {
                            var nextLine = file.ChangedLines[changedLineIndex + 1];

                            if (nextLine.WasAdded && (nextLine.Line.Contains("}") || Regex.IsMatch(nextLine.Line, @"</[A-z]+>(?!\(\))")))
                            {
                                _gitService.WriteComment(file, changedLine, "Please remove this extra newline before a closing tag, thanks!", prNumber);
                                issueWasFound = true;
                            }
                        }
                    }
                }
            }

            if (!issueWasFound)
            {
                _gitService.AddGeneralComment("No issues found in this PR, good job!", prNumber);
            }
        }
    }
}
