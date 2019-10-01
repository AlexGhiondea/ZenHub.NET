using Octokit;

namespace ZenHub.Tests.Helpers
{
    public static class ObjectCreator
    {
        public static Issue CreateIssue(int issueNumber, long RepoId)
        {
            Issue issue = new Issue();

            // use reflection to set the 2 values
            issue.GetType().GetProperty("Number").SetValue(issue, issueNumber);
            issue.GetType().GetProperty("Repository").SetValue(issue, new Repository(RepoId));

            return issue;
        }
    }
}
