using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGitClient.Models
{
    public class SHA1ItemIdValue
    {
        [JsonProperty("objectId")]
        public string? ObjectId { get; set; }
        [JsonProperty("gitObjectType")]
        public string? GitObjectType { get; set; }
        [JsonProperty("commitId")]
        public string? CommitId { get; set; }
        [JsonProperty("path")]
        public string? Path { get; set; }
        [JsonProperty("isFolder")]
        public string? IsFolder { get; set; }
        [JsonProperty("url")]
        public string? Url { get; set; }
    }
}
