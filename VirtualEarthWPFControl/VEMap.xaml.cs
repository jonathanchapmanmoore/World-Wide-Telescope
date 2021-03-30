/*   
 * Author(s): BurcuDogan (http://burcudogan.com)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace VirtualEarthWPFControl
{
    public delegate void VEEventHandler(object sender, VEEventArgs e);
    /// <summary>
    /// Interaction logic for VEMap.xaml
    /// </summary>
    public partial class VEMap : UserControl
    {
        public event EventHandler ModeInitialized;

        // TODO: Implement event handling
        // Keyboard events

        // Mapviewer events; TODO: Implement the rest
        public event VEEventHandler OnMapLoad;
        public event VEEventHandler OnChangeMapStyle;
        public event VEEventHandler OnChangeView;
        public event VEEventHandler OnEndPan;
        public event VEEventHandler OnEndZoom;
        public event VEEventHandler OnInitMode;
        public event VEEventHandler OnModeNotAvailable;
        public event VEEventHandler OnResize;
        public event VEEventHandler OnStartPan;
        public event VEEventHandler OnStartZoom;

        // Mouse events
        public event VEEventHandler OnClick;
        public event VEEventHandler OnDoubleClick;
        public event VEEventHandler OnMouseMove;
        public event VEEventHandler OnMouseDown;
        public event VEEventHandler OnMouseUp;
        public event VEEventHandler OnMouseOver;
        public event VEEventHandler OnMouseOut;
        public event VEEventHandler OnMouseWheel;


        // Keyboard events
        public event VEEventHandler OnKeyPress;
        public event VEEventHandler OnKeyDown;
        public event VEEventHandler OnKeyUp;

        public Boolean MapLoaded
        {
            get { return this.loaded; }
        }

        List<UserControl> controlsAdded = new List<UserControl>();

        /// <summary>
        /// Constructor, initializes a WebBrowser, and defines the communication
        /// path between map page and this class.
        /// </summary>
        public VEMap()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.WebBrowser.ObjectForScripting = new WindowExternalHelper(this);
                var assembly = Assembly.GetExecutingAssembly();
                var source = assembly.GetManifestResourceStream("VirtualEarthWPFControl.Map.htm");
                WebBrowser.NavigateToStream(source);
            }
        }

        /// <summary>
        /// Removes all shapes, shape layers, routes and search results on the map
        /// </summary>
        public void Clear()
        {
            /*
             * foreach (var tempPushpin in this.pushPins){this.topWindow.mainCanvas.Children.Remove(tempPushpin.Button);}this.pushPins.Clear(); 
             */
            WebBrowser.InvokeScript(this.GetType().Name + "Clear");
        }

        public void Dispose()
        {
            WebBrowser.InvokeScript(this.GetType().Name + "Dispose");
        }


        /// TODO: Implement the other overloadings for Find method.
        /// <summary>
        /// Async call to point map. Where parameter is geocoded and 
        /// map is located to the (lat,long) geocoder returns.
        /// </summary>
        /// <param name="where">Address input</param>
        public void Find(string where)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "Find", where);
        }

        public double GetAltitude()
        {
            return Convert.ToDouble(WebBrowser.InvokeScript(this.GetType().Name + "GetAltitude"));
        }

        /// <summary>
        /// Returns (lat,long) of the map center.
        /// </summary>
        /// <returns></returns>
        public VELatLong GetCenter()
        {
            var result = WebBrowser.InvokeScript(this.GetType().Name + "GetCenter");
            var location = result.ToString().Split(',');
            return new VELatLong(double.Parse(location[0]), double.Parse(location[1]));
        }

        public double GetHeading()
        {
            return Convert.ToDouble(WebBrowser.InvokeScript(this.GetType().Name + "GetHeading"));
        }

        public int GetLeft()
        {
            return Convert.ToInt32(WebBrowser.InvokeScript(this.GetType().Name + "GetLeft"));
        }

        /// TODO: Convert numeric returning value to VEMapMode
        /// <summary>
        /// Returns the current map mode: 2D or 3D.
        /// </summary>
        /// <returns></returns>
        public int GetMapMode()
        {
            return Convert.ToInt32(WebBrowser.InvokeScript(this.GetType().Name + "GetMapMode"));
        }

        /// TODO: Convert character representation into VEMapStyle.
        /// <summary>
        /// Returns the current map style: Road, Aerial, Hybrid.
        /// </summary>
        /// <returns></returns>
        public string GetMapStyle()
        {
            return (WebBrowser.InvokeScript(this.GetType().Name + "GetMapStyle").ToString());
        }

        public double GetPitch()
        {
            return Convert.ToDouble(WebBrowser.InvokeScript(this.GetType().Name + "GetPitch"));
        }

        public int GetTop()
        {
            return Convert.ToInt32(WebBrowser.InvokeScript(this.GetType().Name + "GetTop"));
        }

        /// <summary>
        /// Return the version of the Virtual Earth Interactive SDK.
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return (WebBrowser.InvokeScript(this.GetType().Name + "GetVersion")).ToString();
        }

        /// <summary>
        /// Returns zoom level.
        /// </summary>
        /// <returns></returns>
        public int GetZoomLevel()
        {
            return Convert.ToInt32(WebBrowser.InvokeScript(this.GetType().Name + "GetZoomLevel"));
        }

        /// <summary>
        /// Hides the map control dashboard.
        /// </summary>
        public void HideDashboard()
        {
            WebBrowser.InvokeScript(this.GetType().Name + "HideDashboard");
        }

        /// <summary>
        /// Checks bird's eye imagery availability.
        /// </summary>
        /// <returns></returns>
        public Boolean IsBirdsEyeAvailable()
        {
            return Convert.ToBoolean(WebBrowser.InvokeScript(this.GetType().Name + "IsBirdsEyeAvailable"));
        }

        /// <summary>
        /// Converts a (lat,long) couple to their corresponding pixels on the map viewer.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public VEPixel LatLongToPixel(VELatLong position)
        {
            var result = WebBrowser.InvokeScript(this.GetType().Name + "LatLongToPixel",
                                                                position.Latitude, position.Longitude);
            if (result.ToString() != String.Empty)
            {
                var results = result.ToString().Split(',');
                return new VEPixel(Convert.ToInt32(results[0]), Convert.ToInt32(results[1]));
            }

            return null;
        }

        /// <summary>
        /// Loads/unloads traffic overlay.
        /// </summary>
        /// <param name="b"></param>
        public void LoadTraffic(Boolean b)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "LoadTraffic", b.ToString());
        }

        /// <summary>
        /// Pans the map.
        /// </summary>
        /// <param name="deltaX">Delta pixels to pan on x axis.</param>
        /// <param name="deltaY">Delta pixels to pan on y axis.</param>
        public void Pan(int deltaX, int deltaY)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "Pan", deltaX, deltaY);
        }

        /// <summary>
        /// Pans the map to specified (lat,lon).
        /// </summary>
        /// <param name="position"></param>
        public void PanToLatLong(VELatLong position)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "PanToLatLong", position.Latitude, position.Longitude);
        }

        /// <summary>
        /// Converts a pixel to its corresponding lat,long on the mapviewer.
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public VELatLong PixelToLatLong(VEPixel pixel)
        {
            var result = WebBrowser.InvokeScript(this.GetType().Name + "PixelToLatLong", pixel.X, pixel.Y);
            var results = result.ToString().Split(',');
            return new VELatLong(Convert.ToDouble(results[0]), Convert.ToDouble(results[1]));
        }

        /// <summary>
        /// Resizes the map.
        /// </summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        public void Resize(int width, int height)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "Resize", width, height);
        }

        /// <summary>
        /// Sets center of the map to (lat,long).
        /// </summary>
        /// <param name="position"></param>
        public void SetCenter(VELatLong position)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetCenter", position.Latitude, position.Longitude);
        }

        /// <summary>
        /// Sets center of the map to position, and zooms to zoomLevel.
        /// </summary>
        /// <param name="position">New center position.</param>
        /// <param name="zoomLevel">New zoom level.</param>
        public void SetCenterAndZoom(VELatLong position, int zoomLevel)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetCenterAndZoom",
                                                                        position.Latitude,
                                                                        position.Longitude,
                                                                        zoomLevel);
        }

        public void SetHeading(double heading)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetHeading", heading);
        }
        /// <summary>
        /// Sets map mode to 2D or 3D.
        /// </summary>
        /// <param name="mode"></param>
        public void SetMapMode(int mode)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetMapMode", mode);
        }

        /// <summary>
        /// Sets map style to road, aerial or hybrid.
        /// </summary>
        /// <param name="style"></param>
        public void SetMapStyle(string style)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetMapStyle", style);
        }

        public void SetPitch(double pitch)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetPitch", pitch);
        }

        /// <summary>
        /// Sets the number of "rings" of map tiles that should be loaded outside 
        /// of the visible mapview area. This is also called tile overfetching.
        /// </summary>
        /// <param name="num">Default is 0, maximum is 3.</param>
        public void SetTileBuffer(int num)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetTileBuffer", num);
        }

        /// <summary>
        /// Set zoom levels to the new level specified.
        /// </summary>
        /// <param name="zoomLevel"></param>
        public void SetZoomLevel(int zoomLevel)
        {
            WebBrowser.InvokeScript(this.GetType().Name + "SetZoomLevel", zoomLevel);
        }

        /// <summary>
        /// Shows built-in VE map control dashboard on map viewer.
        /// </summary>
        public void ShowDashboard()
        {
            WebBrowser.InvokeScript(this.GetType().Name + "ShowDashboard");
        }

        /// <summary>
        /// Zooms in one level.
        /// </summary>
        public void ZoomIn()
        {
            WebBrowser.InvokeScript(this.GetType().Name + "ZoomIn");
        }

        /// <summary>
        /// Zooms out a level.
        /// </summary>
        public void ZoomOut()
        {
            WebBrowser.InvokeScript(this.GetType().Name + "ZoomOut");
        }

        public void LoadMap()
        {
            this.Visibility = Visibility.Visible;
        }





        internal void ThrowException(Exception ex)
        {
            throw ex;
        }

        internal void RaiseModeInitialized()
        {
            this.ModeInitialized(this, new EventArgs());
        }



        internal void RaiseViewChanged()
        {
            //this.ViewChanged(this, new EventArgs());
            //this.RepositionTopWindowElements();            
        }

        /*private void RepositionTopWindowElements()
        {
            if (!modeChanging)
            {
                foreach (var pushPin in this.pushPins)
                {
                    var latLong = new VELatLong(pushPin.LatLong.Latitude, pushPin.LatLong.Longitude);
                    var pixel = this.LatLongToPixel(latLong);
                    if (pixel != null)
                    {
                        pushPin.Button.Visibility = Visibility.Visible;
                        var y = pixel.Y - (pushPin.Button.Height / 2);
                        var x = pixel.X - (pushPin.Button.Width / 2);
                        Canvas.SetTop(pushPin.Button, y);
                        Canvas.SetLeft(pushPin.Button, x);
                    }
                    else
                    {
                        pushPin.Button.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }*/

        internal void RaiseMapLoaded()
        {
            //this.loaded = true;

            //BUG: This is causing an exception.  Need to investigate
            //this.MapLoaded(this, new EventArgs());
        }


        /*public void AddShape(VEShape shape)
        {
            switch (shape.Type)
            {
                case VEShapeType.Pushpin:
                    //For pushpins, we render "invisible" buttons using WPF
                    var button = new Button();
                    //NEED to evaluate a better way...
                    button.Tag = shape.Tag;

                    // Since we don't want to see the WPF/Surface button, we set it's opacity to .01
                    // We will still use it for the MouseEnter event though.

                    button.Opacity = .01;

                    button.MouseEnter += new MouseEventHandler(button_MouseEnter);

                    button.Width = 25;
                    button.Height = 25;
                    var pixel = this.LatLongToPixel(shape.Points[0]);
                    var y = pixel.Y - (button.Height / 2);
                    var x = pixel.X - (button.Width / 2);
                    Canvas.SetTop(button, y);
                    Canvas.SetLeft(button, x);

                    this.topWindow.mainCanvas.Children.Add(button);
                    this.pushPins.Add(new TempPushpin { Button = button, LatLong = shape.Points[0] });
                    this.WebBrowser.InvokeScript("AddShape", "Pushpin", ListOfLatLongToString(shape.Points));
                    break;
                case VEShapeType.Polyline:
                    //For polylines, we just let the map render
                    this.WebBrowser.InvokeScript("AddShape", "Polyline", ListOfLatLongToString(shape.Points));
                    break;
                case VEShapeType.Polygon:
                    //For polygons, we just let the map render
                    this.WebBrowser.InvokeScript("AddShape", "Polygon", ListOfLatLongToString(shape.Points));
                    break;
            }
        }*/


        public void AddControl(UserControl control, int? zIndex)
        {
            control.Tag = zIndex;
            this.controlsAdded.Add(control);
        }

        public void SetMapView(List<VELatLong> arrayOfLatLong)
        {

            var stringOfLatLong = ListOfLatLongToString(arrayOfLatLong);
            this.WebBrowser.InvokeScript("SetMapView",
                            stringOfLatLong);
        }

        private string ListOfLatLongToString(List<VELatLong> arrayOfLatLong)
        {
            var sb = new StringBuilder();

            foreach (var latLong in arrayOfLatLong)
            {
                sb.Append(latLong.ToString());
                sb.Append("|");
            }

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        internal Boolean loaded = false;

        // Firing events..., ignore one line typing please.
        ///////// Virtual Earth Events
        internal void _MapLoad(VEEventArgs e)
        {
            if (OnMapLoad != null) OnMapLoad(this, e);
        }

        internal void _ChangeMapStyle(VEEventArgs e)
        {
            if (OnChangeMapStyle != null) OnChangeMapStyle(this, e);
        }

        internal void _ChangeView(VEEventArgs e)
        {
            if (OnChangeView != null) OnChangeView(this, e);
        }

        internal void _EndPan(VEEventArgs e)
        {
            if (OnEndPan != null) OnEndPan(this, e);
        }

        internal void _EndZoom(VEEventArgs e)
        {
            if (OnEndZoom != null) OnEndZoom(this, e);
        }

        internal void _InitMode(VEEventArgs e)
        {
            if (OnInitMode != null) OnInitMode(this, e);
        }

        internal void _ModeNotAvailable(VEEventArgs e)
        {
            if (OnModeNotAvailable != null) OnModeNotAvailable(this, e);
        }

        internal void _Resize(VEEventArgs e)
        {
            if (OnResize != null) OnResize(this, e);
        }

        internal void _StartPan(VEEventArgs e)
        {
            if (OnStartPan != null) OnStartPan(this, e);
        }

        internal void _StartZoom(VEEventArgs e)
        {
            if (OnStartZoom != null) OnStartZoom(this, e);
        }

        ///////// Mouse Events
        internal void _Click(VEEventArgs e)
        {
            if (OnClick != null) OnClick(this, e);
        }

        internal void _DoubleClick(VEEventArgs e)
        {
            if (OnDoubleClick != null) OnDoubleClick(this, e);
        }

        internal void _MouseMove(VEEventArgs e)
        {
            if (OnMouseMove != null) OnMouseMove(this, e);
        }

        internal void _MouseDown(VEEventArgs e)
        {
            if (OnMouseDown != null) OnMouseDown(this, e);
        }

        internal void _MouseUp(VEEventArgs e)
        {
            if (OnMouseUp != null) OnMouseUp(this, e);
        }

        internal void _MouseOver(VEEventArgs e)
        {
            if (OnMouseOver != null) OnMouseOver(this, e);
        }

        internal void _MouseOut(VEEventArgs e)
        {
            if (OnMouseOut != null) OnMouseOut(this, e);
        }

        internal void _MouseWheel(VEEventArgs e)
        {
            if (OnMouseWheel != null) OnMouseWheel(this, e);
        }

        ///////// Keyboard Events
        internal void _KeyPress(VEEventArgs e)
        {
            if (OnKeyPress != null) OnKeyPress(this, e);
        }

        internal void _KeyDown(VEEventArgs e)
        {
            if (OnKeyDown != null) OnKeyUp(this, e);
        }

        internal void _KeyUp(VEEventArgs e)
        {
            if (OnKeyUp != null) OnKeyUp(this, e);
        }

    }
}
