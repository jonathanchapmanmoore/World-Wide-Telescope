using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.DataManager.Common.Services
{
    public class FileDialogResult
    {
        /// <summary>
        /// Gets or sets a string containing the full path of the file selected in a file dialog.
        /// </summary>
        /// <value>
        /// A System.String that is the full path of the file selected in the file dialog. The default is System.String.Empty.
        /// </value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets an array that contains one file name for each selected file.
        /// </summary>
        /// <value>
        /// An array of System.String that contains one file name for each selected file. The default is an array with a single item whose value is System.String.Empty.
        /// </value>
        public string[] FileNames { get; private set; }

        /// <summary>
        /// Gets a string that only contains the file name for the selected file.
        /// </summary>
        /// <value>
        /// A System.String that only contains the file name for the selected file. The default is System.String.Empty, which is also the value when either no file is selected or a directory is selected.
        /// </value>
        public string SafeFileName { get; private set; }

        /// <summary>
        /// Gets an array that contains one safe file name for each selected file.
        /// </summary>
        /// <value>
        /// An array of System.String that contains one safe file name for each selected file. The default is an array with a single item whose value is System.String.Empty.
        /// </value>
        public string[] SafeFileNames { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDialogResult"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileNames">The file names.</param>
        /// <param name="safeFileName">Name of the safe file.</param>
        /// <param name="safeFileNames">The safe file names.</param>
        public FileDialogResult(string fileName, string[] fileNames, string safeFileName, string[] safeFileNames)
        {
            this.FileName = fileName;
            this.FileNames = fileNames;
            this.FileName = safeFileName;
            this.SafeFileNames = safeFileNames;
        }
    }
}
