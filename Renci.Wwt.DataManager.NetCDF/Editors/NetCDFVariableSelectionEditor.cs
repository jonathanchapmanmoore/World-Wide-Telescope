using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Controls.PropertyGrid.Editors;
using System.Windows;
using Microsoft.Windows.Controls.PropertyGrid;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using Renci.Wwt.DataManager.NetCDF.Models;
using Renci.Wwt.DataManager.NetCDF.ViewModels;
using System.IO;
using Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI;

namespace Renci.Wwt.DataManager.NetCDF.Editors
{
    public class NetCDFVariableSelectionEditor : ITypeEditor
    {
        private ItemsControl _element;

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            this._element = new ComboBox();

            var dataSource = propertyItem.Instance as NetCDFDataSourceInfo;

            this.OnPathChanged(dataSource.Path);
            dataSource.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Path")
                {
                    this.OnPathChanged(((NetCDFDataSourceInfo)sender).Path);
                }
            };

            var binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnDataErrors = true;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this._element, ComboBox.SelectedValueProperty, binding);

            return this._element;
        }

        private void OnPathChanged(string path)
        {
            if (File.Exists(path))
            {
                var reader = NetCDFReader.Create(path);
               
                this._element.ItemsSource = from v in reader.Variables select v.Key;
            }
            else
            {
                this._element.ItemsSource = null;
            }
        }
    }
}
