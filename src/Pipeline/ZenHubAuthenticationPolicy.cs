using Azure.Core;
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

        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            // no sync code.
        }

        public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            message.Request.Headers.Add("X-Authentication-Token", _authToken);
#pragma warning restore CA1062 // Validate arguments of public methods
            await ProcessNextAsync(message, pipeline);
        }
    }
}
