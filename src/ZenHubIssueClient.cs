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
    public class ZenHubIssueClient : Client
    {
        private readonly long _repositoryId;
        private readonly int _issueNumber;

        internal ZenHubIssueClient(long repoId, int issueNumber, HttpPipeline pipeline, ZenHubClientOptions options)
            : base(pipeline, options)
        {
            _repositoryId = repoId;
            _issueNumber = issueNumber;
        }

        public async Task<Response<IssueDetails>> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<IssueDetails>(
                    RequestMethod.Get, 
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/issues/{_issueNumber}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<Models.IssueEvent[]>> GetEventsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<Models.IssueEvent[]>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/issues/{_issueNumber}/events",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> SetEstimateAsync(int estimate, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                estimate = estimate
            };

            return await MakeRequestAsync(
                    RequestMethod.Put,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/issues/{_issueNumber}/estimate", 
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> MoveToPipelineAsync(string ZenHubWorkspaceId, string PipelineId, int position, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                pipeline_id = PipelineId,
                position = position
            };

            return await MakeRequestAsync(
                    RequestMethod.Post, 
                    $"{_options.EndPoint}/p2/workspaces/{ZenHubWorkspaceId}/repositories/{_repositoryId}/issues/{_issueNumber}/moves", 
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> MoveToPipelineInOldestWorkspaceAsync(string PipelineId, int position, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                pipeline_id = PipelineId,
                position = position
            };

            return await MakeRequestAsync(
                    RequestMethod.Post, 
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/issues/{_issueNumber}/moves",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> ConvertToEpicAsync(IEnumerable<Issue> issuesToAddToEpic, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                issues = issuesToAddToEpic.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray(),
            };

            return await MakeRequestAsync(
                    RequestMethod.Post, 
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/epics/{_issueNumber}/convert_to_epic",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<IssueDependency>> AddBlockedByAsync(Issue blockingIssue,CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                blocking = new
                {
                    repo_id = blockingIssue.Repository.Id,
                    issue_number = blockingIssue.Number
                },
                blocked = new
                {
                    repo_id = _repositoryId,
                    issue_number = _issueNumber
                }
            };

            return await MakeRequestAsync<IssueDependency>(
                    RequestMethod.Post,
                    $"{_options.EndPoint}/p1/dependencies",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response> RemoveBlockedByAsync(Issue blockingIssue, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                blocking = new
                {
                    repo_id = blockingIssue.Repository.Id,
                    issue_number = blockingIssue.Number
                },
                blocked = new
                {
                    repo_id = _repositoryId,
                    issue_number = _issueNumber
                }
            };

            return await MakeRequestAsync(
                    RequestMethod.Delete,
                    $"{_options.EndPoint}/p1/dependencies",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
