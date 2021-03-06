// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Media;

namespace System.Windows.Controls.Samples
{
    /// <summary>
    /// A class that demonstrates the Rating control.
    /// </summary>
    [Sample("(0)Rating", DifficultyLevel.Basic, "Rating")]
    public partial class RatingSample : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the RatingSample class.
        /// </summary>
        public RatingSample()
        {
            InitializeComponent();
            movie.ValueChanged += OnMovieValueChanged;
        } 

        /// <summary>
        /// Changes the foreground of the rating control to yellow.
        /// </summary>
         /// <param name="sender">Sender Rating.</param>
        /// <param name="e">Event args.</param>
        private void OnMovieValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            movie.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 203, 0));
        }

        /// <summary>
        ///  Set the value of the rating control to 0.
        /// </summary>
        /// <param name="sender">Sender Button.</param>
        /// <param name="e">Event args.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Method is called in XAML.")]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            movie.Value = 0;
        }
    }
}