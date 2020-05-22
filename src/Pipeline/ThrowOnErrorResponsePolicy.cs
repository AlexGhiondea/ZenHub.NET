using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ZenHub.Pipeline
{
    internal class ThrowOnErrorStatusPolicy : HttpPipelinePolicy
    {
        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            // no sync code.
        }

        public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            await ProcessNextAsync(message, pipeline).ConfigureAwait(false);
            if (message.ResponseClassifier.IsErrorResponse(message))
            {
                throw new RequestFailedException(message.Response.Status, message.ToString());
            }
        }
    }
}
