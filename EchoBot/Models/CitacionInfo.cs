using System.Collections.Generic;

namespace EchoBot2.Bots.Models
{
    public class CitacionInfo
    {
        public List<CitationDetails> citations { get; set; }

        public string intent { get; set; }
    }
}
