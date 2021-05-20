using Maraki1982.Core.Dto.Core.Drive;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft.Drive
{
    public class MsDrivesDto : DrivesDto
    {
        public override DriveDto[] Drives { get; set; }

        [JsonProperty("value")]
        public MsDriveDto[] MsDrives { set { Drives = value; } }

        [JsonProperty("@odata.nextLink")]
        public override string NextLink { get; set; }
    }
}
