using NUnit.Framework;
using System;

namespace ZenHub.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class ErrorCaseTests
    {
        [Test]
        public void GetRepositoryClient_Null()
        {
            ZenHubClient zh = new ZenHubClient("dummy");
            Assert.Throws<ArgumentNullException>(() => zh.GetRepositoryClient(null));
        }

        [Test]
        public void GetEpicClient_Null()
        {
            ZenHubClient zh = new ZenHubClient("dummy");
            Assert.Throws<ArgumentNullException>(() => zh.GetEpicClient(null));
        }

        [Test]
        public void GetIssueClient_Null()
        {
            ZenHubClient zh = new ZenHubClient("dummy");
            Assert.Throws<ArgumentNullException>(() => zh.GetIssueClient(null));
        }

        [Test]
        public void GetReleaseClient_Null()
        {
            ZenHubClient zh = new ZenHubClient("dummy");
            Assert.Throws<ArgumentNullException>(() => zh.GetReleaseClient(null));
        }

        [Test]
        public void RepositoryMethods_Null()
        {
            ZenHubClient zh = new ZenHubClient("dummy");
            var repCl = zh.GetRepositoryClient(1);

            Assert.Throws<ArgumentNullException>(() => repCl.GetMilestoneStartAsync(null).GetAwaiter().GetResult());
            Assert.Throws<ArgumentNullException>(() => repCl.SetMilestoneStartAsync(null, DateTime.Now).GetAwaiter().GetResult());
        }

        [Test]
        public void ReleasesMethods_Null()
        {
            ZenHubClient zh = new ZenHubClient("dummy");
            var relCl = zh.GetReleaseClient("release");

            Assert.Throws<ArgumentNullException>(() => relCl.AddRepositoryAsync(null).GetAwaiter().GetResult());
            Assert.Throws<ArgumentNullException>(() => relCl.RemoveRepositoryAsync(null).GetAwaiter().GetResult());
        }

        [Test]
        public void IssueMethods_Null()
        {
            ZenHubClient zh = new ZenHubClient("dummy");
            var icl = zh.GetIssueClient(1, 2);

            Assert.Throws<ArgumentNullException>(() => icl.AddBlockedByAsync(null).GetAwaiter().GetResult());
            Assert.Throws<ArgumentNullException>(() => icl.RemoveBlockedByAsync(null).GetAwaiter().GetResult());
        }
    }
}
