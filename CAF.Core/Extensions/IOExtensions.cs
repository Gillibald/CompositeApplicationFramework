#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    /// <summary>
    ///     IO Extensions
    /// </summary>
    public static class IoExtensions
    {
        /// <summary>
        ///     Receives a string and a file name as parameters and writes the contents of the
        ///     string to that file
        ///     <pre>
        ///         Example:
        ///         string lcString = "This is the line we want to insert in our file.";
        ///         .StrToFile(lcString, "c:\\My Folders\\MyFile.txt");
        ///     </pre>
        /// </summary>
        /// <param name="content"> </param>
        /// <param name="fileName"> </param>
        public static void StringToFile(this string fileName, string content)
        {
            //Check if the sepcified file exists
            if (File.Exists(fileName))
            {
                //If so then Erase the file first as in this case we are overwriting
                File.Delete(fileName);
            }

            //Create the file if it does not exist and open it
            var oFs = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite);

            //Create a writer for the file
            var oWriter = new StreamWriter(oFs);

            //Write the contents
            oWriter.Write(content);
            oWriter.Flush();
            oWriter.Close();

            oFs.Close();
        }

        /// <summary>
        ///     Receives a string and a file name as parameters and writes the contents of the
        ///     string to that file. Receives an additional parameter specifying whether the
        ///     contents should be appended at the end of the file
        ///     <pre>
        ///         Example:
        ///         string lcString = "This is the line we want to insert in our file.";
        ///         .StrToFile(lcString, "c:\\My Folders\\MyFile.txt");
        ///     </pre>
        /// </summary>
        /// <param name="content"> </param>
        /// <param name="fileName"> </param>
        /// <param name="appendToContent"> </param>
        public static void StringToFile(this string fileName, string content, bool appendToContent)
        {
            //Create the file if it does not exist and open it
            var oFs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            //Create a writer for the file
            var oWriter = new StreamWriter(oFs);

            //Set the pointer to the end of file
            oWriter.BaseStream.Seek(0, SeekOrigin.End);

            //Write the contents
            oWriter.Write(content);
            oWriter.Flush();
            oWriter.Close();
            oFs.Close();
        }

        /// <summary>
        ///     Get path segment from path statement
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetPathList(this string path)
        {
            var pathDelimited = Environment.GetEnvironmentVariable("Path");
            if (pathDelimited != null)
            {
                var pathList = pathDelimited.Split(';').ToList();

                var result =
                    pathList.Where(p => p.ToString(CultureInfo.InvariantCulture).ToLower().Contains(path.ToLower()));
                return result.ToList();
            }

            return new List<string>();
        }

        /// <summary>
        ///     If filename is in path then the path is returned
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFilePathFromEnvironment(this string filename)
        {
            string returnFilePath = null;
            var pathDelimited = Environment.GetEnvironmentVariable("Path");
            if (pathDelimited != null)
            {
                var pathList = pathDelimited.Split(';').ToList();

                foreach (var path in pathList)
                {
                    var slash = path.EndsWith("\\") ? "" : "\\";
                    var filePath = $"{path}{slash}{filename}";
                    if (File.Exists(filePath))
                    {
                        new FileInfo(filePath);
                        returnFilePath = filePath;
                    }
                }
            }
            return returnFilePath;
        }
    }
}