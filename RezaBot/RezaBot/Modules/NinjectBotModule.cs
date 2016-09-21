using Ninject.Extensions.Conventions;
using Ninject.Modules;
using PullRequestReviewService.Interfaces;
using PullRequestReviewService.Models;
using PullRequestReviewService.Services;
using RezaBot.Services;

namespace RezaBot.Modules
{
    public class NinjectBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPullRequestReviewService>().To<PrReviewService>();

            Bind<IGitService>().To<GithubService>();

            // Binds all Rules
            Kernel.Bind(x =>
            {
                x.FromThisAssembly()
                 .SelectAllClasses()
                 .InheritedFrom<IRule>()
                 .BindSingleInterface();
            });
        }
    }
}
