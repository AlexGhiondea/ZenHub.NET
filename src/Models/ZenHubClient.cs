using Azure.Core.Pipeline;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZenHub.Pipeline;

namespace ZenHub
{
    public class ZenHubClient
    {
        HttpPipeline _pipeline;

        private static readonly string EndPoint = "https://api.zenhub.io";

        public ZenHubClient(string authToken) : this(authToken, null)
        {
        }

        public ZenHubClient(string authToken, ZenHubClientOptions clientOptions)
        {
            clientOptions ??= new ZenHubClientOptions();

            _pipeline = HttpPipelineBuilder.Build(clientOptions, new ZenHubAuthenticationPolicy(authToken), new ThrowOnErrorStatusPolicy());
        }

        private async Task<dynamic> MakeRequestAsync(RequestMethod method, string endpoint, string jsonBody = "")
        {
            var request = _pipeline.CreateRequest();
            request.Method = method;
            request.UriBuilder.Uri = new Uri(endpoint);

            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.Content = HttpPipelineRequestContent.Create(Encoding.UTF8.GetBytes(jsonBody));
            }

            var response = await _pipeline.SendRequestAsync(request, CancellationToken.None).ConfigureAwait(false);

            // read the content from the stream
            // The policy will throw if we received an invalid response
            using (StreamReader sr = new StreamReader(response.ContentStream))
            {
                return JsonConvert.DeserializeObject<dynamic>(await sr.ReadToEndAsync());
            }
        }

        public async Task<dynamic> GetIssueDataAsync(long repoId, int issueNumber)
        { 
            return await MakeRequestAsync(RequestMethod.Get,  $"{EndPoint}/p1/repositories/{repoId}/issues/{issueNumber}");
        }

        public async Task<dynamic> GetIssueEventsAsync(long repoId, int issueNumber)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repoId}/issues/{issueNumber}/events");
        }

        public async Task<dynamic> GetEpicsAsync(long repoId)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repoId}/epics");
        }

        public async Task<dynamic> GetEpicDataAsync(long repoId, int epicId)
        {
            return await MakeRequestAsync(RequestMethod.Get, $"{EndPoint}/p1/repositories/{repoId}/epics/{epicId}");
        }

        public async Task<bool> SetEstimateAsync(long repoId, int issueNumber, int estimate)
        {
            var contentBody = new
            {
                estimate
            };

            var result = await MakeRequestAsync(RequestMethod.Put, $"{EndPoint}/p1/repositories/{repoId}/issues/{issueNumber}/estimate", JsonConvert.SerializeObject(contentBody));

            return true;
        }

        public async Task<dynamic> AddIssueToEpic(long repoId, int issueNumber, long repoIdToAdd, int issueNumberToAdd)
        {
            // This limits the list of issues added to 1.
            var contentBody = new
            {
                remove_issues = new object[0],
                add_issues = new object[1] { new { repo_id = repoIdToAdd, issue_number = issueNumberToAdd } }
            };

            return await MakeRequestAsync(RequestMethod.Post, $"{EndPoint}/p1/repositories/{repoId}/epics/{issueNumber}/update_issues", JsonConvert.SerializeObject(contentBody));
        }
    }
}
