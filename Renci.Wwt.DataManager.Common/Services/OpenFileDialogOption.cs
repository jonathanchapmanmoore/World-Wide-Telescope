using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.DataManager.Common.Services
{
    public class OpenFileDialogOption : FileDialogOption
    {
        /// <summary>
        /// Gets or sets an option indicating whether OpenFileDialog allows users to select multiple files.
        /// </summary>
        /// <value>
        ///   <c>true</c> if multiple selections are allowed; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool Multiselect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the read-only check box displayed by OpenFileDialog is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the checkbox is selected; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool ReadOnlyChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether OpenFileDialog contains a read-only check box.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the checkbox is displayed; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool ShowReadOnly { get; set; }

    }
}
