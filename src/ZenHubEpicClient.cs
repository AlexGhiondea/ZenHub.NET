using Azure;
using Azure.Core.Pipeline;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ZenHub.Models;
using ZenHub.Pipeline;

namespace ZenHub
{
    public class ZenHubEpicClient : Client
    {
        private readonly long _repositoryId;
        private readonly int _epicNumber;

        internal ZenHubEpicClient(long repoId, int epicNumber, HttpPipeline pipeline, ZenHubClientOptions options)
            : base(pipeline, options)
        {
            _repositoryId = repoId;
            _epicNumber = epicNumber;
        }

        /// <summary>
        /// Get details about the epic
        /// </summary>
        public async Task<Response<EpicDetails>> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<EpicDetails>(
                    RequestMethod.Get,
                    $"{Options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Add issues to the epic
        /// </summary>
        /// <param name="issuesToAdd">A collection of issues to add</param>
        public async Task<Response> AddIssuesAsync(IEnumerable<Issue> issuesToAdd, CancellationToken cancellationToken = default)
        {
            if (issuesToAdd == null)
            {
                issuesToAdd = Enumerable.Empty<Issue>();
            }

            // convert the list of issues to a list of tuples with just the information necessary
            return await AddOrRemoveIssueToEpicAsync(issuesToAdd.Select(x => (x.Repository.Id, x.Number)), Enumerable.Empty<(long, int)>(), cancellationToken)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Add issues to the epic
        /// </summary>
        /// <param name="issuesToAdd">An collection of tuple (repoId, issueNumber)</param>
        public async Task<Response> AddIssuesAsync(IEnumerable<(long repoId, int issueNumber)> issuesToAdd = null, CancellationToken cancellationToken = default)
        {
            if (issuesToAdd == null)
            {
                issuesToAdd = Enumerable.Empty<(long, int)>();
            }

            return await AddOrRemoveIssueToEpicAsync(issuesToAdd, Enumerable.Empty<(long, int)>(), cancellationToken)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove issues from the epic
        /// </summary>
        /// <param name="issuesToAdd">A collection of issues to remove</param>
        public async Task<Response> RemoveIssuesAsync(IEnumerable<Issue> issuesToRemove, CancellationToken cancellationToken = default)
        {
            if (issuesToRemove == null)
            {
                issuesToRemove = Enumerable.Empty<Issue>();
            }

            // convert the list of issues to a list of tuples with just the information necessary
            return await AddOrRemoveIssueToEpicAsync(Enumerable.Empty<(long, int)>(), issuesToRemove.Select(x => (x.Repository.Id, x.Number)), cancellationToken)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove issues from the epic
        /// </summary>
        /// <param name="issuesToAdd">A collection of of tuple (repoId, issueNumber)</param>
        public async Task<Response> RemoveIssuesAsync(IEnumerable<(long repoId, int issueNumber)> issuesToRemove = null, CancellationToken cancellationToken = default)
        {
            if (issuesToRemove == null)
            {
                issuesToRemove = Enumerable.Empty<(long, int)>();
            }

            return await AddOrRemoveIssueToEpicAsync(Enumerable.Empty<(long, int)>(), issuesToRemove, cancellationToken)
                        .ConfigureAwait(false);
        }

        /// <summary>
        /// Converts the current epic to an issue
        /// </summary>
        public async Task<Response> ConvertToIssueAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync(
                    RequestMethod.Post,
                    $"{Options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}/convert_to_issue",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<Response> AddOrRemoveIssueToEpicAsync(IEnumerable<(long repoId, int issueNumber)> issuesToAdd, IEnumerable<(long repoId, int issueNumber)> issuesToRemove, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                add_issues = issuesToAdd.Select(x => new { repo_id = x.repoId, issue_number = x.issueNumber }).ToArray(),
                remove_issues = issuesToRemove.Select(x => new { repo_id = x.repoId, issue_number = x.issueNumber }).ToArray()
            };

            return await MakeRequestAsync(
                    RequestMethod.Post,
                    $"{Options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}/update_issues",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
