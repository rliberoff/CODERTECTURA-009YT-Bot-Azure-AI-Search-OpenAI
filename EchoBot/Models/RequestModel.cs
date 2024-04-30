using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Xml;

namespace EchoBot2.Bots.Models.RequestModel
{  
    public class RequestModel
    {
        public Datasource[] dataSources { get; set; }
        public string deployment { get; set; }
        public int max_tokens { get; set; }
        public Message[] messages { get; set; }
        public bool stream { get; set; }
        public double temperature { get; set; }
        public int top_p { get; set; }

    }

    public class Datasource
    {
        public Parameters parameters { get; set; }
        public string type { get; set; }
    }

    public class Parameters
    {
        public string embeddingDeploymentName { get; set; }
        public string endpoint { get; set; }
        public Fieldsmapping fieldsMapping { get; set; }
        public object filter { get; set; }
        public string indexName { get; set; }
        public bool inScope { get; set; }
        public string key { get; set; }
        public string queryType { get; set; }
        public string roleInformation { get; set; }
        public string semanticConfiguration { get; set; }
        public int strictness { get; set; }
        public int topNDocuments { get; set; }
    }

    public class Fieldsmapping
    {
    }

    public class Message
    {
        public string content { get; set; }
        public string role { get; set; }
    }

}
