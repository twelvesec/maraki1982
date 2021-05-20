using Newtonsoft.Json;

namespace Maraki1982.Core.Models.Database
{
    public class MsToken : Token
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public override string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public override string RefreshToken { get; set; }
    }
}
