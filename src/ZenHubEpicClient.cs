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

        public async Task<Response<EpicData>> GetEpicDataAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<EpicData>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> AddOrRemoveIssueToEpic(IEnumerable<Issue> issuesToAdd, IEnumerable<Issue> issuesToRemove, CancellationToken cancellationToken = default)
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

        public async Task<Response> ConvertEpicToIssue(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync(
                    RequestMethod.Post,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_epicNumber}/convert_to_issue",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
