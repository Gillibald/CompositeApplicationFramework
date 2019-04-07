#region Usings

using System.IO;

#endregion

namespace CompositeApplicationFramework.Helper
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     Class FileHelper
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        ///     Saves the text to file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="content">The content.</param>
        public static void SaveTextToFile(string fileName, string content)
        {
            File.WriteAllText(fileName, content);
        }

        public static string GetAppDataFolder()
        {
            var lAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var assemblyName = Assembly.GetEntryAssembly();

            var lFolder = Path.Combine(lAppDataFolder, $"{assemblyName.GetName().Name}\\");

            if (!Directory.Exists(lFolder))
            {
                Directory.CreateDirectory(lFolder);
            }

            return lFolder;
        }
    }
}