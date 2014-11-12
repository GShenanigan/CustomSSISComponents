using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace PicnicerrorDotNet.SSIS.Enumerators
{
    [DtsForEachEnumerator(
        DisplayName = "For Each File Integer Expression Enumerator",
        Description="Custom for each file enumerator that enumerates files as the result of an integer comparison against the filename. Only for files which have an integer as a filename.",
        ForEachEnumeratorContact="http://picnicerror.net",
        UITypeName = "PicnicerrorDotNet.SSIS.Enumerators.ForEachFileIntegerExpressionEnumeratorUI,PicnicerrorDotNet.SSIS.Enumerators.FileIntegerExpressionUI,Version=1.0.0.0,Culture=Neutral,PublicKeyToken=7fef2b07d0ea7898")]
    public class ForEachFileIntegerExpressionEnumerator : ForEachEnumerator
    {
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
        /// Flag indicating whether to perform the integer comparison or not
        /// </summary>
        public bool UseComparison
        {
            get;
            set;
        }

        /// <summary>
        /// Definition of the available conditional operators for the comparison
        /// </summary>
        public enum ConditionalOperatorType
        {
            GreaterThan,
            GreaterThanOrEqualTo,
            Equals,
            LessThanOrEqualTo,
            LessThan
        }

        /// <summary>
        /// The conditional operator to use for the comparison
        /// </summary>
        public ConditionalOperatorType ConditionalOperator
        {
            get;
            set;
        }

        /// <summary>
        /// The value to be compared against the filename integer
        /// </summary>
        public int ComparisonValue
        {
            get;
            set;
        }

        /// <summary>
        /// Flag indicating whether to remove files which appear in the exclusion list
        /// </summary>
        public bool UseExclusionList
        {
            get;
            set;
        }

        /// <summary>
        /// List of integers (i.e. filenames) which should be excluded from the final list
        /// </summary>
        public string ExclusionListName
        {
            get;
            set;
        }

        /// <summary>
        /// Which portion of the file name to retrieve
        /// </summary>
        public enum FilePathType
        {
            NameAndExtension,
            FullyQualified,
            NameOnly
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
            //Get the directory, use getenumerator and return list of strings based on expression
            List<string> activeFiles = new List<string>();
            List<int> numbersToExclude = new List<int>();

            if (!String.IsNullOrEmpty(ExclusionListName))
            {
                //Retrieves the exclusion list variable
                Variables varList = null;
                variableDispenser.LockOneForRead(ExclusionListName, ref varList);
                numbersToExclude = (List<int>)varList[0].Value;
                varList.Unlock();
            }

            System.Threading.Tasks.Task listTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                DirectoryInfo dir = new DirectoryInfo(SourceDirectory);
                foreach (FileInfo f in dir.EnumerateFiles(FileSpec))
                {
                    FileInfo file = f;
                    
                    //Grab each relevant level of the file path
                    string fileExtension = file.Extension;
                    string filePath = file.FullName;
                    string fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                    string fileNameOnly = fileName.Substring(0, fileName.IndexOf(fileExtension));
                    int valueToCompare = Convert.ToInt32(fileNameOnly);

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

                    //Check whether we need to use the exclusion list, to further filter the results
                    if (UseExclusionList)
                    {
                        if (!numbersToExclude.Contains(valueToCompare))
                        {
                            if (UseComparison)
                            {
                                if (Match(valueToCompare))
                                    activeFiles.Add(outputFileName);
                            }
                            else
                                activeFiles.Add(outputFileName);
                        }
                    }
                    else
                    {
                        if (UseComparison)
                        {
                            if (Match(valueToCompare))
                                activeFiles.Add(outputFileName);
                        }
                        else
                            activeFiles.Add(outputFileName);
                    }
                }
            });

            System.Threading.Tasks.Task.WaitAll(new System.Threading.Tasks.Task[] { listTask });

            return activeFiles.GetEnumerator();
        }

        /// <summary>
        /// Performs the comparison operation depending on chosen operator
        /// </summary>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        private bool Match(int valueToCompare)
        {
            switch (ConditionalOperator)
            {
                case ConditionalOperatorType.GreaterThan:
                    if (valueToCompare > ComparisonValue)
                        return true;
                    break;
                case ConditionalOperatorType.GreaterThanOrEqualTo:
                    if (valueToCompare >= ComparisonValue)
                        return true;
                    break;
                case ConditionalOperatorType.Equals:
                    if (valueToCompare == ComparisonValue)
                        return true;
                    break;
                case ConditionalOperatorType.LessThanOrEqualTo:
                    if (valueToCompare <= ComparisonValue)
                        return true;
                    break;
                case ConditionalOperatorType.LessThan:
                    if (valueToCompare < ComparisonValue)
                        return true;
                    break;
                default:
                    return true;
            }

            return false;
        }
    }
}
