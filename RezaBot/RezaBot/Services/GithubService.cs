using Octokit;
using PullRequestReviewService.Interfaces;
using PullRequestReviewService.Models;
using RestSharp;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RezaBot.Services
{
    public class GithubService : IGitService
    {
        protected string Token = ConfigurationManager.AppSettings["Github-Token"];
        protected string RepositoryName = ConfigurationManager.AppSettings["Repository-Name"];
        protected string RepositoryOwner = ConfigurationManager.AppSettings["Repository-Owner"];

        private GitHubClient GetClient()
        {
            var client = new GitHubClient(new Octokit.ProductHeaderValue("sherpa-bot"));
            client.Credentials = new Credentials(Token);
            return client;
        }

        private RestClient GetGithubRestClient()
        {
            return new RestClient("https://api.github.com");
        }

        public async void WriteComment(ChangedFile file, CodeLine line, string message, int prNumber)
        {
            message = "@" + GetPrAuthor(prNumber) + " " + message;

            var github = GetClient();
            var client = github.PullRequest.Comment;

            var comment = new PullRequestReviewCommentCreate(message, GetLastestCommitInPr(prNumber), file.FileName, line.LineNumber);

            await client.Create(RepositoryOwner, RepositoryName, prNumber, comment);
        }

        public async void AddGeneralComment(string message, int prNumber)
        {
            message = "@" + GetPrAuthor(prNumber) + " " + message;

            var github = GetClient();
            var client = github.Issue.Comment;

            await client.Create(RepositoryOwner, RepositoryName, prNumber, message);
        }

        public string GetPrAuthor(int prNumber)
        {
            var github = GetClient();
            var request = github.PullRequest.Get(RepositoryOwner, RepositoryName, prNumber).Result;

            return request.User.Login;
        }

        public string GetLastestCommitInPr(int prNumber)
        {
            var client = GetGithubRestClient();

            var request = new RestRequest("/repos/{owner_name}/{repo_name}/pulls/{pr_number}/commits", Method.GET);

            request.AddParameter("access_token", ConfigurationManager.AppSettings["Github-Token"]);
            request.AddUrlSegment("owner_name", RepositoryOwner);
            request.AddUrlSegment("repo_name", RepositoryName);
            request.AddUrlSegment("pr_number", prNumber.ToString());

            var response = client.Execute<List<PullRequestReviewService.Models.Commit>>(request);

            return response.Data.Last().SHA;
        }

        public List<ChangedFile> DownloadPrFiles(int prNumber)
        {
            var client = GetGithubRestClient();

            var request = new RestRequest("/repos/{owner_name}/{repo_name}/pulls/{pr_number}/files", Method.GET);

            request.AddParameter("access_token", Token);

            request.AddUrlSegment("owner_name", RepositoryOwner);
            request.AddUrlSegment("repo_name", RepositoryName);
            request.AddUrlSegment("pr_number", prNumber.ToString());

            var response = client.Execute<List<ChangedFile>>(request);

            var files = response.Data;

            foreach (var file in files)
            {
                file.ConvertPatchToChangedLines();
            }

            return files;
        }
    }
}
