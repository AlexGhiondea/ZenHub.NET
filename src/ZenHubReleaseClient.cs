using Azure;
using Azure.Core.Pipeline;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ZenHub.Models;
using ZenHub.Pipeline;

namespace ZenHub
{
    public class ZenHubReleaseClient : Client
    {
        private readonly string _releaseId;

        internal ZenHubReleaseClient(string releaseId, HttpPipeline pipeline, ZenHubClientOptions options)
            : base(pipeline, options)
        {
            _releaseId = releaseId;
        }

        /// <summary>
        /// Get the report for the release
        /// </summary>
        public async Task<Response<ReleaseReport>> GetReportAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<ReleaseReport>(
                    RequestMethod.Get,
                    $"{Options.EndPoint}/p1/reports/release/{_releaseId}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Edits the report for the release
        /// </summary>
        public async Task<Response<ReleaseReport>> EditReportAsync(string Title, string Description, DateTime StartDate, DateTime EndDate, string State, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                title = Title,
                description = Description,
                start_date = StartDate.ToUniversalTime(),
                desired_end_date = EndDate.ToUniversalTime(),
                state = State
            };

            return await MakeRequestAsync<ReleaseReport>(
                    RequestMethod.Patch,
                    $"{Options.EndPoint}/p1/reports/release/{_releaseId}",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Add repository to the release
        /// </summary>
        /// <param name="repositoryToAdd">The repository to add to the report</param>
        public async Task<Response> AddRepositoryAsync(Repository repositoryToAdd, CancellationToken cancellationToken = default)
        {
            if (repositoryToAdd == null)
            {
                throw new ArgumentNullException(nameof(repositoryToAdd));
            }

            return await MakeRequestAsync(
                    RequestMethod.Post,
                    $"{Options.EndPoint}/p1/reports/release/{_releaseId}/repository/{repositoryToAdd.Id}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove repository from the release
        /// </summary>
        /// <param name="repositoryToAdd">The repository to remove from the report</param>
        public async Task<Response> RemoveRepositoryAsync(Repository repositoryToRemove, CancellationToken cancellationToken = default)
        {
            if (repositoryToRemove == null)
            {
                throw new ArgumentNullException(nameof(repositoryToRemove));
            }

            return await MakeRequestAsync(
                    RequestMethod.Delete,
                    $"{Options.EndPoint}/p1/reports/release/{_releaseId}/repository/{repositoryToRemove.Id}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the issues in the report
        /// </summary>
        public async Task<Response<IssueDetails[]>> GetIssuesAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<IssueDetails[]>(
                    RequestMethod.Get,
                    $"{Options.EndPoint}/p1/reports/release/{_releaseId}/issues",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Add issue to the release
        /// </summary>
        /// <param name="issuesToAdd">The issue to add to the release</param>
        public async Task<Response> AddIssuesAsync(IEnumerable<Issue> issuesToAdd, CancellationToken cancellationToken = default)
        {
            return await ChangeIssuesToReleaseReportAsync(issuesToAdd, Enumerable.Empty<Issue>(), cancellationToken)
                         .ConfigureAwait(false);
        }

        /// <summary>
        /// Remove issue from the release
        /// </summary>
        /// <param name="issuesToRemove">The issue to remove from the release</param>
        public async Task<Response> RemoveIssuesAsync(IEnumerable<Issue> issuesToRemove, CancellationToken cancellationToken = default)
        {
            return await ChangeIssuesToReleaseReportAsync(Enumerable.Empty<Issue>(), issuesToRemove, cancellationToken)
                        .ConfigureAwait(false);
        }

        private async Task<Response> ChangeIssuesToReleaseReportAsync(IEnumerable<Issue> issuesToAdd, IEnumerable<Issue> issuesToRemove, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                add_issues = issuesToAdd.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray(),
                remove_issues = issuesToRemove.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray()
            };

            return await MakeRequestAsync(
                    RequestMethod.Patch,
                    $"{Options.EndPoint}/p1/reports/release/{_releaseId}/issues",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
