// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Linq;
using Microsoft.Research.Science.Data.Imperative;
using sds = Microsoft.Research.Science.Data;

namespace Tutorial4
{
    class Program
    {
        static void Main(string[] args)
        {
            // Data file (126 MB) can be downloaded from
            // ftp://ftp.cdc.noaa.gov/Datasets/ncep.reanalysis2.derived/pressure/air.mon.mean.nc
            var dataset = sds.DataSet.Open("air.mon.mean.nc?openMode=readOnly");
            var valid_range = dataset.GetAttr<short[]>("air", "valid_range");
            var scale = dataset.GetAttr<float>("air", "scale_factor");
            var offset = dataset.GetAttr<float>("air", "add_offset");
            var lat = dataset.GetData<float[]>("lat");
            for (int year = 0; year < 31; year++)
            {
                var air = dataset.GetData<short[, ,]>("air",
                    sds.DataSet.Range(year * 12, year * 12 + 11),
                    sds.DataSet.ReduceDim(0),
                    sds.DataSet.Range(1, 71),
                    sds.DataSet.FromToEnd(0));
                double sum = 0;
                double sum_w = 0;
                for (int m = 0; m < 12; m++)
                    for (int i = 0; i < 71; i++)
                        for (int j = 0; j < 144; j++)
                            if (air[m, i, j] >= valid_range[0]
                                && air[m, i, j] <= valid_range[1])
                            {
                                double w = Math.Cos(lat[i + 1] / 180.0 * Math.PI);
                                sum += air[m, i, j] * w;
                                sum_w += w;
                            }
                Console.WriteLine("{0} {1}", year + 1979, sum / sum_w * scale + offset - 273.15);
            }
        }
    }
}

