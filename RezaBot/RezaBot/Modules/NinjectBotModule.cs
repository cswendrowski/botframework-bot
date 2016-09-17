using Ninject.Modules;
using RezaBot.Services;

namespace RezaBot.Modules
{
    public class NinjectBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPullRequestReviewService>().To<PullRequestReviewService>();

            Bind<IGitService>().To<GithubService>();
        }
    }
}
