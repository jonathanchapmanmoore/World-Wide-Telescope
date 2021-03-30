// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Linq;
using Microsoft.Research.Science.Data.Imperative;
using sds = Microsoft.Research.Science.Data;

namespace Tutorial3
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("I expect 2 command line parameters.");

            // open input dataset. Set 'read only' mode by default
            var uri = sds.DataSetUri.Create(args[0]);
            if (!uri.ContainsParameter("openMode"))
                uri.OpenMode = sds.ResourceOpenMode.ReadOnly;
            var input = sds.DataSet.Open(uri);
            Console.WriteLine(input);
            
            // open output dataset. Set 'create' mode by default
            uri = sds.DataSetUri.Create(args[1]);
            if (!uri.ContainsParameter("openMode"))
                uri.OpenMode = sds.ResourceOpenMode.Create;
            var output = sds.DataSet.Open(uri);
            
            // read input data
            var x = input.GetData<double[]>("X");
            var y = input.GetData<double[]>("Observation");
            if (x.Length != y.Length)
                throw new ArgumentException("X and Observation must have equal length");
            
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
            
            // output results
            int x_id = output.Add<double[]>("X", "table1").ID;
            int y_id = output.Add<double[]>("Observation", "table1").ID;
            int m_id = output.Add<double[]>("Model", "table1").ID;
            output.PutAttr(m_id, "long_name", "linear fit to Observation");
            output.PutAttr(m_id, "Model_A", a);
            output.PutAttr(m_id, "Model_B", b);
            output.PutAttr(0, "VisualHints", "Model(X) Style:Polyline;Stroke:Navy;;"
                + "Observation(X) Style:Markers;Color:Red");
            for (int i = 0; i < x.Length; i++)
            {
                output.Append(x_id, x[i]);
                output.Append(y_id, y[i]);
                output.Append(m_id, a * x[i] + b);
            }
            Console.WriteLine(output);
        }
    }
}

