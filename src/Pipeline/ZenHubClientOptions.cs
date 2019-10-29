using Azure.Core.Pipeline;

namespace ZenHub.Pipeline
{
    public class ZenHubClientOptions : ClientOptions
    {

        public string EndPoint { get; set; }
    }
}
