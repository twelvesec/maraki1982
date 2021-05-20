using Maraki1982.Core.Dto.Core.Mail;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Mail
{
    public class MsEmailsDto : EmailsDto
    {
        public override EmailDto[] Emails { get; set; }

        [JsonProperty("value")]
        public MsEmailDto[] MsEmails { set { Emails = value; } }

        [JsonProperty("@odata.nextLink")]
        public override string NextLink { get; set; }
    }
}
