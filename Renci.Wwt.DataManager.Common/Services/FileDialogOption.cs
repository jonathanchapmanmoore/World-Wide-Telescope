using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.DataManager.Common.Services
{
    public abstract class FileDialogOption
    {
        /// <summary>
        /// Gets or sets a value indicating whether a file dialog automatically adds an extension to a file name if the user omits an extension.
        /// </summary>
        /// <value>
        ///   <c>true</c> if extensions are added; otherwise, <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool AddExtension { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog displays a warning if the user specifies a file name that does not exist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default in this base class is <c>false</c>.
        /// </value>
        public virtual bool CheckFileExists { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether warnings are displayed if the user types invalid paths and file names.
        /// </summary>
        /// <value>
        ///   <c>true</c> if warnings are displayed; otherwise, <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool CheckPathExists { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies the default extension string to use to filter the list of files that are displayed.
        /// </summary>
        /// <value>
        /// The default extension string. The default is System.String.Empty.
        /// </value>
        public string DefaultExt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a file dialog returns either the location of the file referenced by a shortcut or the location of the shortcut file (.lnk).
        /// </summary>
        /// <value>
        ///   <c>true</c> to return the location referenced; <c>false</c> to return the shortcut location. The default is <c>false</c>.
        /// </value>
        public bool DereferenceLinks { get; set; }

        /// <summary>
        /// Gets or sets a string containing the full path of the file selected in a file dialog.
        /// </summary>
        /// <value>
        /// A System.String that is the full path of the file selected in the file dialog. The default is System.String.Empty.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the filter string that determines what types of files are displayed from either the Microsoft.Win32.OpenFileDialog or Microsoft.Win32.SaveFileDialog.
        /// </summary>
        /// <value>
        /// A System.String that contains the filter. The default is System.String.Empty, which means that no filter is applied and all file types are displayed.
        /// </value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the index of the filter currently selected in a file dialog.
        /// </summary>
        /// <value>
        /// The System.Int32 that is the index of the selected filter. The default is 1.
        /// </value>
        public int FilterIndex { get; set; }

        /// <summary>
        /// Gets or sets the initial directory that is displayed by a file dialog.
        /// </summary>
        /// <value>
        /// A System.String that contains the initial directory. The default is System.String.Empty.
        /// </value>
        public string InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the text that appears in the title bar of a file dialog.
        /// </summary>
        /// <value>
        /// A System.String that is the text that appears in the title bar of a file dialog. The default is System.String.Empty.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog accepts only valid Win32 file names.
        /// </summary>
        /// <value>
        ///   <c>true</c> if warnings will be shown when an invalid file name is provided; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool ValidateNames { get; set; }
    }
}
