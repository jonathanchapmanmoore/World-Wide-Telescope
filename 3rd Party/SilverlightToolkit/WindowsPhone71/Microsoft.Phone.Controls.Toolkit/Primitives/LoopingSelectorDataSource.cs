// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows.Controls;

namespace Microsoft.Phone.Controls.Primitives
{
    /// <summary>
    /// Defines how the LoopingSelector communicates with data source.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public interface ILoopingSelectorDataSource
    {
        /// <summary>
        /// Get the next datum, relative to an existing datum.
        /// </summary>
        /// <param name="relativeTo">The datum the return value will be relative to.</param>
        /// <returns>The next datum.</returns>
        object GetNext(object relativeTo);

        /// <summary>
        /// Get the previous datum, relative to an existing datum.
        /// </summary>
        /// <param name="relativeTo">The datum the return value will be relative to.</param>
        /// <returns>The previous datum.</returns>
        object GetPrevious(object relativeTo);

        /// <summary>
        /// The selected item. Should never be null.
        /// </summary>
        object SelectedItem { get; set; }

        /// <summary>
        /// Raised when the selection changes.
        /// </summary>
        event EventHandler<SelectionChangedEventArgs> SelectionChanged;
    }
}
