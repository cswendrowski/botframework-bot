using Ninject.Extensions.Conventions;
using Ninject.Modules;
using RezaBot.Rules;
using RezaBot.Services;

namespace RezaBot.Modules
{
    public class NinjectBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPullRequestReviewService>().To<PullRequestReviewService>();

            Bind<IGitService>().To<GithubService>();

            // Binds all Rules
            Kernel.Bind(x =>
            {
                x.FromAssemblyContaining<Rule>()
                 .SelectAllClasses()
                 .InheritedFrom<IRule>()
                 .BindSingleInterface();
            });
        }
    }
}
