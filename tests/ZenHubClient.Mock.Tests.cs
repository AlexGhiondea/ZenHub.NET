using Azure;
using NUnit.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZenHub.Pipeline;
using ZenHub.Tests.Helpers;

namespace ZenHub.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class Tests
    {
        private static MockServer s_mockServer;

        private static ZenHubClient _zenhubClient;

        [OneTimeSetUp]
        public void Initialize()
        {
            s_mockServer = new MockServer();
            s_mockServer.RegisterServer();

            ZenHubClientOptions options = new ZenHubClientOptions() { EndPoint = s_mockServer.EndPoint };
            _zenhubClient = new ZenHubClient("dummyToken", options);
        }

        [OneTimeTearDown]
        public void TeadDown()
        {
            s_mockServer.Stop();
        }

        [Test]
        public void GetIssueDataAsyncTest1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var result = _zenhubClient.GetIssueClient(repoId, issueNumber).GetDetailsAsync().GetAwaiter().GetResult();

            Assert.AreEqual(8, result.Value.Estimate.Value);
            Assert.AreEqual(true, result.Value.IsEpic);
            Assert.AreEqual("5d0a7cea41fd098f6b7f58b7", result.Value.Pipelines[1].PipelineId);
        }

        [Test]
        public void GetIssueEvents1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var result = _zenhubClient.GetIssueClient(repoId, issueNumber).GetEventsAsync().GetAwaiter().GetResult();

            Assert.AreEqual(16717, result.Value[0].UserId);
            Assert.AreEqual("transferIssue", result.Value[3].EventType);
        }

        [Test]
        public void GetEpicsForRepo1()
        {
            long repoId = MockServer.repositoryId;

            var result = _zenhubClient.GetRepositoryClient(new Repository(repoId)).GetEpicsAsync().GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3953, result.Value.Epics[0].IssueNumber);
        }

        [Test]
        public void GetEpicData1()
        {
            long repoId = MockServer.repositoryId;
            int epicId = MockServer.issueNumber;

            var result = _zenhubClient.GetEpicClient(repoId, epicId).GetDetailsAsync().GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3161, result.Value.Issues[0].IssueNumber);
        }

        [Test]
        public void GetEpicData2()
        {
            long repoId = MockServer.repositoryId;
            int epicId = MockServer.issueNumber;

            var result = _zenhubClient.GetEpicClient(ObjectCreator.CreateIssue(epicId, repoId)).GetDetailsAsync().GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3161, result.Value.Issues[0].IssueNumber);
        }

        [Test]
        public void GetWorkspaces1()
        {
            long repoId = MockServer.repositoryId;

            var result = _zenhubClient.GetRepositoryClient(new Octokit.Repository(repoId)).GetWorkspacesAsync().GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("Design and UX", result.Value[0].Name);
            Assert.AreEqual(12345678, (int)result.Value[1].Repositories[0]);
        }

        [Test]
        public void GetZenHubBoard1()
        {
            var repository = new Octokit.Repository(MockServer.repositoryId);
            string ZenHubWorkspaceId = MockServer.ZenHubWorkspaceId;

            var result = _zenhubClient.GetRepositoryClient(repository).GetZenHubBoardAsync(ZenHubWorkspaceId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("595d430add03f01d32460080", result.Value.Pipelines[0].Id);
        }

        [Test]
        public void GetZenHubBoard2()
        {
            var repository = new Octokit.Repository(MockServer.repositoryId);

            var result = _zenhubClient.GetRepositoryClient(repository).GetOldestZenHubBoardAsync().GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("595d430add03f01d32460080", result.Value.Pipelines[0].Id);
        }

        [Test]
        public void GetDependencyForRepo1()
        {
            var repository = new Octokit.Repository(MockServer.repositoryId);

            var result = _zenhubClient.GetRepositoryClient(repository).GetDependenciesAsync().GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3953, result.Value.Dependencies[0].Blocking.IssueNumber);
        }

        [Test]
        public void GetReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.GetReleaseClient(zenHubReleaseId).GetReportAsync().GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59d3cd520a430a6344fd3bdb", result.Value.ReleaseId);
        }

        [Test]
        public void GetReleaseReportForRepo1()
        {
            var repository = new Octokit.Repository(MockServer.repositoryId);

            var result = _zenhubClient.GetRepositoryClient(repository).GetReleaseReportsAsync().GetAwaiter().GetResult();

            Assert.AreEqual("59cbf2fde010f7a5207406e8", result.Value[0].ReleaseId);
        }


        [Test]
        public void EditReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.GetReleaseClient(zenHubReleaseId).EditReportAsync("Amazing title", "Amazing description", DateTime.Parse("2007-01-01T00:00:00Z"), DateTime.Parse("2007-01-01T00:00:00Z"), "closed").GetAwaiter().GetResult();

            Assert.AreEqual("59d3d6438b3f16667f9e7174", result.Value.ReleaseId);
            Assert.AreEqual("Amazing title", result.Value.Title);
        }


        [Test]
        public void GetIssuesForReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.GetReleaseClient(zenHubReleaseId).GetIssuesAsync().GetAwaiter().GetResult();

            Assert.AreEqual(103707262, result.Value[0].RepositoryId);
        }

        [Test]
        public void SetIssueEstimate1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetIssueClient(repoId, issueNumber).SetEstimateAsync(15).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var response = _zenhubClient.GetReleaseClient(zenHubReleaseId).AddIssuesAsync(new Issue[] { }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToReleaseReport2()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var response = _zenhubClient.GetReleaseClient(zenHubReleaseId).AddIssuesAsync(new Issue[] { ObjectCreator.CreateIssue(1, 2) }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveIssueFromReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var response = _zenhubClient.GetReleaseClient(zenHubReleaseId).RemoveIssuesAsync(new Issue[] { }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveIssueFromReleaseReport2()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var response = _zenhubClient.GetReleaseClient(zenHubReleaseId).RemoveIssuesAsync(new Issue[] { ObjectCreator.CreateIssue(1, 2) }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveIssueFromEpic1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber).RemoveIssuesAsync(new Issue[] { }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveIssueFromEpic2()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber).RemoveIssuesAsync().GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveIssueFromEpic3()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber).RemoveIssuesAsync((List<Issue>)null).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveIssueFromEpic4()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber)
                .RemoveIssuesAsync(new[] { ObjectCreator.CreateIssue(1, 2), ObjectCreator.CreateIssue(2, 2) })
                .GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToEpic1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber).AddIssuesAsync(new Issue[] { }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToEpic2()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber).AddIssuesAsync().GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToEpic3()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber).AddIssuesAsync((List<Issue>)null).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToEpic4()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.GetEpicClient(repoId, issueNumber)
                .AddIssuesAsync(new[] { ObjectCreator.CreateIssue(1, 2), ObjectCreator.CreateIssue(2, 2) })
                .GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void MoveIssueToPipeline1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;
            string workspaceId = MockServer.ZenHubWorkspaceId;
            string pipelineId = MockServer.ZenHubPipelineId;

            var response = _zenhubClient.GetIssueClient(repoId, issueNumber).MoveToPipelineAsync(workspaceId, pipelineId, 1).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void MoveIssueToPipelineOld1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;
            string pipelineId = MockServer.ZenHubPipelineId;

            var response = _zenhubClient.GetIssueClient(repoId, issueNumber).MoveToPipelineInOldestWorkspaceAsync(pipelineId, 1).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void ConvertIssueToEpic1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;
            var response = _zenhubClient.GetIssueClient(repoId, issueNumber).ConvertToEpicAsync(Enumerable.Empty<Issue>()).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void ConvertIssueToEpic2()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;
            var response = _zenhubClient.GetIssueClient(repoId, issueNumber).ConvertToEpicAsync(new Issue[] { ObjectCreator.CreateIssue(3, 13550592), ObjectCreator.CreateIssue(1, 13550592) }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void ConvertIssueToEpic3()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;
            var response = _zenhubClient.GetIssueClient(repoId, issueNumber).ConvertToEpicAsync((List<Issue>)null).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void ConvertEpicToIssue1()
        {
            long repoId = MockServer.repositoryId;
            int issueNumber = MockServer.issueNumber;
            var response = _zenhubClient.GetEpicClient(repoId, issueNumber)
                                        .ConvertToIssueAsync()
                                        .GetAwaiter()
                                        .GetResult();
            Assert.AreEqual(200, response.Status);
        }


        [Test]
        public void GetMilestoneStart1()
        {
            long repoId = MockServer.repositoryId;
            var result = _zenhubClient.GetRepositoryClient(new Repository(repoId)).GetMilestoneStartAsync(new Milestone(MockServer.milestoneNumber)).GetAwaiter().GetResult();

            Assert.AreEqual(DateTime.Parse("2010-11-13T01:38:56.842Z").ToLocalTime(), result.Value.Start.ToLocalTime());
        }

        [Test]
        public void SetMilestoneStart1()
        {
            long repoId = MockServer.repositoryId;
            DateTime startDate = DateTime.Parse("2019-11-01T07:00:00Z");
            var result = _zenhubClient.GetRepositoryClient(new Repository(repoId)).SetMilestoneStartAsync(new Milestone(MockServer.milestoneNumber), startDate).GetAwaiter().GetResult();

            Assert.AreEqual(startDate.ToUniversalTime(), (DateTime)result.Value.Start.ToUniversalTime());
        }

        [Test]
        public void CreateDependency1()
        {
            var result = _zenhubClient.GetIssueClient(MockServer.repositoryId, MockServer.issueNumber).AddBlockedByAsync(ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repositoryId)).GetAwaiter().GetResult();

            Assert.AreEqual(MockServer.repositoryId, result.Value.Blocking.RepositoryId);
            Assert.AreEqual(MockServer.issueNumber, result.Value.Blocking.IssueNumber);
        }

        [Test]
        public void DeleteDependency1()
        {
            var response = _zenhubClient.GetIssueClient(ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repositoryId))
                .RemoveBlockedByAsync(ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repositoryId)).GetAwaiter().GetResult();

            Assert.AreEqual(204, response.Status);
        }

        [Test]
        public void CreateReleaseReport1()
        {
            var result = _zenhubClient.GetRepositoryClient(new Repository(MockServer.repositoryId)).CreateReleaseReportAsync("", "", DateTime.Parse("2019-11-19T08:00:00Z"), DateTime.Parse("2019-11-19T08:00:00Z"), Enumerable.Empty<Repository>()).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59dff4f508399a35a276a1ea", result.Value.ReleaseId);
        }

        [Test]
        public void CreateReleaseReport2()
        {
            var result = _zenhubClient.GetRepositoryClient(new Repository(MockServer.repositoryId)).CreateReleaseReportAsync("", "", DateTime.Parse("2019-11-19T08:00:00Z"), DateTime.Parse("2019-11-19T08:00:00Z"), new Repository[] { new Repository(MockServer.repositoryId) }).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59dff4f508399a35a276a1ea", result.Value.ReleaseId);
        }

        [Test]
        public void AddRepositoryToReleaseReport1()
        {
            var response = _zenhubClient.GetReleaseClient(MockServer.ZenHubReleaseId).AddRepositoryAsync(new Repository(MockServer.repositoryId)).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveRepositoryFromReleaseReport1()
        {
            var response = _zenhubClient.GetReleaseClient(MockServer.ZenHubReleaseId).RemoveRepositoryAsync(new Repository(MockServer.repositoryId)).GetAwaiter().GetResult();
            Assert.AreEqual(204, response.Status);
        }

        [Test]
        public void GetIssueDataAsyncTest_404()
        {
            var exception = Assert.ThrowsAsync<RequestFailedException>(async () =>  await _zenhubClient.GetReleaseClient("error_" + MockServer.ZenHubReleaseId).RemoveRepositoryAsync(new Repository(MockServer.repositoryId + 1)));
            Assert.AreEqual(404, exception.Status);
        }
    }
}