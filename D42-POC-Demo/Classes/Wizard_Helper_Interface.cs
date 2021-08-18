using System;
using System.Collections.Generic;
using System.Security;

namespace D42_POC_Demo
{
    public interface Wizard_Helper_Interface
    {
        void SaveTextToCSVFile(string text, string fileName, string directory);
        void SaveTextToFIle(string text, string fileName, string directory, string fileExtension);
        void AddFileContentToList(string fileName, List<KeyValuePair<String, String>> listOfKvp);
        void AddDirectoryFileContentToList(string directory, List<KeyValuePair<String, String>> listOfKvp);
        void ExtractFilesToDirectory(string fileName, string directory, string fileExtension, string suffixToAdd);
        String ToUnsecureString(SecureString source);
    }
}
