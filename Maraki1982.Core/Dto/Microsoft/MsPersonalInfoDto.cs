using Maraki1982.Core.Dto.Core;
using Newtonsoft.Json;

namespace Maraki1982.Core.Dto.Microsoft
{
    public class MsPersonalInfoDto : PersonalInfoDto
    {
        [JsonProperty("id")]
        public override string Id { get; set; }

        [JsonProperty("userPrincipalName")]
        public override string Email { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }
    }
}
