using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGitClient.Services
{
    public interface IFileEditor
    {
        string EditCustomFile(string originalFile, string codeToBeInserted);   
    }
}
