using System.Runtime.InteropServices;
using System.Text;
using System;

namespace VirtualEarthWPFControl
{
    [ComVisible(true)]
    public class WindowExternalHelper
    {
        VEMap map; //holds the current VEMap instance
        public WindowExternalHelper(VEMap map)
        {
            this.map = map;
        }

        /// Event Handling: Junky, junky code...
        ////////////////////////////// Virtual Earth Events ///////////////////////////////
        public void OnMapLoad()
        {
            this.map.loaded = true;
            this.map._MapLoad(new VEEventArgs());
        }

        public void OnChangeMapStyle(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._ChangeMapStyle(args);
        }

        public void OnChangeView(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._ChangeView(args);
        }

        public void OnEndPan(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._EndPan(args);
        }

        public void OnEndZoom(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._EndZoom(args);
        }

        public void OnInitMode(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._InitMode(args);
        }

        public void OnModeNotAvailable(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._ModeNotAvailable(args);
        }

        public void OnResize(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._Resize(args);
        }

        public void OnStartPan(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._StartPan(args);
        }

        public void OnStartZoom(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = encodeVirtualEarthEventArgs(eventName, mapStyle, sceneID, sceneOrientation, zoomLevel);
            this.map._StartZoom(args);
        }

        //////////////////////////////     Mouse Events     ///////////////////////////////

        public void OnClick(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._Click(args);
        }

        public void OnDoubleClick(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._DoubleClick(args);
        }

        public void OnMouseMove(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._MouseMove(args);
        }

        public void OnMouseDown(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._MouseDown(args);
        }

        public void OnMouseUp(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._MouseUp(args);
        }

        public void OnMouseOver(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._MouseOver(args);
        }

        public void OnMouseOut(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._MouseOut(args);
        }

        public void OnMouseWheel(string altKey, string ctrlKey, string elementID, string eventName, string leftMouseButton,
                            string rightMouseButton, string mapStyle, string sceneID, string sceneOrientation, string shiftKey, string clientX,
                            string clientY, string screenX, string screenY, string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = encodeMouseEventArgs(altKey, ctrlKey, elementID, eventName, leftMouseButton, rightMouseButton, mapStyle, sceneID, sceneOrientation, shiftKey, clientX, clientY, screenX, screenY, mapX, mapY, zoomLevel);
            this.map._MouseWheel(args);
        }

        //////////////////////////////   Keyboard Events     ///////////////////////////////
        public void OnKeyPress(string altKey, string ctrlKey, string eventName, string keyCode,
                               string mapStyle, string sceneID, string sceneOrientation,
                               string shiftKey, string zoomLevel)
        {
            VEEventArgs args = encodeKeyboardEventArgs(altKey, ctrlKey, eventName, keyCode, mapStyle, sceneID, sceneOrientation, shiftKey, zoomLevel);
            this.map._KeyPress(args);
        }

        public void OnKeyDown(string altKey, string ctrlKey, string eventName, string keyCode,
                               string mapStyle, string sceneID, string sceneOrientation,
                               string shiftKey, string zoomLevel)
        {
            VEEventArgs args = encodeKeyboardEventArgs(altKey, ctrlKey, eventName, keyCode, mapStyle, sceneID, sceneOrientation, shiftKey, zoomLevel);
            this.map._KeyDown(args);
        }

        public void OnKeyUp(string altKey, string ctrlKey, string eventName, string keyCode,
                       string mapStyle, string sceneID, string sceneOrientation,
                       string shiftKey, string zoomLevel)
        {
            VEEventArgs args = encodeKeyboardEventArgs(altKey, ctrlKey, eventName, keyCode, mapStyle, sceneID, sceneOrientation, shiftKey, zoomLevel);
            this.map._KeyUp(args);
        }

        ////////////////////////////// Exception Handling /////////////////////////////////

        public void ThrowWindowError(string message, string uri, string line)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("MAP WINDOW ERROR\n\n");
            sb.Append("message: ");
            sb.Append(message);
            sb.Append("\n");
            sb.Append("uri: ");
            sb.Append(uri);
            sb.Append("line: ");
            sb.Append("\n");
            sb.Append(line);

            this.map.ThrowException(new Exception(sb.ToString()));
        }

        public void ThrowMapError(string error, string eventName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("MAP ERROR\n\n");
            sb.Append("error: ");
            sb.Append(error);
            sb.Append("\n");
            sb.Append("eventName: ");
            sb.Append(eventName);

            this.map.ThrowException(new Exception(sb.ToString()));
        }

        public void ThrowVEException(string source, string name, string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("MAP VEEXCEPTION\n\n");
            sb.Append("source: ");
            sb.Append(source);
            sb.Append("\n");
            sb.Append("name: ");
            sb.Append(name);
            sb.Append("message: ");
            sb.Append("\n");
            sb.Append(message);

            this.map.ThrowException(new Exception(sb.ToString()));
        }

        // Virtual Earth Event Arguments Initializer...
        private VEEventArgs encodeVirtualEarthEventArgs(string eventName, string mapStyle, string sceneID, string sceneOrientation, string zoomLevel)
        {
            VEEventArgs args = new VEEventArgs();
            args.EventName = eventName;
            args.MapStyle = mapStyle;
            args.ZoomLevel = Convert.ToInt32(zoomLevel);

            return args;
        }

        // Mouse Event Arguments...
        private VEEventArgs encodeMouseEventArgs(string altKey, string ctrlKey, string elementID,
                                                string eventName, string leftMouseButton,
                                                string rightMouseButton, string mapStyle, string sceneID,
                                                string sceneOrientation, string shiftKey, string clientX,
                                                string clientY, string screenX, string screenY,
                                                string mapX, string mapY, string zoomLevel)
        {
            VEEventArgs args = new VEEventArgs();
            args.AltKey = Convert.ToBoolean(altKey);
            args.CtrlKey = Convert.ToBoolean(ctrlKey);
            args.ElementID = elementID; args.EventName = eventName;

            args.LeftMouseButton = Convert.ToBoolean(leftMouseButton);
            args.RightMouseButton = Convert.ToBoolean(rightMouseButton);
            args.MapStyle = mapStyle;
            args.ShiftKey = Convert.ToBoolean(shiftKey);
            args.ClientX = Convert.ToInt32(clientX); args.ClientY = Convert.ToInt32(clientY);
            args.ScreenX = Convert.ToInt32(screenX);
            args.ScreenY = Convert.ToInt32(screenY);
            args.MapX = Convert.ToInt32(mapX); args.MapY = Convert.ToInt32(mapY);
            args.ZoomLevel = Convert.ToInt32(zoomLevel);

            return args;
        }

        // Keyboard Events Arguments
        private VEEventArgs encodeKeyboardEventArgs(string altKey, string ctrlKey, string eventName, string keyCode,
                               string mapStyle, string sceneID, string sceneOrientation,
                               string shiftKey, string zoomLevel)
        {
            VEEventArgs args = new VEEventArgs();
            args.AltKey = Convert.ToBoolean(altKey);
            args.CtrlKey = Convert.ToBoolean(ctrlKey);
            args.EventName = eventName;
            args.KeyCode = Convert.ToInt32(keyCode);
            args.MapStyle = mapStyle;
            args.ShiftKey = Convert.ToBoolean(shiftKey);
            args.ZoomLevel = Convert.ToInt32(zoomLevel);

            return args;
        }
    }
}
