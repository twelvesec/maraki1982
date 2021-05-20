using Maraki1982.Core.Dto.Core.Drive;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Drive
{
    public class MsDriveDto : DriveDto
    {
        [JsonProperty("id")]
        public override string Id { get; set; }

        [JsonProperty("driveType")]
        public override string DriveType { get; set; }
    }
}
