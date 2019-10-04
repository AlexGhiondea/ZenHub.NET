using NUnit.Framework;
using WireMock.Server;
using WireMock.Matchers.Request;
using WireMock.ResponseBuilders;
using WireMock.RequestBuilders;
using ZenHub;
using ZenHub.Pipeline;
using Octokit;
using System.Linq;
using System;
using RestEase;
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
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;

            var result = _zenhubClient.GetIssueDataAsync(repoId, issueNumber).GetAwaiter().GetResult();

            Assert.AreEqual(8, result.Value.Estimate.Value);
            Assert.AreEqual(true, result.Value.IsEpic);
            Assert.AreEqual("5d0a7cea41fd098f6b7f58b7", result.Value.Pipelines[1].PipelineId);
        }

        [Test]
        public void GetIssueEvents1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;

            var result = _zenhubClient.GetIssueEventsAsync(repoId, issueNumber).GetAwaiter().GetResult();

            Assert.AreEqual(16717, result.Value[0].UserId);
            Assert.AreEqual("transferIssue", result.Value[3].EventType);
        }

        [Test]
        public void GetEpicsForRepo1()
        {
            long repoId = MockServer.repoId;

            var result = _zenhubClient.GetEpicsAsync(repoId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3953, result.Value.Epics[0].IssueNumber);
        }

        [Test]
        public void GetEpicData1()
        {
            long repoId = MockServer.repoId;
            int epicId = MockServer.issueNumber;


            var result = _zenhubClient.GetEpicDataAsync(repoId, epicId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3161, result.Value.Issues[0].IssueNumber);
        }

        [Test]
        public void GetWorkspaces1()
        {
            long repoId = MockServer.repoId;

            var result = _zenhubClient.GetWorkspaces(new Octokit.Repository(repoId)).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("Design and UX", result.Value[0].Name);
            Assert.AreEqual(12345678, (int)result.Value[1].Repositories[0]);
        }

        [Test]
        public void GetZenHubBoard1()
        {
            var repository = new Octokit.Repository(MockServer.repoId);
            string ZenHubWorkspaceId = MockServer.ZenHubWorkspaceId;

            var result = _zenhubClient.GetZenHubBoard(repository, ZenHubWorkspaceId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("595d430add03f01d32460080", result.Value.Pipelines[0].Id);
        }

        [Test]
        public void GetZenHubBoard2()
        {
            var repository = new Octokit.Repository(MockServer.repoId);

            var result = _zenhubClient.GetOldestZenHubBoard(repository).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("595d430add03f01d32460080", result.Value.Pipelines[0].Id);
        }

        [Test]
        public void GetDependencyForRepo1()
        {
            var repository = new Octokit.Repository(MockServer.repoId);

            var result = _zenhubClient.GetDependencies(repository).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3953, result.Value.Dependencies[0].Blocking.IssueNumber);
        }

        [Test]
        public void GetReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.GetReleaseReport(zenHubReleaseId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59d3cd520a430a6344fd3bdb", result.Value.ReleaseId);
        }

        [Test]
        public void GetReleaseReportForRepo1()
        {
            var repository = new Octokit.Repository(MockServer.repoId);

            var result = _zenhubClient.GetReleaseReports(repository).GetAwaiter().GetResult();

            Assert.AreEqual("59cbf2fde010f7a5207406e8", result.Value[0].ReleaseId);
        }


        [Test]
        public void EditReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.EditReleaseReport(zenHubReleaseId, "Amazing title", "Amazing description", DateTime.Parse("2007-01-01T00:00:00Z"), DateTime.Parse("2007-01-01T00:00:00Z"), "closed").GetAwaiter().GetResult();

            Assert.AreEqual("59d3d6438b3f16667f9e7174", result.Value.ReleaseId);
            Assert.AreEqual("Amazing title", result.Value.Title);
        }


        [Test]
        public void GetIssuesForReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.GetAllIssuesInReleaseReport(zenHubReleaseId).GetAwaiter().GetResult();

            Assert.AreEqual(103707262, result.Value[0].RepositoryId);
        }

        [Test]
        public void SetIssueEstimate1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.SetEstimateAsync(repoId, issueNumber, 15).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var response = _zenhubClient.ChangeIssuesToReleaseReport(zenHubReleaseId, new Issue[] { }, new Issue[] { }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void AddIssueToEpic1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;

            var response = _zenhubClient.AddOrRemoveIssueToEpic(ObjectCreator.CreateIssue(issueNumber, repoId), new Issue[] { }, new Issue[] { }).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void MoveIssueToPipeline1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            string workspaceId = MockServer.ZenHubWorkspaceId;
            string pipelineId = MockServer.ZenHubPipelineId;

            var response = _zenhubClient.MoveIssueToPipeline(ObjectCreator.CreateIssue(issueNumber, repoId), workspaceId, pipelineId, 1).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void MoveIssueToPipelineOld1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            string pipelineId = MockServer.ZenHubPipelineId;

            var response = _zenhubClient.MoveIssueToPipelineOldestWorkspace(ObjectCreator.CreateIssue(issueNumber, repoId), pipelineId, 1).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void ConvertIssueToEpic1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            var response = _zenhubClient.ConvertIssueToEpic(ObjectCreator.CreateIssue(issueNumber, repoId)).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void ConvertIssueToEpic2()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            var response = _zenhubClient.ConvertIssueToEpic(ObjectCreator.CreateIssue(issueNumber, repoId), ObjectCreator.CreateIssue(3, 13550592), ObjectCreator.CreateIssue(1, 13550592)).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void ConvertEpicToIssue1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            var response = _zenhubClient.ConvertEpicToIssue(ObjectCreator.CreateIssue(issueNumber, repoId)).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }


        [Test]
        public void GetMilestoneStart1()
        {
            long repoId = MockServer.repoId;
            var result = _zenhubClient.GetMilestoneStart(new Repository(repoId), new Milestone(MockServer.milestoneNumber)).GetAwaiter().GetResult();

            Assert.AreEqual(DateTime.Parse("2010-11-13T01:38:56.842Z").ToLocalTime(), result.Value.Start.ToLocalTime());
        }

        [Test]
        public void SetMilestoneStart1()
        {
            long repoId = MockServer.repoId;
            DateTime startDate = new DateTime(2019, 11, 1);
            var result = _zenhubClient.SetMilestoneStart(new Repository(repoId), new Milestone(MockServer.milestoneNumber), startDate).GetAwaiter().GetResult();

            Assert.AreEqual(startDate.ToUniversalTime(), (DateTime)result.Value.Start.ToUniversalTime());
        }

        [Test]
        public void CreateDependency1()
        {
            var result = _zenhubClient.CreateDependency(ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId), ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId)).GetAwaiter().GetResult();

            Assert.AreEqual(MockServer.repoId, result.Value.Blocking.RepositoryId);
            Assert.AreEqual(MockServer.issueNumber, result.Value.Blocking.IssueNumber);
        }

        [Test]
        public void DeleteDependency1()
        {
            var response = _zenhubClient.DeleteDependency(
                ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId),
                ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId)).GetAwaiter().GetResult();

            Assert.AreEqual(204, response.Status);
        }

        [Test]
        public void CreateReleaseReport1()
        {
            var result = _zenhubClient.CreateReleaseReport(new Repository(MockServer.repoId), "", "", new DateTime(2019, 11, 19), new DateTime(2019, 11, 19), Enumerable.Empty<Repository>()).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59dff4f508399a35a276a1ea", result.Value.ReleaseId);
        }

        [Test]
        public void AddRepositoryToReleaseReport1()
        {
            var response = _zenhubClient.AddRepositoryToReleaseReport(MockServer.ZenHubReleaseId, new Repository(MockServer.repoId)).GetAwaiter().GetResult();
            Assert.AreEqual(200, response.Status);
        }

        [Test]
        public void RemoveRepositoryFromReleaseReport1()
        {
            var response = _zenhubClient.RemoveRepositoryFromReleaseReport(MockServer.ZenHubReleaseId, new Repository(MockServer.repoId)).GetAwaiter().GetResult();
            Assert.AreEqual(204, response.Status);
        }
    }
}