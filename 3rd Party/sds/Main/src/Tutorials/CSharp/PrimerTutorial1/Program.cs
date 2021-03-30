// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System.Linq;
using Microsoft.Research.Science.Data.Imperative;
using sds = Microsoft.Research.Science.Data;

namespace Tutorial1
{
    class Program
    {
        static void Main(string[] args)
        {
            // read input data
            var dataset = sds.DataSet.Open("Tutorial1.csv");
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
            // add new variable
            dataset.Add<double[]>("Model");
            dataset.PutData<double[]>("Model", model);
        }
    }
}

