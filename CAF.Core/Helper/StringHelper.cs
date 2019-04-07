#region Usings

using System;

#endregion

namespace CompositeApplicationFramework.Helper
{
    /// <summary>
    ///     Class StringHelper
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        ///     Gets the search pattern from file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public static string GetSearchPatternFromFilePath(string filePath)
        {
            // Get the offset of the last \ so that we can
            // get the dir name and search pattern, e.g.,
            // c:\myfolder\myfiles\*.xml
            var offset = filePath.LastIndexOf(@"\", StringComparison.Ordinal);

            // dir = c:\myfolder\myfiles

            // searchPattern = *.xml
            var searchPattern = filePath.Substring(offset + 1);

            return searchPattern;
        }

        /// <summary>
        ///     Gets the directory from file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        public static string GetDirectoryFromFilePath(string filePath)
        {
            // Get the offset of the last \ so that we can
            // get the dir name and search pattern, e.g.,
            // c:\myfolder\myfiles\*.xml
            var offset = filePath.LastIndexOf(@"\", StringComparison.Ordinal);

            if (offset < 1)
            {
                return null;
            }

            // dir = c:\myfolder\myfiles
            var dir = filePath.Substring(0, offset);

            return dir;
        }
    }
}