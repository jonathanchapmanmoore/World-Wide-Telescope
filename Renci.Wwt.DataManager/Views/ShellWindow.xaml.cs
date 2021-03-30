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
using System.Windows.Shapes;
using Renci.Wwt.DataManager.ViewModels;
using Renci.Wwt.DataManager.Common.Framework;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Microsoft.Windows.Controls.Ribbon;

namespace Renci.Wwt.DataManager.Views
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : RibbonWindow
    {
        public ShellWindow(ShellWindowModel viewModel)
        {
            InitializeComponent();

            ViewModelBinder.Bind(viewModel, this);
        }
    }
}
