using PullRequestReviewService.Models;

namespace RezaBot.Rules
{
    public abstract class SitecoreBaseRule : Rule
    {
        public SitecoreBaseRule()
        {
            FileNamesToIgnore.Add("GlassItems.cs");
        }
    }
}
