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

            // Rules
            Bind<IRule>().To<Brackets>();
            Bind<IRule>().To<EndOfLine>();
            Bind<IRule>().To<ExtraNewLine>();
            Bind<IRule>().To<Scss0px>();
            Bind<IRule>().To<Whitespaces>();
        }
    }
}
