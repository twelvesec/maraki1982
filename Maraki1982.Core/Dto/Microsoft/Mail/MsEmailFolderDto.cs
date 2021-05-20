using Maraki1982.Core.Dto.Core.Mail;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Mail
{
    public class MsEmailFolderDto : EmailFolderDto
    {
        [JsonProperty("id")]
        public override string Id { get; set; }

        [JsonProperty("displayName")]
        public override string Name { get; set; }
    }
}
