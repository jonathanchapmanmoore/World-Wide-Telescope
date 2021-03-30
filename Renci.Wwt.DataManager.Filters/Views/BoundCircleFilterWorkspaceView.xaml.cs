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
using Renci.Wwt.DataManager.Filters.ViewModels;
using Renci.Wwt.DataManager.Common.Framework;
using Renci.Wwt.DataManager.Common.ViewModels;

namespace Renci.Wwt.DataManager.Filters.Views
{
    /// <summary>
    /// Interaction logic for BoundCircleFilterWorkspaceView.xaml
    /// </summary>
    public partial class BoundCircleFilterWorkspaceView : UserControl
    {
        public BoundCircleFilterWorkspaceView(FilterViewModel viewModel)
        {
            InitializeComponent();

            ViewModelBinder.Bind(viewModel, this);
        }
    }
}
