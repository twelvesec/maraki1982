using Maraki1982.Core.Dto.Core.Drive;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Drive
{
    public class MsFoldersAndFilesDto : FoldersAndFilesDto
    {
        public override FolderAndFileDto[] FoldersAndFiles { get; set; }

        [JsonProperty("value")]
        public MsFolderAndFileDto[] MsFoldersAndFiles { set { FoldersAndFiles = value; } }

        [JsonProperty("@odata.nextLink")]
        public override string NextLink { get; set; }
    }
}
