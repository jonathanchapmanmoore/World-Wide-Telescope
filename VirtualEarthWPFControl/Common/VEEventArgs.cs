using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarthWPFControl
{
    public class VEEventArgs : EventArgs
    {
        public string Error;
        public int ZoomLevel;
        public string MapStyle;
        public string BirdseyeSceneID;
        public string BirdseyeSceneOrientation;
        public Boolean LeftMouseButton;
        public Boolean RightMouseButton;
        public Boolean MiddleMouseButton;
        public int MouseWheelChange;

        public int ClientX;
        public int ClientY;

        public int ScreenX;
        public int ScreenY;
        public int MapX;
        public int MapY;

        public VELatLong LatLong;
        public int KeyCode;
        public Boolean AltKey;
        public Boolean CtrlKey;
        public Boolean ShiftKey;
        public string EventName;
        public string ElementID;
    }
}
