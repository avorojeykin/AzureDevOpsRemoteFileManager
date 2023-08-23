using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AzureGitClient.Common
{
    internal class CommonUtilities
    {
        public static T DeserializeJsontoObject<T>(string jsonToDeserialize)
        {
            return JsonConvert.DeserializeObject<T>(jsonToDeserialize);
        }
    }
}
