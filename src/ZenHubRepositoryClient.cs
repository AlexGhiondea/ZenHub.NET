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
    public class ZenHubRepositoryClient : Client
    {
        private readonly long _repositoryId;

        internal ZenHubRepositoryClient(long repositoryId, HttpPipeline pipeline, ZenHubClientOptions options)
            : base(pipeline, options)
        {
            _repositoryId = repositoryId;
        }

        public async Task<Response<Workspace[]>> GetWorkspacesAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<Workspace[]>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p2/repositories/{_repositoryId}/workspaces",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<ZenHubBoard>> GetZenHubBoardAsync(string ZenHubWorkspaceId, CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<ZenHubBoard>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p2/workspaces/{ZenHubWorkspaceId}/repositories/{_repositoryId}/board",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<ZenHubBoard>> GetOldestZenHubBoardAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<ZenHubBoard>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/board",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<StartDate>> GetMilestoneStartAsync(Milestone milestone, CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<StartDate>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/milestones/{milestone.Number}/start_date",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<StartDate>> SetMilestoneStartAsync(Milestone milestone, DateTime startDate, CancellationToken cancellationToken = default)
        {
            var contentBody = new
            {
                start_date = startDate.ToUniversalTime()
            };

            return await MakeRequestAsync<StartDate>(
                    RequestMethod.Post,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/milestones/{milestone.Number}/start_date",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<RepoDependencies>> GetDependenciesAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<RepoDependencies>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/dependencies",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<EpicList>> GetEpicsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<EpicList>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/epics",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<ReleaseReport>> CreateReleaseReportAsync(string Title, string Description, DateTime startDate, DateTime endDate, IEnumerable<Repository> repositoriesInTheReport, CancellationToken cancellationToken = default)
        {
            long[] repos = repositoriesInTheReport.Select(x => x.Id).ToArray();

            var contentBody = new
            {
                title = Title,
                description = Description,
                start_date = startDate.ToUniversalTime(),
                desired_end_date = endDate.ToUniversalTime(),
                repositories = repos
            };

            return await MakeRequestAsync<ReleaseReport>(
                    RequestMethod.Post,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/reports/release",
                    JsonSerializer.Serialize(contentBody),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Response<ReleaseReport[]>> GetReleaseReportsAsync(CancellationToken cancellationToken = default)
        {
            return await MakeRequestAsync<ReleaseReport[]>(
                    RequestMethod.Get,
                    $"{_options.EndPoint}/p1/repositories/{_repositoryId}/reports/releases",
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }


    }
}
