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
using Renci.Wwt.DataManager.Sos.ViewModels;

namespace Renci.Wwt.DataManager.Sos.Views
{
    /// <summary>
    /// Interaction logic for AddSosRibbonMenuItem.xaml
    /// </summary>
    public partial class AddSosRibbonMenuItemView : RibbonMenuItem
    {
        public AddSosRibbonMenuItemView(AddSosRibbonMenuItemViewModel model)
        {
            InitializeComponent();

            this.DataContext = model;
        }
    }
}
