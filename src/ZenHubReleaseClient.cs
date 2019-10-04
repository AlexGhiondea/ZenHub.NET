using Azure;
using Azure.Core.Pipeline;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task<Response<ReleaseReport>> GetReportAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<ReleaseReport>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/reports/release/{_releaseId}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

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
                    $"{_options.EndPoint}/p1/reports/release/{_releaseId}",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> AddRepositoryAsync(Repository repositoryToAdd, CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync(
                    RequestMethod.Post,
                    $"{_options.EndPoint}/p1/reports/release/{_releaseId}/repository/{repositoryToAdd.Id}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> RemoveRepositoryAsync(Repository repositoryToRemove, CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync(
                    RequestMethod.Delete,
                    $"{_options.EndPoint}/p1/reports/release/{_releaseId}/repository/{repositoryToRemove.Id}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<IssueDetails[]>> GetIssuesAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<IssueDetails[]>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/reports/release/{_releaseId}/issues",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> AddIssuesAsync(IEnumerable<Issue> issuesToAdd, CancellationToken cancellationToken = default)
        {
            return await ChangeIssuesToReleaseReportAsync(issuesToAdd, Enumerable.Empty<Issue>(), cancellationToken);
        }

        public async Task<Response> RemoveIssuesAsync(IEnumerable<Issue> issuesToRemove, CancellationToken cancellationToken = default)
        {
            return await ChangeIssuesToReleaseReportAsync(Enumerable.Empty<Issue>(), issuesToRemove, cancellationToken);
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
                    $"{_options.EndPoint}/p1/reports/release/{_releaseId}/issues",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
