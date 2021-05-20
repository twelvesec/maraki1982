using Maraki1982.Core.Dto.Core;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Google
{
    public class GooglePersonalInfoDto : PersonalInfoDto
    {
        [JsonProperty("sub")]
        public override string Id { get; set; }

        [JsonProperty("email")]
        public override string Email { get; set; }

        [JsonProperty("name")]
        public override string Name { get; set; }
    }
}
