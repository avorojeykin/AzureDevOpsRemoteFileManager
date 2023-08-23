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

public string RunAzureDevOpsRemoteFileManager()
        {           
            // Creates new Feature Branch off of Specified Source Branch
            _httpClient.createBranch(_configuration["sourceBranchName"], _configuration["featureBranchName"]);

            _httpClient.getListOfIdsOfRepoFilesByRepo(_configuration["repo"]); //Get List of All File Ids in Bg.Next
            string _editFileAlreadyExists = "";
            string _customFactoryFileId = _httpClient.getIdOfSpecificRepoFile(_configuration["customFactoryFileRepoPath"]);// Get CustomFactory File Id based on repo path
            string _customTypesFileId = _httpClient.getIdOfSpecificRepoFile(_configuration["customTypesFileRepoPath"]); // Get CustomTypes File Id based on repo path
            string _customEditsFileId = _httpClient.getIdOfSpecificRepoFile(_configuration["customEditsFileRepoPath"]); // Get CustomEdits File Id based on repo path  
            try
            {
                _editFileAlreadyExists = _httpClient.getIdOfSpecificRepoFile(_configuration["customFilePushURL"]); // Get Repo File URL of Custom File for Editing
            }
            catch (Exception ex)
            {
                _editFileAlreadyExists = "";
            }

            string customFactoryContent = _httpClient.getContentOfSpecificRepoFilesById(_customFactoryFileId); // Get Contents of Custom Factory File

            string customTypesContent = _httpClient.getContentOfSpecificRepoFilesById(_customTypesFileId); // Get Contents of Custom Types File

            string customEditsContent = _httpClient.getContentOfSpecificRepoFilesById(_customEditsFileId); // Get Contents of Custom Edits File

            // Edit Files & Commit and Push Changes           
            string newCustomFactoryFileContent = _fileEditor.editCustomFactoryFile(customFactoryContent, _configuration["codeToInsert"]); // Returns Custom Factory File as string with insertion of Info at marker points
            if (!newCustomFactoryFileContent.Equals(customFactoryContent))
            {
                _httpClient.pushEditBranchChanges(_configuration["featureBranchName"], newCustomFactoryFileContent, _configuration["customFactoryRepoPath"], $"Added CustomFactory Changes");
            }

            string newCustomTypesFileContent = _fileEditor.editCustomTypesFile(customTypesContent, _configuration["codeToInsert"]); // Returns Custom Types File as string with insertion of Info at marker points
            if (!newCustomTypesFileContent.Equals(customTypesContent))
            {
                _httpClient.pushEditBranchChanges(_configuration["featureBranchName"], newCustomTypesFileContent, _configuration["customTypesRepoPath"], $"Added/Updated CustomTypes Changes");
            }

            string newCustomEditsFileContent = _fileEditor.editCustomEditsFile(customEditsContent, _configuration["codeToInsert"]); // Returns Custom Edits File as string with insertion of Info at marker points
            if (!newCustomEditsFileContent.Equals(customEditsContent))
            {
                _httpClient.pushEditBranchChanges(_configuration["featureBranchName"], newCustomEditsFileContent, _configuration["customEditsRepoPath"], $"Added/Updated CustomEdits Changes");
            }

            if (!_editFileAlreadyExists.Equals(""))
            {
                return _httpClient.pushEditFileBranchChanges(_configuration["featureBranchName"], _configuration["editSourceCode"], _configuration["editPushURL"], $"Made Changes To File {_configuration["fileName"]}");
            }
            else
            {
                return _httpClient.pushAddFileBranchChanges(_configuration["featureBranchName"], _configuration["editSourceCode"], _configuration["editPushURL"], $"Added New File {_configuration["fileName"]}"); //Pushes new file with Code provided inside Appsettings
            }
        }
    }
