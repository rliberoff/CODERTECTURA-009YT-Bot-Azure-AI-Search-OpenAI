using System.Linq;

namespace EchoBot2.Bots.Models.ResponseModel
{
    public class ResponseModel
    {
        public Choice[] choices { get; set; }
        public int created { get; set; }
        public string id { get; set; }
        public string model { get; set; }
        public string _object { get; set; }
        public Usage usage { get; set; }

        public string GetResponseContent() => 
            choices[0].messages.Where(message => message.role == "assistant").Select(Message => Message.content).FirstOrDefault();
		
    }

    public class Usage
    {
        public int completion_tokens { get; set; }
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
    }

    public class Choice
    {
        public int index { get; set; }
        public Message[] messages { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
        public bool end_turn { get; set; }
        public int index { get; set; }
        public string role { get; set; }
    }


}
