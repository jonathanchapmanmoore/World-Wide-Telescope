// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

extern alias Silverlight;
using System.ComponentModel;
using System.Windows.Controls.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using SSWC = Silverlight::System.Windows.Controls;

namespace System.Windows.Controls.Input.Design
{
    /// <summary>
    /// To register design time metadata for ButtonSpinner.
    /// </summary>
    internal class ButtonSpinnerMetadata : AttributeTableBuilder
    {
        /// <summary>
        /// To register design time metadata for ButtonSpinner.
        /// </summary>
        public ButtonSpinnerMetadata()
            : base()
        {
            AddCallback(
                typeof(SSWC.ButtonSpinner),
                b =>
                {
                    b.AddCustomAttributes(
                        Extensions.GetMemberName<SSWC.ButtonSpinner>(x => x.Content),
                        new CategoryAttribute(Properties.Resources.CommonProperties));

                    b.AddCustomAttributes(new DefaultBindingPropertyAttribute(
                        Extensions.GetMemberName<SSWC.ButtonSpinner>(x => x.Content)));

#if MWD40
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.BasicControls, false));
#endif
                });
        }
    }
}