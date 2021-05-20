using Newtonsoft.Json;
using System.Collections.Generic;

namespace Maraki1982.Web.Models.Changelog
{
    public class ChangelogModel
    {
        [JsonProperty("current_version")]
        public int CurrentVersion { get; set; }

        [JsonProperty("details")]
        public List<ChangelogDetailsModel> Details {get;set;}
    }
}
