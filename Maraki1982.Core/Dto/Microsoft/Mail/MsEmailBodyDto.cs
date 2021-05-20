using Maraki1982.Core.Dto.Core.Mail;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Mail
{
    public class MsEmailBodyDto : EmailBodyDto
    {
        [JsonProperty("content")]
        public override string Content { get; set; }
    }
}
