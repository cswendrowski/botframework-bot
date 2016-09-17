using RezaBot.Models;
using System.Collections.Generic;

namespace RezaBot.Services
{
    public interface IGitService
    {
        void AddGeneralComment(string message, int prNumber);

        List<ChangedFile> DownloadPrFiles(int prNumber);

        string GetLastestCommitInPr(int prNumber);

        string GetPrAuthor(int prNumber);

        void WriteComment(ChangedFile file, CodeLine line, string message, int prNumber);
    }
}
