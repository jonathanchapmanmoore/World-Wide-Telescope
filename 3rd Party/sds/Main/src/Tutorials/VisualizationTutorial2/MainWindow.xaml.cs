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

namespace VisualizationTutorial2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create dataset with three variables sharing one dimension
            DataSet ds = DataSet.Open("msds:memory");
            ds.Add<double[]>("x");
            ds.Add<double[]>("sin");
            ds.Add<double[]>("cos");

            // Populate dataset with data. Following code is short, but not very fast. 
            // It is better to fill array and use one PutData method instead of appending in loop.
            for (double x = 0; x < 2 * Math.PI; x += 0.01)
            {
                ds.Append("x", x);
                ds.Append("sin", Math.Sin(x));
                ds.Append("cos", Math.Cos(x));
            }
            ds.Commit();

            // Tell DataSetViewer how we want to show this dataset
            ds.PutAttr(DataSet.GlobalMetadataVariableID,
                "VisualHints",
                "sin(x) Style:Polyline; Stroke:Orange; Thickness:3;; " +
                "cos(x) Style:Markers; Marker:Circle; Color:Blue; Size:10");

            // Attach data to visualizer
            dsvc.DataSet = ds;
        }
    }
}

