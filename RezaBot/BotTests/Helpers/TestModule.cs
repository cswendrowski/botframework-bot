using Ninject.Extensions.Conventions;
using Ninject.Modules;
using PullRequestReviewService.Interfaces;
using PullRequestReviewService.Models;
using RezaBot.Rules;

namespace BotTests.Helpers
{
    public class TestModule : NinjectModule
    {
        public override void Load()
        {
            // Binds all Rules
            Kernel.Bind(x =>
            {
                x.FromAssemblyContaining<SitecoreBaseRule>()
                 .SelectAllClasses()
                 .InheritedFrom<IRule>()
                 .BindSingleInterface();
            });
        }
    }
}
