// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;

namespace System.Windows.Controls.Testing
{
    /// <summary>
    /// Interface used to test virtual members of Expander.
    /// </summary>
    public interface IOverriddenExpander : IOverriddenHeaderedContentControl
    {
        /// <summary>
        /// Gets the OnExpanded test actions.
        /// </summary>
        OverriddenMethod ExpandedActions { get; }

        /// <summary>
        /// Gets the OnCollapsed test actions.
        /// </summary>
        OverriddenMethod CollapsedActions { get; }
    }
}