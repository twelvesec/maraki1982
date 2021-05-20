using Maraki1982.Core.Dto.Core.Drive;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Drive
{
    public class MsFolderAndFileDto : FolderAndFileDto
    {
        [JsonProperty("id")]
        public override string Id { get; set; }

        [JsonProperty("name")]
        public override string Name { get; set; }

        [JsonProperty("@microsoft.graph.downloadUrl")]
        public override string DownloadUrl { get; set; }

        [JsonProperty("folder")]
        public override dynamic Folder { get; set; }
    }
}
