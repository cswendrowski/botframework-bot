using Ninject.Modules;
using RezaBot.Rules;
using RezaBot.Rules.Sitecore;
using RezaBot.Services;

namespace RezaBot.Modules
{
    public class NinjectBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPullRequestReviewService>().To<PullRequestReviewService>();

            Bind<IGitService>().To<GithubService>();

            Bind<IRule>().To<EndOfLine>();
        }
    }
}
