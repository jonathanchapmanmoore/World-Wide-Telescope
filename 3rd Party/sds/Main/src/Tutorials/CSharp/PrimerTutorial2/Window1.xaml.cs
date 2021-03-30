// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Research.Science.Data.Imperative;
using sds = Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Factory;

namespace PrimerTutorial2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			DataSetFactory.SearchFolder(".");
            // read input data
			var dataset = sds.DataSet.Open("Tutorial2.csv?inferDims=true&appendMetadata=true");
            if (!dataset.Any(var => var.Name == "Model"))
            {
                var x = dataset.GetData<double[]>("X");
                var y = dataset.GetData<double[]>("Observation");
                // compute
                var xm = x.Sum() / x.Length;
                var ym = y.Sum() / y.Length;
                double a = 0, d = 0;
                for (int i = 0; i < x.Length; i++)
                {
                    a += (x[i] - xm) * (y[i] - ym);
                    d += (x[i] - xm) * (x[i] - xm);
                }
                a /= d;
                var b = ym - a * xm;
                var model = x.Select(xx => a * xx + b).ToArray();
                //
				var varid = dataset.Add<double[]>("Model", dataset.Dimensions[0].Name).ID;
                dataset.PutData<double[]>(varid, model);
            }
            Viewer.DataSet = dataset;
        }
    }
}

