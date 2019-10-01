using Azure.Core.Pipeline;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZenHub.Pipeline;

namespace ZenHub
{
    public class ZenHubClient
    {
        HttpPipeline _pipeline;

        private readonly string EndPoint = "https://api.zenhub.io";

        public ZenHubClient(string authToken) : this(authToken, null)
        {
        }

        public ZenHubClient(string authToken, ZenHubClientOptions clientOptions)
        {
            if (clientOptions != null)
            {
                EndPoint = clientOptions.EndPoint;
            }
            else
            {
                clientOptions ??= new ZenHubClientOptions();
            }

            _pipeline = HttpPipelineBuilder.Build(clientOptions, new ZenHubAuthenticationPolicy(authToken), new ThrowOnErrorStatusPolicy());
        }

        public async Task<dynamic> GetIssueDataAsync(long repoId, int issueNumber)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repoId}/issues/{issueNumber}");
        }

        public async Task<dynamic> GetIssueDataAsync(Issue issue)
        {
            return await GetIssueDataAsync(issue.Repository.Id, issue.Number);
        }

        public async Task<dynamic> GetIssueEventsAsync(long repoId, int issueNumber)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repoId}/issues/{issueNumber}/events");
        }

        public async Task<dynamic> GetIssueEventsAsync(Issue issue)
        {
            return await GetIssueEventsAsync(issue.Repository.Id, issue.Number);
        }

        public async Task<dynamic> GetEpicsAsync(long repoId)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repoId}/epics");
        }

        public async Task<dynamic> GetEpicsAsync(Repository repository)
        {
            return await GetEpicsAsync(repository.Id);
        }

        public async Task<dynamic> GetEpicDataAsync(long repoId, int epicId)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repoId}/epics/{epicId}");
        }

        public async Task<dynamic> GetEpicDataAsync(Issue epic)
        {
            return await GetEpicDataAsync(epic.Repository.Id, epic.Number);
        }

        public async Task<dynamic> SetEstimateAsync(long repoId, int issueNumber, int estimate)
        {
            var contentBody = new
            {
                estimate = estimate
            };

            return await MakeRequestAsync(RequestMethod.Put, $"{EndPoint}/p1/repositories/{repoId}/issues/{issueNumber}/estimate", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> SetEstimateAsync(Issue issue, int estimate)
        {
            var contentBody = new
            {
                estimate = estimate
            };

            return await MakeRequestAsync(RequestMethod.Put, $"{EndPoint}/p1/repositories/{issue.Repository.Id}/issues/{issue.Number}/estimate", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> AddIssueToEpic(Issue issue, Issue epic)
        {
            return await AddOrRemoveIssueToEpic(epic, new List<Issue>() { issue }, Enumerable.Empty<Issue>());
        }

        public async Task<dynamic> AddOrRemoveIssueToEpic(Issue epic, IEnumerable<Issue> issuesToAdd, IEnumerable<Issue> issuesToRemove)
        {
            var contentBody = new
            {
                add_issues = issuesToAdd.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray(),
                remove_issues = issuesToRemove.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray()
            };

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/repositories/{epic.Repository.Id}/epics/{epic.Number}/update_issues", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> MoveIssueToPipeline(Issue issue, string ZenHubWorkspaceId, string PipelineId, int position)
        {
            var contentBody = new
            {
                pipeline_id = PipelineId,
                position = position
            };

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p2/workspaces/{ZenHubWorkspaceId}/repositories/{issue.Repository.Id}/issues/{issue.Number}/moves", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> MoveIssueToPipelineOldestWorkspace(Issue issue, string PipelineId, int position)
        {
            var contentBody = new
            {
                pipeline_id = PipelineId,
                position = position
            };

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/repositories/{issue.Repository.Id}/issues/{issue.Number}/moves", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> ConvertEpicToIssue(Issue epic)
        {
            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/repositories/{epic.Repository.Id}/epics/{epic.Number}/convert_to_issue");
        }

        public async Task<dynamic> ConvertIssueToEpic(Issue issue, params Issue[] issuesToAddToEpic)
        {
            var contentBody = new
            {
                issues = issuesToAddToEpic.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray(),
            };

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/repositories/{issue.Repository.Id}/epics/{issue.Number}/convert_to_epic", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> GetWorkspaces(Repository repository)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p2/repositories/{repository.Id}/workspaces");
        }

        public async Task<dynamic> GetZenHubBoard(Repository repository, string ZenHubWorkspaceId)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p2/workspaces/{ZenHubWorkspaceId}/repositories/{repository.Id}/board");
        }

        public async Task<dynamic> GetOldestZenHubBoard(Repository repository)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repository.Id}/board");
        }

        public async Task<dynamic> GetMilestoneStart(Repository repository, Milestone milestone)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repository.Id}/milestones/{milestone.Number}/start_date");
        }

        public async Task<dynamic> SetMilestoneStart(Repository repository, Milestone milestone, DateTime startDate)
        {
            var contentBody = new
            {
                start_date = startDate.ToUniversalTime()
            };

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/repositories/{repository.Id}/milestones/{milestone.Number}/start_date", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> GetDependencies(Repository repository)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repository.Id}/dependencies");
        }

        public async Task<dynamic> CreateDependency(Issue blockingIssue, Issue blockedIssue)
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
                    repo_id = blockedIssue.Repository.Id,
                    issue_number = blockedIssue.Number
                }
            };

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/dependencies", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> DeleteDependency(Issue blockingIssue, Issue blockedIssue)
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
                    repo_id = blockedIssue.Repository.Id,
                    issue_number = blockedIssue.Number
                }
            };

            return await MakeRequestAsync(RequestMethod.Delete, $"{EndPoint}/p1/dependencies", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> CreateReleaseReport(Repository repository, string Title, string Description, DateTime startDate, DateTime endDate, IEnumerable<Repository> repositoriesInTheReport)
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

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/repositories/{repository.Id}/reports/release", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> GetReleaseReport(string zenHubReleaseId)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/reports/release/{zenHubReleaseId}");
        }

        public async Task<dynamic> GetReleaseReports(Repository repository)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repository.Id}/reports/releases");
        }

        public async Task<dynamic> EditReleaseReport(string ZenHubReleaseId, string Title, string Description, DateTime StartDate, DateTime EndDate, string State)
        {
            var contentBody = new
            {
                title = Title,
                description = Description,
                start_date = StartDate.ToUniversalTime(),
                desired_end_date = EndDate.ToUniversalTime(),
                state = State
            };

            return await MakeRequestAsync(RequestMethod.Patch, $"{EndPoint}/p1/reports/release/{ZenHubReleaseId}", JsonConvert.SerializeObject(contentBody));
        }

        public async Task<dynamic> AddRepositoryToReleaseReport(string ZenHubReleaseId, Repository repositoryToAdd)
        {
            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/reports/release/{ZenHubReleaseId}/repository/{repositoryToAdd.Id}");
        }

        public async Task<dynamic> RemoveRepositoryFromReleaseReport(string ZenHubReleaseId, Repository repositoryToRemove)
        {
            return await MakeRequestAsync(RequestMethod.Delete, $"{EndPoint}/p1/reports/release/{ZenHubReleaseId}/repository/{repositoryToRemove.Id}");
        }

        public async Task<dynamic> GetAllIssuesInReleaseReport(string ZenHubReleaseId)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/reports/release/{ZenHubReleaseId}/issues");
        }

        public async Task<dynamic> ChangeIssuesToReleaseReport(string ZenHubReleaseId, IEnumerable<Issue> issuesToAdd, IEnumerable<Issue> issuesToRemove)
        {
            var contentBody = new
            {
                add_issues = issuesToAdd.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray(),
                remove_issues = issuesToRemove.Select(x => new { repo_id = x.Repository.Id, issue_number = x.Number }).ToArray()
            };

            return await MakeRequestAsync(RequestMethod.Patch, $"{EndPoint}/p1/reports/release/{ZenHubReleaseId}/issues", JsonConvert.SerializeObject(contentBody));
        }

        private async Task<dynamic> MakeRequestAsync(RequestMethod method, string endpoint, string jsonBody = "")
        {
            var request = _pipeline.CreateRequest();
            request.Method = method;
            request.UriBuilder.Uri = new Uri(endpoint);

            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.Content = HttpPipelineRequestContent.Create(Encoding.UTF8.GetBytes(jsonBody));
                request.Headers.Add("Content-Type", "application/json");
            }

            var response = await _pipeline.SendRequestAsync(request, CancellationToken.None).ConfigureAwait(false);

            // read the content from the stream
            // The policy will throw if we received an invalid response
            using (StreamReader sr = new StreamReader(response.ContentStream))
            {
                return JsonConvert.DeserializeObject<dynamic>(await sr.ReadToEndAsync());
            }
        }
    }
}

