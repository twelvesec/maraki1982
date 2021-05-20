using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maraki1982.Web.Models.Changelog
{
    public class ChangelogDetailsModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("items")]
        public List<string> Items { get; set; }
    }
}