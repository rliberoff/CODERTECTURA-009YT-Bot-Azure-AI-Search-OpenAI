using EchoBot2.Bots.Models.RequestModel;
using EchoBot2.Bots.Models.ResponseModel;
using EchoBot2.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EchoBot2.Services
{
	public class OpenAIService : IOpenAIService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ConfigurationModel _configuration;

		public OpenAIService(IHttpClientFactory httpClientFactory, IOptions<ConfigurationModel> configuration)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration.Value;
		}

		public async Task<ResponseModel> SendRequest(string message)
		{
			using (HttpClient client = _httpClientFactory.CreateClient("OpenAI"))
			{
				RequestModel request = CreateRequest(message);

				var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

				HttpRequestMessage requestMessage = new HttpRequestMessage() { Content = content, Method = HttpMethod.Post };

				var response = await client.SendAsync(requestMessage);

				if (response.IsSuccessStatusCode)
				{
					return JsonSerializer.Deserialize<ResponseModel>(response.Content.ReadAsStream());
				}
				else
				{
					throw new Exception($"Error: {response.StatusCode}");
				}
			}
		}

		private RequestModel CreateRequest(string text)
		{
			RequestModel request = new RequestModel();

            Datasource ds = new Datasource();
            ds.type = _configuration.AISearchConfig.DataSourceType;


            Parameters param = new Parameters();
            param.embeddingDeploymentName = _configuration.AISearchConfig.EmbeddingDeploymentName;
            param.endpoint = _configuration.AISearchConfig.Endpoint;
            param.indexName = _configuration.AISearchConfig.IndexName;
            param.key = _configuration.AISearchConfig.Key;
            param.queryType = _configuration.AISearchConfig.QueryType;
            param.semanticConfiguration = _configuration.AISearchConfig.SemanticConfiguration;
            param.strictness = _configuration.AISearchConfig.Strictness;
            param.topNDocuments = _configuration.AISearchConfig.TopNDocuments;

            ds.parameters = param;

            request.dataSources = new Datasource[] { ds };
            request.deployment = _configuration.OpenAIConfig.Deployment;
            request.max_tokens = _configuration.OpenAIConfig.Max_tokens;
            request.temperature = _configuration.OpenAIConfig.Temperature;
            request.top_p = _configuration.OpenAIConfig.Top_p;
			request.stream = _configuration.OpenAIConfig.Steam;

            Bots.Models.RequestModel.Message message = new Bots.Models.RequestModel.Message();
			message.role = _configuration.OpenAIConfig.MainRole;
			message.content = _configuration.OpenAIConfig.ContentMainRole;

            Bots.Models.RequestModel.Message message2 = new Bots.Models.RequestModel.Message();
			message2.role = _configuration.OpenAIConfig.UserRole;
            message2.content = text;

            request.messages = new Bots.Models.RequestModel.Message[] { message, message2 };


            return request;
		}
	}
}
