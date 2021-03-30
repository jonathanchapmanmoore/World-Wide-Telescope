using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Controls.PropertyGrid.Editors;

namespace Microsoft.Windows.Controls.PropertyGrid
{
    public class PropertyEditorAttribute : Attribute
    {
        public Type EditorType { get; set; }

        public PropertyEditorAttribute(Type editorType)
        {
            this.EditorType = editorType;
        }
    }
}
