using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls.PropertyGrid.Editors;
using Microsoft.Win32;
using System.IO;

namespace Renci.Wwt.DataManager.Common.Editors
{
    /// <summary>
    /// Interaction logic for PathSelectionEditorControl.xaml
    /// </summary>
    public partial class PathSelectionEditor : UserControl, ITypeEditor
    {
        public PathSelectionEditor()
        {
            InitializeComponent();
        }

        public FrameworkElement ResolveEditor(Microsoft.Windows.Controls.PropertyGrid.PropertyItem propertyItem)
        {
            var binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnDataErrors = true;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this._pathName, TextBox.TextProperty, binding);

            return this;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files (*.*)|*.*";
            dialog.CheckFileExists = true;

            string filename = this._pathName.Text;
            if (File.Exists(filename))
            {
                dialog.FileName = filename;
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(filename);
            }

            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                this._pathName.Text = dialog.FileName;
                this._pathName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }
}
