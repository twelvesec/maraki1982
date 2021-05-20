using Newtonsoft.Json;

namespace Maraki1982.Web.Models
{
    public class Log
    {
        [JsonProperty("@t")]
        public string Date { get; set; }

        [JsonProperty("@m")]
        public string Message { get; set; }

        [JsonProperty("@x")]
        public string Exception { get; set; }
    }
}
