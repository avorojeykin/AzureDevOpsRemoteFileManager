using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureGitClient.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using AzureGitClient.AzureDevOpsApiClient;
using AzureGitClient.Common;
using Microsoft.Extensions.Logging;

namespace AzureGitClient.Services
{
    public sealed class AzureDevOpsHttpClient : IAzureDevOpsHttpClient
    {
        private readonly IConfiguration _configuration;
        private static SHA1ItemIdList _sha1Values = new SHA1ItemIdList();
        
        public AzureDevOpsHttpClient (IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public bool BranchExists(GitHttpClient gitClient, GitRepository repo, string branchName)
        {
            bool result = true;
            try
            {
                var branch = gitClient.GetBranchAsync(repo.ProjectReference.Id, repo.Id, branchName).Result;
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException != null && ae.InnerException.Message == $"Branch \"{branchName}\" does not exist in the {repo.Id} repository.")
                {
                    result = false;
                }
                else
                {
                    throw ae;
                }
            }
            return result;
        }

        public bool CreateBranch(string baseBranchName, string featureBranchName)
        {
            bool result = false;
            VssBasicCredential credentials = new VssBasicCredential(string.Empty, _configuration["PAT"]);
            VssConnection connection = new VssConnection(new Uri(string.Format("https://dev.azure.com/{0}", "PlaceHolder")), credentials);

            using (GitHttpClient gitClient = connection.GetClient<GitHttpClient>())
            {
                try
                {
                    GitRepository repoTest = gitClient.GetRepositoryAsync("PlaceHolder", _configuration["repo"]).Result;
                }
                catch (AggregateException ae)
                {                  
                    throw new Exception("The PAT Credentials entered are not authorized or the Repo provided does not Exist");
                }
                GitRepository repo = gitClient.GetRepositoryAsync("PlaceHolder", _configuration["repo"]).Result;
                GitBranchStats? baseBranch = null;
                try
                {
                    baseBranch = gitClient.GetBranchAsync(repo.ProjectReference.Id, repo.Id, baseBranchName).Result;
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException.Message.Equals($"Branch \"{baseBranchName}\" does not exist in the {repo.Id} repository."))
                    {
                        throw new Exception("Source Branch: " + baseBranchName + " does not exist");
                    }
                    else if (baseBranch == null)
                    {
                        throw new Exception("Source branch can not be null");
                    }
                    else
                    {
                        throw ae;
                    }
                }

                if (baseBranch != null)
                {
                    bool exists = BranchExists(gitClient, repo, featureBranchName);
                    if (!exists)
                    {
                        try
                        {
                            var gitRef = gitClient.UpdateRefsAsync(new GitRefUpdate[] {new GitRefUpdate
                            {
                                Name = $"refs/heads/{featureBranchName}",
                                NewObjectId = baseBranch.Commit.CommitId,
                                OldObjectId = new string('0', 40),
                                IsLocked = false,
                            }}, repositoryId: repo.Id).Result;

                            result = BranchExists(gitClient, repo, featureBranchName);
                        }
                        catch (AggregateException)
                        {
                            throw new Exception("PAT Token has access restrictions");
                        }                      
                    }
                    else
                    {                        
                        throw new Exception("Branch with name:" + featureBranchName + " already Exists");
                    }
                }
            }
            return result;
        }

        public string GetContentOfSpecificRepoFilesById(string sha1Id)
        {
            AzureDevOpsItemSelector client = new AzureDevOpsItemSelector(_configuration["PAT"]);
            return client.GetIndividualRepoFiles(sha1Id, _configuration["repo"]);
        }

        public string GetIdOfSpecificRepoFile(string filePath, List<SHA1ItemIdValue> items)
        {
            string fileId = null;
            string fileNotFound = "No File Was Found With Associated File Path:";            
            if (items.Count > 0)
            {
                foreach (SHA1ItemIdValue item in items)
                {
                    if (item.Path.Equals(filePath))
                    {
                        fileId = item.ObjectId;
                        Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Found File with Associated File Path " + filePath);
                    }
                }
                if (fileId != null)
                {
                    return fileId;
                }
                throw new Exception (fileNotFound + " " + filePath);
            }
            else
            {
                throw new Exception("List of SHA1 Ids cannot be empty");                
            }
        }

