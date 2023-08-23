using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGitClient.Services
{
    public interface IFileEditor
    {
        string editCustomFactoryFile(string originalFile, string textToBeInserted);
        string editCustomTypesFile(string originalFile, string textToBeInserted);
        string editCustomEditsFile(string originalFile, string textToBeInserted);
    }
}
