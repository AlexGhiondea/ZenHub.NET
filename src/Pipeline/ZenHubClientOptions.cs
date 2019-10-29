using Azure.Core;

namespace ZenHub.Pipeline
{
    public class ZenHubClientOptions : ClientOptions
    {
        public string EndPoint { get; set; }
    }
}