        public List<SHA1ItemIdValue> GetListOfIdsOfRepoFilesByRepo(string repoName)
        {
            AzureDevOpsItemSelector devOpsClient = new AzureDevOpsItemSelector(_configuration["PAT"]);

            devOpsClient.GetRepoFilesSHA1(repoName);
            if (devOpsClient.Response.IsSuccess)
            {
                 _sha1Values =
                    CommonUtilities.DeserializeJsontoObject<SHA1ItemIdList>(devOpsClient.Response.ReturnedResponseString);
                Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Succesfully retrived list of Item Ids in Repo: " + repoName);

                return _sha1Values.Value.Where(x => x.ObjectId != null).ToList();
            }
            else
            {
                return new List<SHA1ItemIdValue>();
                throw new Exception(" Could not get list of Item Ids for Repo: " + repoName);
            }
        }

        public string PushAddFileBranchChanges(string branchName, string content, string pushURL, string commitComment)
        {
            string result = "";
            VssBasicCredential credentials = new VssBasicCredential(string.Empty, _configuration["PAT"]);
            VssConnection connection = new VssConnection(new Uri(string.Format("https://dev.azure.com/{0}", "PlaceHolder")), credentials);
            using (GitHttpClient gitClient = connection.GetClient<GitHttpClient>())
            {
                GitRepository repo = gitClient.GetRepositoryAsync("PlaceHolder", _configuration["repo"]).Result;
                GitBranchStats? baseBranch = null;
                try
                {
                    baseBranch = gitClient.GetBranchAsync(repo.ProjectReference.Id, repo.Id, branchName).Result;
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException == null || ae.InnerException.Message != $"Branch \"{branchName}\" does not exist in the {repo.Id} repository.")
                    {
                        throw ae;
                    }
                }
                string OldObjectId = baseBranch.Commit.CommitId;
                var gitRef = new GitRefUpdate
                {
                    Name = $"refs/heads/{branchName}",
                    OldObjectId = baseBranch.Commit.CommitId,
                    IsLocked = false,
                };

                GitCommitRef newCommit = new GitCommitRef
                {
                    Comment = commitComment,
                    Changes = new GitChange[]
                    {
                        new GitChange
                        {
                            ChangeType = VersionControlChangeType.Add,
                            Item = new GitItem { Path = pushURL },
                            NewContent = new ItemContent
                                {
                                    Content = Base64Encode(content),
                                    ContentType = ItemContentType.Base64Encoded,
                                },
                        }
                    }
                };

                GitPush push = gitClient.CreatePushAsync(new GitPush
                {
                    RefUpdates = new GitRefUpdate[] { gitRef },
                    Commits = new GitCommitRef[] { newCommit },
                }, repo.Id).Result;
                result = push.Url;
            }
            return result;
        }

        public string PushEditFileBranchChanges(string branchName, string content, string pushURL, string commitComment)
        {
            string result = "";
            VssBasicCredential credentials = new VssBasicCredential(string.Empty, _configuration["PAT"]);
            VssConnection connection = new VssConnection(new Uri(string.Format("https://dev.azure.com/{0}", "PlaceHolder")), credentials);
            using (GitHttpClient gitClient = connection.GetClient<GitHttpClient>())
            {
                GitRepository repo = gitClient.GetRepositoryAsync("PlaceHolder", _configuration["repo"]).Result;
                GitBranchStats? baseBranch = null;
                try
                {
                    baseBranch = gitClient.GetBranchAsync(repo.ProjectReference.Id, repo.Id, branchName).Result;
                }
                catch (AggregateException ae)
                {
                    if (ae.InnerException == null || ae.InnerException.Message != $"Branch \"{branchName}\" does not exist in the {repo.Id} repository.")
                    {
                        throw ae;
                    }
                }
                string OldObjectId = baseBranch.Commit.CommitId;
                var gitRef = new GitRefUpdate
                {
                    Name = $"refs/heads/{branchName}",
                    OldObjectId = baseBranch.Commit.CommitId,
                    IsLocked = false,
                };

                GitCommitRef newCommit = new GitCommitRef
                {
                    Comment = commitComment,
                    Changes = new GitChange[]
                    {
                        new GitChange
                        {
                            ChangeType = VersionControlChangeType.Edit,
                            Item = new GitItem { Path = pushURL },
                            NewContent = new ItemContent
                                {
                                    Content = Base64Encode(content),
                                    ContentType = ItemContentType.Base64Encoded,
                                },
                        }
                    }
                };

                GitPush push = gitClient.CreatePushAsync(new GitPush
                {
                    RefUpdates = new GitRefUpdate[] { gitRef },
                    Commits = new GitCommitRef[] { newCommit },
                }, repo.Id).Result;
                result = push.Url;
            }
            return result;
        }
    }
}
