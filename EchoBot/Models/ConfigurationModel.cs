using Microsoft.Net.Http.Headers;

namespace EchoBot2.Models
{
    public class ConfigurationModel
    {
        public AISearchConfiguration AISearchConfig { get; set; }
        public OpenAIConfiguration OpenAIConfig { get; set; }
    }

    public class AISearchConfiguration
    {
        public string DataSourceType { get; set; }
        public string EmbeddingDeploymentName { get; set; }
        public string Endpoint { get; set; }
        public string IndexName { get; set; }
        public string Key { get; set; }
        public string QueryType { get; set; }
        public string SemanticConfiguration { get; set; }
        public int Strictness { get; set; }
        public int TopNDocuments { get; set; }
    }

    public class OpenAIConfiguration
    {
        public string Deployment { get; set; }
        public int Max_tokens { get; set; }
        public double Temperature { get; set; }
        public int Top_p { get; set; }
        public string MainRole { get; set; }
        public string ContentMainRole { get; set; }
        public string UserRole { get; set; }
        public bool Steam { get; set; }
    }

}
