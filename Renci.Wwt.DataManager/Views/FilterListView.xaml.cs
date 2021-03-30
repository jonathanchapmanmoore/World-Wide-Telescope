using System.Windows;
using Renci.Wwt.DataManager.ViewModels;
using Renci.Wwt.DataManager.Common.Framework;
using System.Windows.Controls;

namespace Renci.Wwt.DataManager.Views
{
    /// <summary>
    /// Interaction logic for DataSourceFilterListView.xaml
    /// </summary>
    public partial class FilterListView : UserControl
    {
        public FilterListView(FilterListViewModel viewModel)
        {
            InitializeComponent();

            ViewModelBinder.Bind(viewModel, this);
        }
    }
}
