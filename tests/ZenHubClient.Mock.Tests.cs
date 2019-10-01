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
        //private FluentMockServer _mockServer;

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

            Assert.NotNull(result);
            Assert.AreEqual(8, (int)result.estimate.value);
            Assert.AreEqual(true, (bool)result.is_epic);
            Assert.AreEqual("5d0a7cea41fd098f6b7f58b7", (string)result.pipelines[1].pipeline_id);
        }

        [Test]
        public void GetIssueEvents1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;

            var result = _zenhubClient.GetIssueEventsAsync(repoId, issueNumber).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(16717, (int)result[0].user_id);
            Assert.AreEqual("transferIssue", (string)result[3].type);
        }

        [Test]
        public void GetEpicsForRepo1()
        {
            long repoId = MockServer.repoId;

            var result = _zenhubClient.GetEpicsAsync(repoId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3953, (int)result.epic_issues[0].issue_number);
        }

        [Test]
        public void GetEpicData1()
        {
            long repoId = MockServer.repoId;
            int epicId = MockServer.issueNumber;


            var result = _zenhubClient.GetEpicDataAsync(repoId, epicId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3161, (int)result.issues[0].issue_number);
        }

        [Test]
        public void GetWorkspaces1()
        {
            long repoId = MockServer.repoId;

            var result = _zenhubClient.GetWorkspaces(new Octokit.Repository(repoId)).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("Design and UX", (string)result[0].name);
            Assert.AreEqual(12345678, (int)result[1].repositories[0]);
        }

        [Test]
        public void GetZenHubBoard1()
        {
            var repository = new Octokit.Repository(MockServer.repoId);
            string ZenHubWorkspaceId = MockServer.ZenHubWorkspaceId;

            var result = _zenhubClient.GetZenHubBoard(repository, ZenHubWorkspaceId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("595d430add03f01d32460080", (string)result.pipelines[0].id);
        }

        [Test]
        public void GetZenHubBoard2()
        {
            var repository = new Octokit.Repository(MockServer.repoId);

            var result = _zenhubClient.GetOldestZenHubBoard(repository).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("595d430add03f01d32460080", (string)result.pipelines[0].id);
        }

        [Test]
        public void GetDependencyForRepo1()
        {
            var repository = new Octokit.Repository(MockServer.repoId);

            var result = _zenhubClient.GetDependencies(repository).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(3953, (int)result.dependencies[0].blocking.issue_number);
        }

        [Test]
        public void GetReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.GetReleaseReport(zenHubReleaseId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59d3cd520a430a6344fd3bdb", (string)result.release_id);
        }

        [Test]
        public void GetReleaseReportForRepo1()
        {
            var repository = new Octokit.Repository(MockServer.repoId);

            var result = _zenhubClient.GetReleaseReports(repository).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59cbf2fde010f7a5207406e8", (string)result[0].release_id);
        }


        [Test]
        public void EditReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.EditReleaseReport(zenHubReleaseId, "", "", DateTime.Now, DateTime.Now, "").GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59d3d6438b3f16667f9e7174", (string)result.release_id);
        }


        [Test]
        public void GetIssuesForReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.GetAllIssuesInReleaseReport(zenHubReleaseId).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(103707262, (int)result[0].repo_id);
        }

        [Test]
        public void SetIssueEstimate1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;

            var result = _zenhubClient.SetEstimateAsync(repoId, issueNumber, 15).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(15, (int)result.estimate);
        }

        [Test]
        public void AddIssueToReleaseReport1()
        {
            var zenHubReleaseId = MockServer.ZenHubReleaseId;

            var result = _zenhubClient.ChangeIssuesToReleaseReport(zenHubReleaseId, new Issue[] { }, new Issue[] { }).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(103707262, (int)result.added[0].repo_id);
        }

        [Test]
        public void AddIssueToEpic1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;

            var result = _zenhubClient.AddOrRemoveIssueToEpic(ObjectCreator.CreateIssue(issueNumber, repoId), new Issue[] { }, new Issue[] { }).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(2, (int)result.add_issues[0].issue_number);
        }

        [Test]
        public void MoveIssueToPipeline1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            string workspaceId = MockServer.ZenHubWorkspaceId;
            string pipelineId = MockServer.ZenHubPipelineId;

            var result = _zenhubClient.MoveIssueToPipeline(ObjectCreator.CreateIssue(issueNumber, repoId), workspaceId, pipelineId, 1).GetAwaiter().GetResult();

            // no response
            Assert.Null(result);
        }

        [Test]
        public void MoveIssueToPipelineOld1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            string pipelineId = MockServer.ZenHubPipelineId;

            var result = _zenhubClient.MoveIssueToPipelineOldestWorkspace(ObjectCreator.CreateIssue(issueNumber, repoId), pipelineId, 1).GetAwaiter().GetResult();

            // no response
            Assert.Null(result);
        }

        [Test]
        public void ConvertIssueToEpic1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            var result = _zenhubClient.ConvertIssueToEpic(ObjectCreator.CreateIssue(issueNumber, repoId)).GetAwaiter().GetResult();

            Assert.Null(result);
        }

        [Test]
        public void ConvertIssueToEpic2()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            var result = _zenhubClient.ConvertIssueToEpic(ObjectCreator.CreateIssue(issueNumber, repoId), ObjectCreator.CreateIssue(3, 13550592), ObjectCreator.CreateIssue(1, 13550592)).GetAwaiter().GetResult();

            Assert.Null(result);
        }

        [Test]
        public void ConvertEpicToIssue1()
        {
            long repoId = MockServer.repoId;
            int issueNumber = MockServer.issueNumber;
            var result = _zenhubClient.ConvertEpicToIssue(ObjectCreator.CreateIssue(issueNumber, repoId)).GetAwaiter().GetResult();

            Assert.Null(result);
        }


        [Test]
        public void GetMilestoneStart1()
        {
            long repoId = MockServer.repoId;
            var result = _zenhubClient.GetMilestoneStart(new Repository(repoId), new Milestone(MockServer.milestoneNumber)).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("11/13/2010 01:38:56", (string)result.start_date);
        }

        [Test]
        public void SetMilestoneStart1()
        {
            long repoId = MockServer.repoId;
            DateTime startDate = new DateTime(2019, 11, 1);
            var result = _zenhubClient.SetMilestoneStart(new Repository(repoId), new Milestone(MockServer.milestoneNumber), startDate).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(startDate.ToUniversalTime(), (DateTime)result.start_date);
        }

        [Test]
        public void CreateDependency1()
        {
            var result = _zenhubClient.CreateDependency(ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId), ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId)).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual(MockServer.repoId, (long)result.blocking.repo_id);
            Assert.AreEqual(MockServer.issueNumber, (int)result.blocking.issue_number);
        }

        [Test]
        public void DeleteDependency1()
        {
            var result = _zenhubClient.DeleteDependency(
                ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId),
                ObjectCreator.CreateIssue(MockServer.issueNumber, MockServer.repoId)).GetAwaiter().GetResult();

            Assert.Null(result);
        }

        [Test]
        public void CreateReleaseReport1()
        {
            var result = _zenhubClient.CreateReleaseReport(new Repository(MockServer.repoId), "", "", new DateTime(2019, 11, 19), new DateTime(2019, 11, 19), Enumerable.Empty<Repository>()).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.AreEqual("59dff4f508399a35a276a1ea", (string)result.release_id);
        }

        [Test]
        public void AddRepositoryToReleaseReport1()
        {
            var result = _zenhubClient.AddRepositoryToReleaseReport(MockServer.ZenHubReleaseId, new Repository(MockServer.repoId)).GetAwaiter().GetResult();

            Assert.Null(result);
        }

        [Test]
        public void RemoveRepositoryFromReleaseReport1()
        {
            var result = _zenhubClient.RemoveRepositoryFromReleaseReport(MockServer.ZenHubReleaseId, new Repository(MockServer.repoId)).GetAwaiter().GetResult();

            Assert.Null(result);
        }
    }
}