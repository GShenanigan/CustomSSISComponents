using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace PicnicerrorDotNet.SSIS.Enumerators
{
    [DtsForEachEnumerator(
        DisplayName = "For Each File Name Enumerator",
        Description = "Custom for each file enumerator that enumerates file names only.",
        ForEachEnumeratorContact = "http://picnicerror.net",
        UITypeName = "PicnicerrorDotNet.SSIS.Enumerators.ForEachFileNameEnumeratorUI,PicnicerrorDotNet.SSIS.Enumerators.FileNameUI,Version=1.0.0.0,Culture=Neutral,PublicKeyToken=283ba3efea3effbb")]
    public class ForEachFileNameEnumerator : ForEachEnumerator
    {
        #region "Dependencies"
        /// <summary>
        /// Which portion of the file name to retrieve
        /// </summary>
        public enum FilePathType
        {
            NameAndExtension,
            FullyQualified,
            NameOnly
        }
        #endregion

        #region "Properties"
        /// <summary>
        /// The directory which contains the files
        /// </summary>
        public string SourceDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// File mask for filtering the directory contents I.E. "*.txt"
        /// </summary>
        public string FileSpec
        {
            get;
            set;
        }

        /// <summary>
        /// Flag indicating whether to enumerate files in subfolders
        /// </summary>
        public bool TraverseSubfolders
        {
            get;
            set;
        }

        /// <summary>
        /// Portion of the filename to be retrieved
        /// </summary>
        public FilePathType FilePath
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Override the GetEnumerator method to implement your own custom enumeration logic
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="variableDispenser"></param>
        /// <param name="events"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public override object GetEnumerator(Connections connections, Microsoft.SqlServer.Dts.Runtime.VariableDispenser variableDispenser, IDTSInfoEvents events, IDTSLogging log)
        {
            //Get the directory, use getenumerator and return list of strings
            List<string> activeFiles = new List<string>();

            System.Threading.Tasks.Task listTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var dir = new DirectoryInfo(SourceDirectory);
                IEnumerable<FileInfo> files = null;

                if (TraverseSubfolders)
                    files = dir.EnumerateFiles(FileSpec, SearchOption.AllDirectories);
                else
                    files = dir.EnumerateFiles(FileSpec, SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    //Grab each relevant level of the file path
                    string fileExtension = file.Extension;
                    string filePath = file.FullName;
                    string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                    string fileNameOnly = fileName.Substring(0, fileName.IndexOf(fileExtension));

                    //Check how to retrieve the filename
                    string outputFileName = filePath;
                    switch (FilePath)
                    {
                        case FilePathType.FullyQualified:
                            outputFileName = filePath;
                            break;
                        case FilePathType.NameAndExtension:
                            outputFileName = fileName;
                            break;
                        case FilePathType.NameOnly:
                            outputFileName = fileNameOnly;
                            break;
                    }

                    activeFiles.Add(outputFileName);
                }
            });

            //Wait for all threads to complete and return the collection
            System.Threading.Tasks.Task.WaitAll(new System.Threading.Tasks.Task[] { listTask });
            return activeFiles.GetEnumerator();
        }
    }
}
