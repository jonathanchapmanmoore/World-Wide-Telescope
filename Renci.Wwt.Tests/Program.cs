using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.Core;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Globalization;
using Renci.Wwt.Core.Frames.Layers;
using Renci.Wwt.Core.Frames.Layers.Spreadsheet;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI;


namespace Renci.Wwt.Tests
{
    class Program
    {

        public static Color Lerp(Color colour, Color to, float amount)
        {
            // start colours as lerp-able floats
            float sr = colour.R, sg = colour.G, sb = colour.B;

            // end colours as lerp-able floats
            float er = to.R, eg = to.G, eb = to.B;

            // lerp the colours to get the difference
            byte r = (byte)Lerp(sr, er, amount),
                 g = (byte)Lerp(sg, eg, amount),
                 b = (byte)Lerp(sb, eb, amount);

            // return the new colour
            return Color.FromArgb(r, g, b);
        }

        public static float Lerp(float start, float end, float amount)
        {
            if (amount > 1)
                amount = 1;

            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }

        static void Main(string[] args)
        {
            //var path = @"D:\Test\NCFS\fort.63.nc";
            //var path = @"D:\Test\eastcoast_95d_ll.nc";
            //var path = @"D:\Test\isabel_v9.35.nc";
            //var path = @"D:\Test\NCFS\SPDSnc6b-UNC_vortex-nws319_20110826T1200_20110826T1200_20110831T1200_12elev_Z.nc";
            //var path = @"D:\WWT.DataManager.Demo\isabel_v9.81.nc";

            //var reader = NetCDFReader.Create(path);

            //var ele = reader.ReadDecimalVariable("ele");

            //var zeta = reader.ReadDecimalVariable("zeta").Select((v) => { return decimal.ToSingle(v); }).ToArray();

            //var values1 = reader.ReadVariable("zeta");
            //foreach (var value1 in values1)
            //{
            //    decimal abc = (decimal)value1;
            //}
            //var values2 = reader.ReadDecimalVariable("zeta").Select((v) => { return decimal.ToDouble(v); }).ToArray();


            //var values = reader.ReadVariable<float>("zeta");



            //double a = (double)values[0];
            ////  Find maximum and minimum values
            //var missingValue = (from attribute in reader.Variables["zeta"].Attributes
            //                    where attribute.Key == "missing_value"
            //                    select ((double[])attribute.Value.Value).FirstOrDefault()).FirstOrDefault();

            //var maximumValue = (from v in values where v != missingValue select v).Max();
            //var minimumValue = (from v in values where v != missingValue select v).Min();

            //var total = (from v in values where v < 0 select v).Count();
            //var total1 = (from v in values where v == missingValue select v).Count();

            //var ele = reader.Variables["ele"];

            //var variables = from v in reader.Variables
            //                from d in v.Value.DimensionIDs
            //                from dd in reader.Dimensions
            //                where dd.Key == d
            //                select new
            //                {
            //                    Name = v.Key,
            //                    NN = dd.Value.Name,
            //                };

            //var names = variables.ToList();
            //var variable = reader.Variables["zeta"];

            //var aa = (from v in reader.Variables select v.Key).Concat(new string[] { "<none>" });

            //return;

            //var wwt = new WwtClient("janavar.europa.renci.org");
            var wwt = new WwtClient();
            //var f = wwt.Frames;
            //wwt.ViewClock = DateTime.Now.AddDays(-10);

            var layer = new SpreadsheetLayer(wwt, wwt.Earth, "test");

            //var geography = "Polygon ((0 0, 0 1, 1 0, 0 0))";
            var geography = "Polygon ((0 0, 1 0, 0 1, 0 0))";

            layer.AddData(new SpreadsheetDataItem("test 1", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(9), geography, Color.Blue, string.Empty, string.Empty));

            return;

            //layer.AddData(new SpreadsheetDataItem("test 2", DateTime.Now.AddDays(-9), DateTime.Now.AddDays(-8), 3, 0, 0, 10, Color.Green));

            //layer.Activate();

            //layer.ClearData();

            //layer.Name = "asdasdasdas";

            //layer.Update();

            //layer.Delete();

            //wwt.Timerate = 4;
            //var viewState = wwt.GetViewState();

            //wwt.FlyTo(0, 0, 123, 0, 0);
            //wwt.FlyTo(5, 0, 60, 0, 0);
            //wwt.FlyTo(0, 5, 123, 0, 0);

            //wwt.Test();

            return;

            //var colors = new Color[] { Color.Red, Color.Green, Color.Blue };
            //Random rnd1 = new Random();
            //for (int j = 0; j < colors.Length; j++)
            //{
            //    var testLayer = SpreadsheetLayer.Create(wwt, string.Format("Layer {0}", colors[j]), "Earth");
            //    for (int i = 0; i < 3; i++)
            //    {
            //        var latrnd = ((double)rnd1.Next(27222) / 1000000);
            //        var lonrnd = ((double)rnd1.Next(34166) / 1000000);
            //        var lat = 36.084167 + latrnd;
            //        var lon = -79.609444 + lonrnd;
            //        var height = rnd1.NextDouble() * 1000000;
            //        //testLayer.AddData(new SpreadsheetDataItem("Test", DateTime.Now, DateTime.Now, lat, lon, height, (float)0.000000001, colors[j]));
            //        testLayer.AddData(new SpreadsheetDataItem("Test", DateTime.Now, DateTime.Now, lat, lon, 0.001, (float)0.002, colors[j]));
            //    }
            //    testLayer.AddData(new SpreadsheetDataItem("Test", DateTime.Now, DateTime.Now, "MULTILINE (-79.575278 36.084167, -79.609444 36.111389)", colors[j], string.Empty, string.Empty));
            //    testLayer.Update();
            //    testLayer.Activate();
            //}
            //var geoLayer = SpreadsheetLayer.Create(wwt, "Geography", "Earth");
            ////geoLayer.AddData(new SpreadsheetDataItem("Test", DateTime.Now, DateTime.Now, "MULTILINE (-79.575278 36.084167 1, -79.609444 36.111389 1)", Color.Brown, string.Empty, string.Empty));
            //geoLayer.AddData(new SpreadsheetDataItem("Test", DateTime.Now, DateTime.Now, "Polygon ((-79.599278 36.100167 6,-79.599278 36.090167 6,-79.601444 36.103167 6,-79.601444 36.105389 6 ) 6)", Color.BlueViolet, string.Empty, string.Empty));


            //geoLayer.Update();
            //geoLayer.Activate();

            // LONG: -79.575278 -79.609444
            //  LAT: 36.084167    36.111389
            //36.06 36.05 60
            //-79.36 -79.34

            //var isabelLayer = SpreadsheetLayer.Create(wwt, "isabel", "Earth");
            //isabelLayer.AltUnit = AltitudeUnits.Feet;
            //using (var file = File.OpenText(@"C:\Test\isabel.dat"))
            //{
            //    var line = file.ReadLine();
            //    while (line != null)
            //    {
            //        var lineParts = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            //        var lon = double.Parse(lineParts[0]);
            //        var lat = double.Parse(lineParts[1]);
            //        var depth = double.Parse(lineParts[2]);
            //        isabelLayer.AddData(new SpreadsheetDataItem("Depth:" + depth, DateTime.Now, DateTime.Now, lat, lon, depth, 2, Color.Blue));

            //        line = file.ReadLine();
            //    }
            //}
            //isabelLayer.Update();
            //isabelLayer.Activate();


            //var isabelV9Layer = SpreadsheetLayer.Create(wwt, "isabel_v9", "Earth");

            //DataSetFactory.Register(typeof(NetCDFDataSet));
            //using (var ds = sds.DataSet.Open(@"C:\Test\isabel_v9.35.nc"))
            //{
            //    var time = ds["time"].GetData();
            //    var zeta = (Single[,])ds["zeta"].GetData();
            //    var bnd = ds["bnd"].GetData();
            //    var depth = ds["depth"].GetData();
            //    var ele = ds["ele"].GetData();
            //    var land_binary_mask = ds["land_binary_mask"].GetData();
            //    var lat = ds["lat"].GetData();
            //    var lon = ds["lon"].GetData();
            //    var tri_grid = ds["tri_grid"].GetData();
            //    var water_binary_mask = ds["water_binary_mask"].GetData();

            //    var file = File.CreateText(@"C:\Test\isabel_v9.35\input.xyz");
            //    for (int i = 0; i < lat.Length; i++)
            //    {
            //        var lonValue = (double)lon.GetValue(i);
            //        var latValue = (double)lat.GetValue(i);
            //        //if (lonValue < -78 || lonValue > -77)
            //        //    continue;
            //        //if (latValue < 33.5 || latValue > 34.5)
            //        //    continue;
            //        //if (lonValue < -78 || lonValue > -77.95)
            //        //    continue;
            //        //if (latValue < 33.95 || latValue > 34)
            //        //    continue;


            //        var color = Lerp(Color.Red, Color.Blue, (float)(((double)depth.GetValue(i) + 100) / 200));
            //        //isabelLayer.AddData(new SpreadsheetDataItem("Depth:" + depth.GetValue(i), DateTime.Now, DateTime.Now, (double)lat.GetValue(i), (double)lon.GetValue(i), (double)depth.GetValue(i), 2, color));
            //        isabelV9Layer.AddData(new SpreadsheetDataItem("Depth:" + depth.GetValue(i), DateTime.Now, DateTime.Now, (double)lat.GetValue(i), (double)lon.GetValue(i), zeta[0, i], 2, color));
            //        //file.WriteLine(string.Format("{0},{1},{2}", lon.GetValue(i), lat.GetValue(i), depth.GetValue(i)));
            //    }

            //    file.Close();

            //    //foreach (var variable in ds.Variables)
            //    //{
            //    //    var data = variable.GetData();
            //    //    Console.WriteLine(variable);
            //    //}
            //}

            //isabelV9Layer.Update();
            //isabelV9Layer.Activate();

            return;

            //var layers = wwt.GetLayers();

            //var state1 = wwt.ViewState;
            //wwt.Test();
            //return;

            //var date = DateTime.Parse("1/9/2009 3:44");
            //wwt.SetViewClock(date);

            //var layer1 = SpreadsheetLayer.Create(wwt, "nc_inundation_v9.81_MSL.grd", "Earth");

            //using (var file = File.OpenText(@"C:\Test\adcirc\adcirc\nc_inundation_v9.81_MSL.grd"))
            //{
            //    var line = file.ReadLine();
            //    line = file.ReadLine();
            //    line = file.ReadLine();
            //    var count = 0;
            //    while (!string.IsNullOrEmpty(line) && count < 600000)
            //    {
            //        count++;
            //        var lon = double.Parse(line.Substring(11, 15));
            //        var lat = double.Parse(line.Substring(27, 15));
            //        var depth = double.Parse(line.Substring(43, 10));
            //        layer1.AddData(new SpreadsheetDataItem("Point", DateTime.Today, DateTime.Today, lat, lon, depth * 10, 2, Color.Red));
            //        line = file.ReadLine();
            //    }
            //}
            //layer1.Update();
            //layer1.Activate();

            //var layer1 = SpreadsheetLayer.Create(wwt, "layer1", "Earth");
            //layer1.AddData(new SpreadsheetDataItem("Point A", DateTime.Parse("1/9/2009 3:44"), DateTime.Parse("1/9/2009 4:44"), 0, 0, 10, 7, Color.Red, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point B", DateTime.Parse("1/9/2009 4:44"), DateTime.Parse("1/9/2009 5:44"), 1, 0, 10, 7, Color.Red, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point B", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), 2, 0, 10, 7, Color.Red, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point B", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), "Polygon ((32.987808 -17.265003 1,33.0730510000001 -18.3488919999999 1,32.6991650000001 -18.944447 1,33.0188830000001 -19.9433359999999 1,32.5022200000001 -20.598614 1,32.4888760000001 -21.3444479999999 1,31.2975040000001 -22.414764 1,29.893887 -22.194447 1,29.3736230000001 -22.19241 1,29.060555 -21.798058 1,28.015831 -21.566113 1,27.713165 -20.506432 1,27.287453 -20.494965 1,27.219997 -20.091667 1,26.166111 -19.527779 1,25.264431 -17.80225 1,27.038055 -17.959446 1,27.825275 -16.959167 1,28.759441 -16.552223 1,28.927219 -15.972223 1,30.415756 -15.631872 1,30.422775 -16.009167 1,31.276665 -16.018612 1,32.9811400000001 -16.7090529999999 1,32.987808 -17.265003 1) 1)", 7, Color.Red, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point A", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), "Polygon ((2 2 1,2 1 1, 1 1 1, 1 2 1) 1)", 7, Color.Red, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point B", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), "Point (5 5)", 7, Color.Green, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point B", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), "MULTIPOINT(0 0,1 2)", 1, Color.Red, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point A", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), "Polygon ((2 2 1,2 1 1, 1 1 1, 1 2 1) 1)", Color.Green, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point A", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), "Polygon ((2 2 10,2 1 10, 1 1 1, 1 2 1) 10)", Color.Green, null, "http://www.cnn.com"));
            //layer1.AddData(new SpreadsheetDataItem("Point A", DateTime.Parse("1/9/2009 5:44"), DateTime.Parse("1/9/2009 6:44"), "POINT (2 2 1)", Color.Green, null, "http://www.cnn.com"));

            //layer1.AddData(new SpreadsheetDataItem("Point A", DateTime.Parse("1/9/2009 4:44"), DateTime.Parse("1/9/2009 5:44"), 1, 1, 10, 7, Color.Green, null, "http://www.cnn.com"));
            //layer1.Update();

            //layer1.AddData(new SpreadsheetDataItem("Point B", DateTime.Parse("1/9/2009 4:44"), DateTime.Parse("1/9/2009 5:44"), 1, 0, 10, 7, Color.Red, null, "http://www.cnn.com"));
            //layer1.Update();

            //layer1.ClearData();
            //var data1 = sosData.Where((i)=>i.date_time > DateTime.Today.AddDays(-1)).OrderBy((i) => i.date_time).ToList();

            //wwt.SetViewClock(data1[0].date_time);
            //foreach (var item in data1)
            //{
            //    layer1.AddData(new SpreadsheetDataItem(item.station_id, item.date_time, item.date_time, item.latitude, item.longitude, item.depth, 3, Color.Red, null, "http://www.cnn.com"));
            //}
            //layer1.Update();

            return;

            var counter = 100;
            Random rnd = new Random();

            ////DateTime date = DateTime.Parse("6/10/2011 10:08:16").AddHours(-counter);
            //var color = Color.FromArgb(229, Color.Red);
            //for (int i = 0; i < counter; i++)
            //{
            //    //var lat = rnd.Next(-90000, 90000) / 1000;
            //    //var lng = rnd.Next(-180000, 180000) / 1000;
            //    var lat = rnd.Next(-5000, 5000) / 1000;
            //    var lng = rnd.Next(-5000, 5000) / 1000;
            //    var alt = (float)rnd.Next(-180000, 180000) / 1000;
            //    var size = rnd.Next(1, 20);
            //    //layer1.Add(new SpreadsheetDataItem("test1", DateTime.Now, null, 0, 0, 10, 100, Color.Blue, null, null));
            //    layer1.AddData(new SpreadsheetDataItem(string.Format("Point {0}, Alt: {1}, DateTime: {2}", i, alt, date.AddHours(i)), date.AddHours(i), date.AddHours(i), lat, lng, alt, size, color, null, null));
            //    //sd.Add(new SpreadsheetPoint(string.Format("Point {0}, Alt: {1}, DateTime: {2}", i, alt, date.AddHours(i)), date.AddHours(i), date.AddHours(i), lat, lng, alt, size, color, null, null));
            //}


            //var layer2 = new SpreadsheetLayer(wwt, "layer2", "Earth", false);

            //var vers = wwt.Version;
            //var s = wwt.ViewState;

            //var layerManager = wwt.LayerManager;
            //layerManager.Add("New Layer", DateTime.Parse("1/9/2009 3:44:38 AM"), "Earth", Color.Aqua, "TIME", "LAT", "LON", "DEPTH", "MAG", "Color");
            //layerManager.Add("New Layer", DateTime.Parse("1/9/2009 3:44:38 AM"), "Earth", Color.Aqua, "A", "B", "C", "D", "E", "F");
            //layerManager.Add("Layer1", DateTime.Parse("1/9/2009 3:44:38 AM"), "Earth", Color.Aqua);
            //layerManager.Add("Layer1", "Earth");

            //var testLayer = layerManager.Layers.OfType<SpreadsheetLayer>().First();

            //SpreadsheetData sd = new SpreadsheetData();

            //var aa = date.AddHours(100);

            for (int i = 0; i < counter; i++)
            {
                //var lat = rnd.Next(-90000, 90000) / 1000;
                //var lng = rnd.Next(-180000, 180000) / 1000;
                var lat = rnd.Next(-5000, 5000) / 1000;
                var lng = rnd.Next(-5000, 5000) / 1000;
                var alt = (float)rnd.Next(-180000, 180000) / 1000;
                var size = rnd.Next(1, 20);
                //testLayer.Add(new SpreadsheetDataItem(string.Format("Point {0}, Alt: {1}, DateTime: {2}", i, alt, date.AddHours(i)), date.AddHours(i), date.AddHours(i), lat, lng, alt, size, color, null, null));
                //sd.Add(new SpreadsheetPoint(string.Format("Point {0}, Alt: {1}, DateTime: {2}", i, alt, date.AddHours(i)), date.AddHours(i), date.AddHours(i), lat, lng, alt, size, color, null, null));
            }

            //sd.Add(new SpreadsheetPoint("p1", DateTime.Now, null, 0, 0, 10, 7, Color.Red, null, null));
            //sd.Add(new SpreadsheetPoint("p2", DateTime.Now.AddDays(-1), null, 0, 10, 243, 9, Color.Blue, null, null));
            //sd.Add(new SpreadsheetPoint("p3", DateTime.Now.AddDays(-2), null, -5, -5, 35, 8, Color.Green, null, null));

            //testLayer.Insert(sd);


            //layers.Add("abcd2", DateTime.Now, "Earth", Color.Aqua, "aa", "ss", "dd");

            //wwt.SetMode("Earth");
        }

    }
}
