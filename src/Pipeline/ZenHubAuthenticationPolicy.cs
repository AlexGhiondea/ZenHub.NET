using Azure.Core.Pipeline;
using System;
using System.Threading.Tasks;

namespace ZenHub.Pipeline
{
    public class ZenHubAuthenticationPolicy : HttpPipelinePolicy
    {
        private readonly string _authToken;
        public ZenHubAuthenticationPolicy(string authToken)
        {
            _authToken = authToken;
        }

        public override void Process(HttpPipelineMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            message.Request.Headers.Add("X-Authentication-Token", _authToken);
            ProcessNext(message, pipeline);
        }

        public override async Task ProcessAsync(HttpPipelineMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            message.Request.Headers.Add("X-Authentication-Token", _authToken);
            await ProcessNextAsync(message, pipeline);
        }
    }
}
