using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGitClient.Services
{
    public sealed class FileEditor : IFileEditor
    {
        public string EditCustomFile(string originalFile, string codeToBeInserted)
        {
            string outputCode = originalFile;
            string customRegionMarker = "#region ForCodeInsertion";
            int searchTextLength = customRegionMarker.Length;
            int x = originalFile.IndexOf(customRegionMarker);
            if (x != -1)
            {
                outputCode = originalFile.Insert(x + searchTextLength, $"\n\t\t{codeToBeInserted}");
            }        
            return outputCode;        
        }        
    }
}
