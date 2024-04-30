using EchoBot2.Bots.Models.ResponseModel;
using System.Threading.Tasks;

namespace EchoBot2.Services
{
	public interface IOpenAIService
	{
		public Task<ResponseModel> SendRequest(string message);
	}
}
