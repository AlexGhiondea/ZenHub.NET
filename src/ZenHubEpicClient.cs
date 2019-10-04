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

        public async Task<Response<EpicDetails>> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<EpicDetails>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> AddIssuesAsync(IEnumerable<Issue> issuesToAdd, CancellationToken cancellationToken = default)
        {
            return await AddOrRemoveIssueToEpicAsync(issuesToAdd, Enumerable.Empty<Issue>(), cancellationToken);
        }

        public async Task<Response> RemoveIssuesAsync(IEnumerable<Issue> issuesToRemove, CancellationToken cancellationToken = default)
        {
            return await AddOrRemoveIssueToEpicAsync(Enumerable.Empty<Issue>(), issuesToRemove, cancellationToken);
        }

        public async Task<Response> ConvertToIssueAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync(
                    RequestMethod.Post,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}/convert_to_issue",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<Response> AddOrRemoveIssueToEpicAsync(IEnumerable<Issue> issuesToAdd, IEnumerable<Issue> issuesToRemove, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                add_issues = issuesToAdd.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray(),
                remove_issues = issuesToRemove.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray()
            };

            return await MakeRequestAsync(
                    RequestMethod.Post,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}/update_issues",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
