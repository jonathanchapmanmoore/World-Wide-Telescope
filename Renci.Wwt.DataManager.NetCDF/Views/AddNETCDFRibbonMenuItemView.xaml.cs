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
using Microsoft.Windows.Controls.Ribbon;
using Renci.Wwt.DataManager.NetCDF.ViewModels;

namespace Renci.Wwt.DataManager.NetCDF.Views
{
    /// <summary>
    /// Interaction logic for AddNETCDFRibbonMenuItem.xaml
    /// </summary>
    public partial class AddNETCDFRibbonMenuItemView : RibbonMenuItem
    {
        public AddNETCDFRibbonMenuItemView(AddNETCDFRibbonMenuItemViewModel model)
        {
            InitializeComponent();

            this.DataContext = model;
        }
    }
}
