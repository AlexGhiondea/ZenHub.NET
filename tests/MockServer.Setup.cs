using System.IO;
using WireMock.Server;

namespace ZenHub.Tests
{
    internal class MockServer
    {
        public FluentMockServer Server { get; }
        public string EndPoint => Server.Urls[0];

        public MockServer()
        {
            Server = FluentMockServer.Start();
        }

        public const long repositoryId = 1;
        public const int issueNumber = 2;
        public const int milestoneNumber = 1;
        public const string ZenHubWorkspaceId = "dummyWorkspace";
        public const string ZenHubReleaseId = "dummyRelease";
        public const string ZenHubPipelineId = "pipelineDummy";

        public void RegisterServer()
        {
            Server.ReadStaticMappings(
                Path.Combine(
                    Path.GetDirectoryName(typeof(MockServer).Assembly.Location),
                    "..", "..", "..", "testData")
                );
        }

        internal void Stop()
        {
            Server.Stop();
        }
    }
}
