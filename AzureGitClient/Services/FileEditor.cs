using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGitClient.Services
{
    public sealed class FileEditor : IFileEditor
    {
        public string editCustomTypesFile(string originalFile, string editNumberToBeInserted)
        {
            int j = originalFile.IndexOf($"E{editNumberToBeInserted}");
            string outputCode = "";
            if (j != -1)
            {
                outputCode = originalFile;
            }
            else
            {
                string vetRegionMarker = "#region VetEdits";
                int searchTextLength = vetRegionMarker.Length;
                int x = originalFile.IndexOf(vetRegionMarker);

                if (x != -1)
                {
                    outputCode = originalFile.Insert(x + searchTextLength, $"\n\t\tE{editNumberToBeInserted},");
                }
            }
            return outputCode;

        }

        public string editCustomEditsFile(string originalFile, string editNumberToBeInserted)
        {
            int j = originalFile.IndexOf($"EditorTypes.E{editNumberToBeInserted}");
            string outputCode = "";
            if (j != -1)
            {
                outputCode = originalFile;
            }
            else
            {              
                string vetRegionMarker = "#region VetEdits";
                int searchTextLength = vetRegionMarker.Length;
                int x = originalFile.IndexOf(vetRegionMarker);
                if (x != -1)
                {
                    outputCode = originalFile.Insert(x + searchTextLength, $"\n\t\t\tEditorTypes.E{editNumberToBeInserted},");
                }
            }           
            return outputCode;
        }

        public string editCustomFactoryFile(string originalFile, string editNumberToBeInserted)
        {
            int j = originalFile.IndexOf($"EditorTypes.E{editNumberToBeInserted}");
            string outputCode = "";
            if (j != -1)
            {
                outputCode = originalFile;
            }
            else
            {
                string editorTypeMarker = "// EditorTypes";
                string editorBaseMarker = "// EditorBase GetEdit";
                string editorMetaDataMarker = "// EditorMetaData";
                int searchTextLength = editorTypeMarker.Length;
                int x = originalFile.IndexOf(editorTypeMarker);

                if (x != -1)
                {
                    outputCode = originalFile.Insert(x + searchTextLength, $"\n\t\tEditorTypes.E{editNumberToBeInserted},");
                }

                searchTextLength = editorBaseMarker.Length;
                x = outputCode.IndexOf(editorBaseMarker);
                if (x != -1)
                {
                    outputCode = outputCode.Insert(x + searchTextLength, $"\n\t\tcase EditorTypes.E{editNumberToBeInserted}:\n\t\t  "
                        + $"return Edit_{editNumberToBeInserted}.Factory.Create(claimDto);");
                }

                searchTextLength = editorMetaDataMarker.Length;
                x = outputCode.IndexOf(editorMetaDataMarker);
                if (x != -1)
                {
                    outputCode = outputCode.Insert(x + searchTextLength, $"\n\t\tcase EditorTypes.E{editNumberToBeInserted}:\n\t\t  "
                        + $"return Edit_{editNumberToBeInserted}.Factory.EditMetaData();");
                }
            }           
            return outputCode;
        }
    }
}
