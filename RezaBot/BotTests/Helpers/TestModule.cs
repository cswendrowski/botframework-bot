using BotTests.Fakes;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using RezaBot.Rules;
using RezaBot.Services;

namespace BotTests.Helpers
{
    public class TestModule : NinjectModule
    {
        public override void Load()
        {
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
