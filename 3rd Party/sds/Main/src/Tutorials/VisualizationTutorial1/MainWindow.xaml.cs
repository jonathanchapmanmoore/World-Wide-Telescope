// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
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

using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Imperative;

namespace VisualizationTutorial1
{
    /// <summary>Sample application for setting visual hints programmatically</summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create memory dataset with one variable
            DataSet ds = DataSet.Open("msds:memory");
            ds.AddVariable<double>("values", "i", "j");
            
            // Compute nice-looking data 
            const int Size = 100;
            double[,] values = new double[Size, Size];
            Random r = new Random();
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    values[i, j] = Math.Sin((Math.Sqrt((i - Size / 2) * (i - Size / 2) + 
                                                      (j - Size / 2) * (j - Size / 2)) + r.NextDouble()) / Math.Sqrt(Size));
            
            // Put data to variable
            ds.PutData("values", values);
            ds.Commit();

            // Assign dataset to view
            dsvc.DataSet = ds;

            // Show colormap first
            colorMap.IsChecked = true;
        }

        private void ColorMapChecked(object sender, RoutedEventArgs e)
        {
            dsvc.DataView.SetHints("values Style:Colormap");
        }

        private void TableChecked(object sender, RoutedEventArgs e)
        {
            dsvc.DataView.SetHints("values Style:Matrix");
        }

        private void ContourLinesChecked(object sender, RoutedEventArgs e)
        {
            dsvc.DataView.SetHints("values Style:Isolines");
        }
    }
}

