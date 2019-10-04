using Azure.Core.Pipeline;
using Octokit;
using ZenHub.Pipeline;

namespace ZenHub
{
    public class ZenHubClient : Client
    {
        public ZenHubClient(string authToken)
            : this(authToken, new ZenHubClientOptions() { EndPoint = "https://api.zenhub.io" })
        {
        }

        public ZenHubClient(string authToken, ZenHubClientOptions clientOptions)
            : base(HttpPipelineBuilder.Build(clientOptions, new ZenHubAuthenticationPolicy(authToken), new ThrowOnErrorStatusPolicy()), clientOptions)
        {
        }

        public ZenHubRepositoryClient GetRepositoryClient(Repository repository)
        {
            return new ZenHubRepositoryClient(repository.Id, _pipeline, _options);
        }

        public ZenHubRepositoryClient GetRepositoryClient(long repositoryId)
        {
            return new ZenHubRepositoryClient(repositoryId, _pipeline, _options);
        }

        public ZenHubIssueClient GetIssueClient(Issue issue)
        {
            return GetIssueClient(issue.Repository.Id, issue.Number);
        }

        public ZenHubIssueClient GetIssueClient(long repoId, int issueNumber)
        {
            return new ZenHubIssueClient(repoId, issueNumber, _pipeline, _options);
        }

        public ZenHubEpicClient GetEpicClient(Issue epic)
        {
            return GetEpicClient(epic.Repository.Id, epic.Number);
        }

        public ZenHubEpicClient GetEpicClient(long repoId, int epicNumber)
        {
            return new ZenHubEpicClient(repoId, epicNumber, _pipeline, _options);
        }

        public ZenHubReleaseClient GetReleaseClient(string releaseId)
        {
            return new ZenHubReleaseClient(releaseId, _pipeline, _options);
        }
    }
}

