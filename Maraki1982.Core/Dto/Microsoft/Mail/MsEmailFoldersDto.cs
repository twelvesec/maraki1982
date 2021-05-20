using Maraki1982.Core.Dto.Core.Mail;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Mail
{
    public class MsEmailFoldersDto : EmailFoldersDto
    {
        public override EmailFolderDto[] EmailFolders { get; set; }

        [JsonProperty("value")]
        public MsEmailFolderDto[] MsEmailFolders { set { EmailFolders = value; } }

        [JsonProperty("@odata.nextLink")]
        public override string NextLink { get; set; }
    }
}
