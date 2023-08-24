using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGitClient.Models
{
    internal class SHA1ItemIdList
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("value")]
        public List<SHA1ItemIdValue> Value { get; set; }
    }
}
