using Azure.Core.Pipeline;
using Octokit;
using System;
using ZenHub.Pipeline;

namespace ZenHub
{
    /// <summary>
    /// This represents the main object to be used by clients communicating with the service.
    /// </summary>
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

        /// <summary>
        /// Get the repository client for a given repository
        /// </summary>
        /// <param name="repository">The repository to create the client for</param>
        /// <returns>A client that provides operations on a repository</returns>
        public ZenHubRepositoryClient GetRepositoryClient(Repository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            return new ZenHubRepositoryClient(repository.Id, Pipeline, Options);
        }

        /// <summary>
        /// Get the repository client for a given repository
        /// </summary>
        /// <param name="repository">The repository id to create the client for</param>
        /// <returns>A client that provides operations on a repository</returns>
        public ZenHubRepositoryClient GetRepositoryClient(long repositoryId)
        {
            return new ZenHubRepositoryClient(repositoryId, Pipeline, Options);
        }

        /// <summary>
        /// Get the Issue client for a given issue
        /// </summary>
        /// <param name="issue">The issue to create the client for</param>
        /// <returns>A client that provides operations on an Issue</returns>
        public ZenHubIssueClient GetIssueClient(Issue issue)
        {
            if (issue == null)
            {
                throw new ArgumentNullException(nameof(issue));
            }
            
            return GetIssueClient(issue.Repository.Id, issue.Number);
        }

        /// <summary>
        /// Get the Issue client for a given issue
        /// </summary>
        /// <param name="issueNumber">The number of the issue</param>
        /// <param name="repoId">The repository id of the issue</param>
        /// <returns>A client that provides operations on an Issue</returns>
        public ZenHubIssueClient GetIssueClient(long repoId, int issueNumber)
        {
            return new ZenHubIssueClient(repoId, issueNumber, Pipeline, Options);
        }

        /// <summary>
        /// Get the Essue client for a given epic
        /// </summary>
        /// <param name="issue">The epic to create the client for</param>
        /// <returns>A client that provides operations on an Epic</returns>
        public ZenHubEpicClient GetEpicClient(Issue epic)
        {
            if (epic == null)
            {
                throw new ArgumentNullException(nameof(epic));
            }

            return GetEpicClient(epic.Repository.Id, epic.Number);
        }

        /// <summary>
        /// Get the Epic client for a given issue
        /// </summary>
        /// <param name="issueNumber">The number of the epic</param>
        /// <param name="repoId">The repository id of the epic</param>
        /// <returns>A client that provides operations on an Epic</returns>
        public ZenHubEpicClient GetEpicClient(long repoId, int epicNumber)
        {
            return new ZenHubEpicClient(repoId, epicNumber, Pipeline, Options);
        }

        /// <summary>
        /// Get the release client for a given issue
        /// </summary>
        /// <param name="releaseId">The releaseId for which to create the client</param>
        /// <returns>A client that provides operations on an Release</returns>
        public ZenHubReleaseClient GetReleaseClient(string releaseId)
        {
            if (string.IsNullOrEmpty(releaseId))
            {
                throw new ArgumentNullException(nameof(releaseId));
            }

            return new ZenHubReleaseClient(releaseId, Pipeline, Options);
        }
    }
}

