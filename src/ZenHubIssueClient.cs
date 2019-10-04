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

        /// <summary>
        /// Get details about the issue
        /// </summary>
        public async Task<Response<IssueDetails>> GetDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<IssueDetails>(
                    RequestMethod.Get, 
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/issues/{_issueNumber}",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get the events for the isue
        /// </summary>
        public async Task<Response<Models.IssueEvent[]>> GetEventsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<Models.IssueEvent[]>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/issues/{_issueNumber}/events",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Set the estimate on the issue
        /// </summary>
        /// <param name="estimate">The value of the estimate</param>
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

        /// <summary>
        /// Move the issue to a pipeline
        /// </summary>
        /// <param name="ZenHubWorkspaceId">The ZenHub id of the workspace where the pipeline is</param>
        /// <param name="PipelineId">The pipeline id</param>
        /// <param name="position">The position in the pipeline to add the issue to</param>
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
        /// <summary>
        /// Move the issue to a pipeline
        /// </summary>
        /// <param name="PipelineId">The pipeline id of the oldest board</param>
        /// <param name="position">The position in the pipeline to add the issue to</param>
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

        /// <summary>
        /// Converts the issue to an epic
        /// </summary>
        /// <param name="issuesToAddToEpic">The list of issues to add to the epic during the conversion</param>
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

        /// <summary>
        /// Add a blocked dependency to the issue
        /// </summary>
        /// <param name="blockingIssue">The issue that is blocking</param>
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

        /// <summary>
        /// Remove a blocked dependency to the issue
        /// </summary>
        /// <param name="blockingIssue">The issue that is blocking</param>
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
