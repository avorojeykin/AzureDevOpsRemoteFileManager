# AzureDevOpsRemoteFileManager
This C# class library is designed to streamline your development workflow by enabling you to edit files directly within Azure DevOps repositories, eliminating the need to pull files locally.

## Key Features

- **Remote Editing:** Edit files within Azure DevOps repositories without the overhead of local file syncing.

- **Effortless Integration:** Easily incorporate the library into your C# projects for seamless remote file management.

- **Secure Operations:** Ensure secure authentication and communication with Azure DevOps for sensitive code and data.

- **Version Control Compatibility:** Maintain version control integrity as the library interacts seamlessly with Azure DevOps' version control systems.

- **Automation Enhancement:** Facilitate automation with concurrent remote edits.

## Getting Started

The `AzureDevOpsRemoteFileManager` is currently a class library and has not been converted to being a Nuget:

1)  You can either convert it to a Nuget and package it to be used within your Feed or use it directly as a class library
2)  You will need to replace everywhere where it says "PlaceHolder" with your Azure DevOps organization name
3)  The FileEditor methods will need to be adjusted based on your requierments of what you want to change within the files. These methods act as marker points for code insertion.

Here is an example of the Class Library Being Integrated

        public void RunAzureDevOpsRemoteFileManager()
        {           
            // Creates new Feature Branch off of Specified Source Branch
            _httpClient.CreateBranch(_configuration["sourceBranchName"], _configuration["featureBranchName"]);

            _httpClient.GetListOfIdsOfRepoFilesByRepo(_configuration["repoName"]); //Get List of All File Ids in Specified Repo
            string _customFileAlreadyExists = "";                    
            try
            {
                _customFileAlreadyExists = _httpClient.GetIdOfSpecificRepoFile(_configuration["customFileRepoPath"]); // Get Repo File URL of Custom File for Editing
            }
            catch (Exception ex)
            {
                _customFileAlreadyExists = "";
            }

            string customFileContent = _httpClient.GetContentOfSpecificRepoFilesById(_customFileAlreadyExists); // Get Contents of Custom Factory File            

            // Edit Files & Commit and Push Changes
            if (!_customFileAlreadyExists.Equals(""))
            {
                string newCustomFileContent = _fileEditor.EditCustomFile(customFileContent, _configuration["codeToInsert"]); // Returns Custom File as string with insertion of Info at marker points based on internal editCustomFile Method
                if (!newCustomFileContent.Equals(customFileContent))
                {
                    _httpClient.PushEditFileBranchChanges(_configuration["featureBranchName"], newCustomFileContent, _configuration["customFileRepoPath"], $"Added CustomFactory Changes");
                }
            }
            else
            {
               _httpClient.PushAddFileBranchChanges(_configuration["featureBranchName"], _configuration["newFileSourceCode"], _configuration["newFilePushURL"], $"Added New File {_configuration["fileName"]}"); //Pushes new file with Code provided inside Appsettings
            }           
        }
