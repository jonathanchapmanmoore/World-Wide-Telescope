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
using System.Windows.Threading;
using Microsoft.Research.Science.Data.Factory;

namespace VisualizationTutorial4
{
    /// <summary>This sample demonstrated dynamic updates of Dataset</summary>
    public partial class MainWindow : Window
    {
        private bool running = false;
        private DataSet dataset;

        public MainWindow()
        {
            DataSetFactory.SearchFolder(Environment.CurrentDirectory);

            InitializeComponent();
            InitModel();
            dsvc.DataSet = dataset;
        }

        /// <summary>Inits structure of dataset and performs one modelling step</summary>
        private void InitModel()
        {
            dataset = DataSet.Open("msds:memory");
            dataset.IsAutocommitEnabled = false;
            dataset.Add<double[]>("sin");
            dataset.Add<double[]>("cos");
            dataset.Add<double[]>("x");
            dataset.Add<double>("phase");
            dataset.Commit();

            dataset.PutData<double>("phase", 0);
            dataset.PutAttr("cos", "VisualHints", "Style:Points(sin,x)");
            dataset.Commit();

            OneStep();
        }

        /// <summary>Performs one modelling step</summary>
        private void OneStep()
        {
            double phase = dataset.GetData<double>("phase");
            const int Size = 300;
            double[] x = new double[Size];
            double[] sin = new double[Size];
            double[] cos = new double[Size];
            for (int i = 0; i < Size; i++)
            {
                x[i] = 2 * Math.PI * i / Size;
                sin[i] = Math.Sin(x[i] + phase);
                cos[i] = Math.Cos(x[i] + phase);
            }
            phase += 0.01;

            dataset.PutData("x", x);
            dataset.PutData("sin", sin);
            dataset.PutData("cos", cos);
            dataset.PutData("phase", phase);

            dataset.Commit();
        }


        /// <summary>Performs one modelling iteration and schedules next one</summary>
        private void NextIteration()
        {
            // Perform one modelling step
            OneStep();

            // Schedule next simulation step
            if (running)
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new NoArgHandler(NextIteration));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!running)
            {
                running = true;
                button.Content = "Stop!";
                NextIteration();
            }
            else
            {
                running = false;
                button.Content = "Start!";
            }
        }
    }

    delegate void NoArgHandler();
}

