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
        ProcessResponse getListOfIdsOfRepoFilesByRepo(string repoName);
        bool branchExists(GitHttpClient gitClient, GitRepository repo, string branchName);
        bool createBranch(string baseBranchName, string featureBranchName);      
        string getIdOfSpecificRepoFile(string filePath);
        string getContentOfSpecificRepoFilesById(string sha1Id);
        string Base64Encode(string plainText);
        string pushEditBranchChanges(string branchName, string content, string pushURL, string commitComment);
        string pushAddFileBranchChanges(string branchName, string content, string pushURL, string commitComment);
        string pushEditFileBranchChanges(string branchName, string content, string pushURL, string commitComment);
    }
}
