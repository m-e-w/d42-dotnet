using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace D42_POC_Demo
{
    public class Wizard_Helper : Wizard_Helper_Interface
    {
        public void AddFileContentToList(string fileName, List<KeyValuePair<string, string>> listOfKvp)
        {
            string content = "";
            try
            {
                MessageBox.Show(Path.GetFullPath($"{fileName}.sql"));
                content = File.ReadAllText(Path.GetFullPath($"{fileName}.sql"));
                List<string> keys = (from kvp in listOfKvp select kvp.Key).Distinct().ToList();

                if (!keys.Contains(Path.GetFileNameWithoutExtension(fileName)))
                {
                    listOfKvp.Add(new KeyValuePair<string, string>(Path.GetFileNameWithoutExtension(fileName), content));
                }
                else
                {
                    var kvp = listOfKvp.FirstOrDefault(x => x.Key == Path.GetFileNameWithoutExtension(fileName));
                    listOfKvp.Remove(kvp);
                    listOfKvp.Add(new KeyValuePair<string, string>(Path.GetFileNameWithoutExtension(fileName), content));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ExtractFilesToDirectory(string fileName, string directory, string fileExtension, string suffixToAdd)
        {
            string zipPath = Path.GetFullPath(fileName);
            string extractPath = Path.GetFullPath(directory);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Normalizes the path.
            extractPath = Path.GetFullPath(extractPath);

            // Ensures that the last character on the extraction path
            // is the directory separator char.
            // Without this, a malicious zip file could try to traverse outside of the expected
            // extraction path.
            if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                extractPath += Path.DirectorySeparatorChar;

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string newName = $"{entry.Name.Substring(0, entry.Name.Length - 4)}{suffixToAdd}{fileExtension}";
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, newName));
                        //MessageBox.Show(destinationPath);

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                            entry.ExtractToFile(destinationPath, true);
                    }
                }
            }
            //Delete the zip file after extracting
            File.Delete(zipPath);
        }

        public void AddDirectoryFileContentToList(string directory, List<KeyValuePair<String, String>> listOfKvp)
        {
            string pathName = Path.GetFullPath(directory);
            foreach (string fileName in Directory.GetFiles(pathName))
            {
                string content = "";
                try
                {
                    content = File.ReadAllText(fileName);
                    List<string> keys = (from kvp in listOfKvp select kvp.Key).Distinct().ToList();

                    if (!keys.Contains(Path.GetFileNameWithoutExtension(fileName)))
                    {
                        listOfKvp.Add(new KeyValuePair<string, string>(Path.GetFileNameWithoutExtension(fileName), content));
                    }
                    else
                    {
                        var kvp = listOfKvp.FirstOrDefault(x => x.Key == Path.GetFileNameWithoutExtension(fileName));
                        listOfKvp.Remove(kvp);
                        listOfKvp.Add(new KeyValuePair<string, string>(Path.GetFileNameWithoutExtension(fileName), content));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void SaveTextToCSVFile(string text, string fileName, string directory)
        {
            string name = $"{fileName}-{DateTime.Now}".Replace(" ", "-").Replace("/", "-").Replace(":", "-");
            SaveTextToFIle(text, name, directory, ".csv");
        }

        public void SaveTextToFIle(string text, string fileName, string  directory, string fileExtension)
        {
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(directory))
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(Path.GetFullPath(Path.Combine(Path.GetFullPath(directory), $"{fileName}{fileExtension}")), text);
            }
        }

        public string ToUnsecureString(SecureString source)
        {
            var returnValue = IntPtr.Zero;
            try
            {
                returnValue = Marshal.SecureStringToGlobalAllocUnicode(source);
                return Marshal.PtrToStringUni(returnValue);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(returnValue);
            }
        }
    }
}
