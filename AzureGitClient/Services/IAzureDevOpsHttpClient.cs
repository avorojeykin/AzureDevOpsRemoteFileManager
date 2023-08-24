using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureGitClient.Models;

namespace AzureGitClient.Services
{
    public interface IAzureDevOpsHttpClient
    {
        List<SHA1ItemIdValue> GetListOfIdsOfRepoFilesByRepo(string repoName);
        bool BranchExists(GitHttpClient gitClient, GitRepository repo, string branchName);
        bool CreateBranch(string baseBranchName, string featureBranchName);      
        string GetIdOfSpecificRepoFile(string filePath, List<SHA1ItemIdValue> items);
        string GetContentOfSpecificRepoFilesById(string sha1Id);
        string Base64Encode(string plainText);        
        string PushAddFileBranchChanges(string branchName, string content, string pushURL, string commitComment);
        string PushEditFileBranchChanges(string branchName, string content, string pushURL, string commitComment);
    }
}
