using Azure;
using Azure.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ZenHub.Pipeline;

namespace ZenHub
{
    public abstract class Client
    {
        protected readonly ZenHubClientOptions _options;
        protected readonly HttpPipeline _pipeline;

        internal Client(HttpPipeline pipeline, ZenHubClientOptions options)
        {
            _options = options;
            _pipeline = pipeline;
        }

        protected async Task<Response<T>> MakeRequestAsync<T>(RequestMethod method, string endpoint, string jsonBody = "", CancellationToken cancellationToken = default)
        {
            var response = await MakeRequestAsync(method, endpoint, jsonBody, cancellationToken).ConfigureAwait(false);

            // read the content from the stream
            // The policy will throw if we received an invalid response
            using StreamReader sr = new StreamReader(response.ContentStream);
            var deserializeValue = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(response.ContentStream, cancellationToken: cancellationToken).ConfigureAwait(false);

            return new Response<T>(response, deserializeValue);
        }

        protected async Task<Response> MakeRequestAsync(RequestMethod method, string endpoint, string jsonBody = "", CancellationToken cancellationToken = default)
        {
            var request = _pipeline.CreateRequest();
            request.Method = method;
            request.UriBuilder.Uri = new Uri(endpoint);

            if (!string.IsNullOrEmpty(jsonBody))
            {
                request.Content = HttpPipelineRequestContent.Create(Encoding.UTF8.GetBytes(jsonBody));
                request.Headers.Add("Content-Type", "application/json");
            }

            return await _pipeline.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
