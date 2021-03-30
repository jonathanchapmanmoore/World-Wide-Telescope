using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using Renci.Wwt.Core.Common;
using Renci.Wwt.Core.Frames;
using Renci.Wwt.Core.Frames.Layers.Spreadsheet;
using System.Web;

namespace Renci.Wwt.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class WwtClient
    {
        private string _machine;

        private int _port;

        #region Reference Frames

        public ReferenceFrame Sky { get { return this.Frames["Sky"] as ReferenceFrame; } }
        public ReferenceFrame Ecliptic { get { return this.Frames["Ecliptic"] as ReferenceFrame; } }
        public ReferenceFrame Galactic { get { return this.Frames["Galactic"] as ReferenceFrame; } }
        public ReferenceFrame Sun { get { return this.Frames["Sun"] as ReferenceFrame; } }
        public ReferenceFrame Mercury { get { return this.Frames["Mercury"] as ReferenceFrame; } }
        public ReferenceFrame Venus { get { return this.Frames["Venus"] as ReferenceFrame; } }
        public ReferenceFrame Earth { get { return this.Frames["Earth"] as ReferenceFrame; } }
        public ReferenceFrame Mars { get { return this.Frames["Mars"] as ReferenceFrame; } }
        public ReferenceFrame Jupiter { get { return this.Frames["Jupiter"] as ReferenceFrame; } }
        public ReferenceFrame Saturn { get { return this.Frames["Saturn"] as ReferenceFrame; } }
        public ReferenceFrame Uranus { get { return this.Frames["Uranus"] as ReferenceFrame; } }
        public ReferenceFrame Neptune { get { return this.Frames["Neptune"] as ReferenceFrame; } }
        public ReferenceFrame Pluto { get { return this.Frames["Pluto"] as ReferenceFrame; } }
        public ReferenceFrame Moon { get { return this.Frames["Moon"] as ReferenceFrame; } }
        public ReferenceFrame Io { get { return this.Frames["Io"] as ReferenceFrame; } }
        public ReferenceFrame Europa { get { return this.Frames["Europa"] as ReferenceFrame; } }
        public ReferenceFrame Ganymede { get { return this.Frames["Ganymede"] as ReferenceFrame; } }
        public ReferenceFrame Callisto { get { return this.Frames["Callisto"] as ReferenceFrame; } }
        public ReferenceFrame Custom { get { return this.Frames["Custom"] as ReferenceFrame; } }
        public ReferenceFrame Identit { get { return this.Frames["Identity"] as ReferenceFrame; } }

        #endregion

        private FrameCollection _frames;
        /// <summary>
        /// Gets the frames.
        /// </summary>
        public FrameCollection Frames
        {
            get
            {
                if (this._frames == null)
                {
                    this._frames = new FrameCollection(this);
                }

                //  Return new instance of layers
                return this._frames;
            }
        }

        private string _version;
        /// <summary>
        /// Gets the version number of the running version of the LCAPI.
        /// </summary>
        public string Version
        {
            get
            {
                if (this._version == null)
                {
                    this._version = this.GetVersion();
                }

                return this._version;
            }
        }

        public DateTime ViewClock
        {
            get
            {
                return this.GetViewState().Time;
            }
            set
            {
                this.Send(string.Format("/layerApi.aspx?cmd=version&datetime={0}", value));
            }
        }

        public int Timerate
        {
            get
            {
                return this.GetViewState().Timerate;
            }
            set
            {
                this.Send(string.Format("/layerApi.aspx?cmd=version&timerate={0}", value));
            }
        }

        public WwtClient()
        {
            //  Get local machine address
            var ip = (from address in Dns.GetHostEntry(Dns.GetHostName()).AddressList
                      where address.AddressFamily == AddressFamily.InterNetwork
                      select address).FirstOrDefault();

            this._machine = (ip ?? IPAddress.Loopback).ToString();
            this._port = 5050;
        }

        public WwtClient(string machine)
            : this(machine, 5050)
        {
        }

        public WwtClient(string machine, int port)
        {
            this._machine = machine;
            this._port = port;
        }

        /// <summary>
        /// Gets the details of the current view.
        /// </summary>
        /// <returns>The details of the current view.</returns>
        public ViewState GetViewState()
        {
            return new ViewState(this.GetState());
        }

        /// <summary>
        /// Sets the position of the view camera.
        /// </summary>
        /// <param name="latitude">Latitude positive to the North.</param>
        /// <param name="longitude">Longitude is in decimal degrees, positive to the East.</param>
        /// <param name="zoom">Zoom level varies from 360 (the most distant view) to 0.00023 (the closest view).</param>
        /// <param name="rotation">Rotation is in radians, positive moves the camera to the left.</param>
        /// <param name="angle">Angle is in radians, positive moves the camera forward.</param>
        /// <param name="smoothly">if set to <c>true</c> then camera should smoothly pan and zoom to the location.</param>
        public void FlyTo(double latitude, double longitude, double zoom, double rotation, double angle, bool smooth)
        {
            //  TODO:   Implement optional parameter : Earth, Moon, Mercury, Venus, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto, Io, Ganymede, Callisto, Europa, Sun, ISS
            this.Send(string.Format("/layerApi.aspx?cmd=version&flyto={0},{1},{2},{3},{4}&instant={5}", latitude, longitude, zoom, rotation, angle, !smooth));

        }

        /// <summary>
        /// Smoothly sets the position of the view camera.
        /// </summary>
        /// <param name="latitude">Latitude positive to the North.</param>
        /// <param name="longitude">Longitude is in decimal degrees, positive to the East.</param>
        /// <param name="zoom">Zoom level varies from 360 (the most distant view) to 0.00023 (the closest view).</param>
        /// <param name="rotation">Rotation is in radians, positive moves the camera to the left.</param>
        /// <param name="angle">Angle is in radians, positive moves the camera forward.</param>
        public void FlyTo(double latitude, double longitude, double zoom, double rotation, double angle)
        {
            this.FlyTo(latitude, longitude, zoom, rotation, angle, true);
        }

        #region API Commands

        /// <summary>
        /// Highlights the selected layer in the layer manager.
        /// </summary>
        /// <param name="layerId">The id.</param>
        internal void ActivateLayer(Guid layerId)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=activate&id={0}", layerId));
        }

        /// <summary>
        /// Specifies that a layer should be permanently deleted.
        /// </summary>
        /// <param name="layerId">The id.</param>
        internal void DeleteLayer(Guid layerId)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=delete&id={0}", layerId));
        }

        /// <summary>
        /// Gets the value of a single layer property.
        /// </summary>
        /// <param name="layerId">The layer id.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        internal string GetLayerPropertyValue(Guid layerId, string propertyName)
        {
            var response = this.Send(string.Format("/layerApi.aspx?cmd=getprop&id={0}&propname={1}", layerId, HttpUtility.UrlEncode(propertyName)));

            return response.Element("Layer").Attribute(propertyName).Value;
        }

        /// <summary>
        /// Gets all the properties for a specified layer.
        /// </summary>
        /// <param name="layerId">The layer id.</param>
        /// <returns></returns>
        internal XElement GetLayerProperties(Guid layerId)
        {
            var response = this.Send(string.Format("/layerApi.aspx?cmd=getprops&id={0}", layerId));

            return response.Element("Layer");
        }

        /// <summary>
        /// Adds group layer
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="frame">The parent frame name.</param>
        internal void CreateNewGroup(string name, string frame)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=group&frame={0}&name={1}", HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(frame)));
        }

        /// <summary>
        /// Gets the list of layers.
        /// </summary>
        /// <returns></returns>
        internal XElement GetAllFrames()
        {
            var response = this.Send("/layerApi.aspx?cmd=layerlist");

            return response.Element("LayerList");
        }

        /// <summary>
        /// Gets the list of layers.
        /// </summary>
        /// <param name="layersOnly">if set to <c>true</c> [layers only].</param>
        /// <returns></returns>
        internal XElement GetAllLayers()
        {
            var response = this.Send("/layerApi.aspx?cmd=layerlist&layersonly=True");

            return response.Element("LayerList");
        }

        /// <summary>
        /// Specifies a data file, and and some optional parameters, to apply to a new layer
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="name">The name.</param>
        /// <param name="color">The color.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="fadeType">Type of the fade.</param>
        /// <param name="fadeRange">The fade range.</param>
        /// <returns></returns>
        internal Guid Load(string frame, string filename, string name = "New Layer", Color color = default(Color), DateTime startDate = default(DateTime), DateTime endDate = default(DateTime), FadeType fadeType = FadeType.None, int fadeRange = 0)
        {
            //  TODO:   fix default values for color and date time
            var response = this.Send(string.Format("/layerApi.aspx?cmd=load&frame={0}&filename={1}&name={2}&color={3}&startdate={4}&enddate={5}&fadetype={6}&faderange{7}", frame, filename, name, color, startDate, endDate, fadeType, fadeRange));

            var xlayerId = response.Descendants("NewLayerID").FirstOrDefault();

            return new Guid(xlayerId.Value);
        }

        /// <summary>
        /// Changes the view to one of Earth, Planet, Sky, Panorama, SolarSystem.
        /// </summary>
        /// <param name="mode">The name.</param>
        internal void ChangeMode(ViewMode mode)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=mode&lookat={0}", mode));
        }

        /// <summary>
        /// Changes the view depending on the supplied parameter.
        /// </summary>
        /// <param name="moveMode">The move mode.</param>
        internal void ChangeView(MoveMode moveMode)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=move&move={0}", moveMode));
        }

        /// <summary>
        /// Specifies that a new layer should be created.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="name">The name.</param>
        /// <param name="color">The color.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="fadeType">Type of the fade.</param>
        /// <param name="fadeRange">The fade range.</param>
        /// <returns></returns>
        internal Guid CreateNewLayer(string frame, string name, Color color, DateTime startDate, DateTime endDate, FadeType fadeType, int fadeRange, string[] columns)
        {
            //  TODO:   fix default values for color and date time
            //  TODO:   Add ability to set column headers
            //var response = this.Send(string.Format("/layerApi.aspx?cmd=new&frame={0}&name={1}&color={2}&startdate={3}&enddate={4}&fadetype={5}&faderange={6}", frame, name, color.ToWwtColor(), startDate, endDate, fadeType, fadeRange), string.Join("\t", columns));
            var response = this.Send(string.Format("/layerApi.aspx?cmd=new&frame={0}&name={1}&color={2}&startdate={3}&enddate={4}&fadetype={5}&faderange={6}", HttpUtility.UrlEncode(frame), HttpUtility.UrlEncode(name), color.ToWwtColor(), startDate, endDate, fadeType, fadeRange));

            var xlayerId = response.Descendants("NewLayerID").FirstOrDefault();

            return new Guid(xlayerId.Value);
        }

        internal Guid CreateNewLayer(string frame, string name)
        {          
            var response = this.Send(string.Format("/layerApi.aspx?cmd=new&frame={0}&name={1}", HttpUtility.UrlEncode(frame), HttpUtility.UrlEncode(name)));

            var xlayerId = response.Descendants("NewLayerID").FirstOrDefault();

            return new Guid(xlayerId.Value);
        }

        /// <summary>
        /// Sets the value of a single layer property.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        internal void SetLayerPropertyValue(Guid id, string name, string value)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=setprop&id={0}&propname={1}&propvalue={2}", id, HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(value)));
        }

        /// <summary>
        /// Sets the values of the layer properties
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="propertyValues">The property values.</param>
        internal void SetLayerPropertyValues(Guid id, IDictionary<string, string> propertyValues)
        {
            var xmlRoot = new XDocument(new XElement("LayerApi",
                new XElement("Layer",
                    from propVal in propertyValues
                    select new XAttribute(propVal.Key, propVal.Value))
                    ));

            this.Send(string.Format("/layerApi.aspx?cmd=setprops&id={0}", id), xmlRoot.ToString());
        }

        /// <summary>
        /// Gets some details of the current view.
        /// </summary>
        /// <returns></returns>
        internal XElement GetState()
        {
            var response = this.Send("/layerApi.aspx?cmd=state");
            return response.Element("ViewState");
        }

        /// <summary>
        /// Changes user interface settings, without altering the layer data.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        internal void UpdateUISettings(string name, string value)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=uisettings&{0}={1}", HttpUtility.UrlEncode(name), HttpUtility.UrlEncode(value)));
        }

        /// <summary>
        /// Updates the specified id.
        /// </summary>
        /// <param name="id">Specifies the id number of the layer.</param>
        /// <param name="hasheader">if set to <c>true</c> the data has a header row.</param>
        /// <param name="name">A friendly name to rename the layer.</param>
        /// <param name="nopurge">if set to <c>true</c> the event data should not be deleted.</param>
        /// <param name="purgeall">if set to <c>true</c> delete all events.</param>
        /// <param name="show">if set to <c>true</c> to show the layer.</param>
        internal void UpdateLayerData(Guid id, bool hasheader, string name, bool nopurge, bool purgeall, bool show, string data)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=update&id={0}&hasheader={1}&name={2}&nopurge={3}&purgeall={4}&show={5}", id, hasheader, HttpUtility.UrlEncode(name), nopurge, purgeall, show), data);
        }

        internal void AddLayerData(Guid id, string data)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=update&id={0}&nopurge=true", id), data);
        }

        internal void ClearLayerData(Guid id)
        {
            this.Send(string.Format("/layerApi.aspx?cmd=update&id={0}&purgeall=true", id));
        }

        /// <summary>
        /// Gets the version number of the running version of the LCAPI.
        /// </summary>
        /// <returns></returns>
        internal string GetVersion()
        {
            var response = this.Send("/layerApi.aspx?cmd=version");

            var version = response.Descendants("Version").FirstOrDefault();

            if (version == null)
            {
                throw new WwtException("Not valid response. 'Version' node is expected.");
            }
            else
            {
                return version.Value;
            }
        }

        #endregion

        private XElement Send(string commandUrl)
        {
            return this.Send(commandUrl, string.Empty);
        }

        private XElement Send(string commandUrl, string payload)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    var urlRequest = string.Format("http://{0}:{1}{2}", this._machine, this._port, commandUrl);

                    Debug.WriteLine(string.Format("Request URL: {0}", urlRequest));
                    if (!string.IsNullOrEmpty(payload))
                        Debug.WriteLine(string.Format("Request payload: {0}", payload));

                    var response = client.UploadString(urlRequest, payload).TrimEnd('\x0');

                    if (string.IsNullOrEmpty(response))
                    {
                        return null;
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Response xml: {0}", response));

                        //  TODO:   Check what exception thrown when xml is invalid
                        var xdoc = XDocument.Parse(response);

                        var layerApi = xdoc.Elements("LayerApi").FirstOrDefault();

                        var status = layerApi.Elements("Status").FirstOrDefault();
                        if (status != null)
                        {
                            if (!status.Value.Equals("Success"))
                            {
                                throw new WwtException("API request failed.");
                            }
                        }

                        return layerApi;
                    }
                }
                catch (XmlException exception)
                {
                    throw new Exception("An error has occurred while performing the current operation. Please try again.", exception);
                }
                catch (WebException exception)
                {
                    throw new Exception("WorldWide Telescope (WWT) needs to be open to perform this operation. Please open WWT and try again.", exception);
                }
            }
        }

        //Optionally there can be a sixth parameter containing the frame to change the view to, which can be one of:
        //Earth, Moon, Mercury, Venus, Mars, Jupiter, Saturn, Uranus, Neptune, Pluto, Io, Ganymede, Callisto, Europa, Sun, ISS.
        //&autoloop	True sets the layer manager to auto loop.
        
        public static void Export(IEnumerable<DataItem> dataItems, Stream output)
        {
            using (var sw = new StreamWriter(output))
            {
                var header = false;
                foreach (var item in dataItems)
                {
                    if (!header)
                    {
                        item.WriteHeaderTo(sw);
                        header = true;
                    }
                    item.WriteDataTo(sw);
                }
            }
        }

        public IEnumerable<T> Import<T>(Stream input) where T : DataItem, new()
        {
            var dataItems = new List<T>();
            var header = false;
            using (var sr = new StreamReader(input))
            {
                var item = new T();
                if (!header)
                {
                    item.ReadHeaderFrom(sr);
                    header = true;
                }
                item.ReadDataFrom(sr);
                dataItems.Add(item);
            }

            return dataItems;
        }
    }
}
