using Maraki1982.Core.Dto.Core.Mail;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Mail
{
    public class MsEmailDto : EmailDto
    {
        [JsonProperty("internetMessageId")]
        public override string Id { get; set; }

        [JsonProperty("subject")]
        public override string Subject { get; set; }

        public override EmailBodyDto Body { get; set; }

        [JsonProperty("body")]
        public MsEmailBodyDto MsBody { set { Body = value; } }
    }
}
