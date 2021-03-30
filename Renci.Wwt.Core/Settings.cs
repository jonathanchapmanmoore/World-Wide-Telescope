using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Renci.Wwt.Core
{
    public class Settings
    {
        public bool AutoHideContext { get; set; }
        public bool AutoHideTabs { get; set; }
        public bool AutoRepeatTour { get; set; }
        public bool AutoRepeatTourAll { get; set; }
        public Color ConstellationBoundryColor { get; set; }
        public Color ConstellationFigureColor { get; set; }
        public string ConstellationFiguresFile { get; set; }
        public Color ConstellationSelectionColor { get; set; }
        public string ContextSearchFilter { get; set; }
        public double DomeTilt { get; set; }
        public int DomeTypeIndex { get; set; }
        public bool DomeView { get; set; }
        public Color EclipticColor { get; set; }
        public bool FollowMouseOnZoom { get; set; }
        public int FovCamera { get; set; }
        public Color FovColor { get; set; }
        public int FovEyepiece { get; set; }
        public int FovTelescope { get; set; }
        public bool FullScreenTours { get; set; }
        public Color GridColor { get; set; }
        public int ImageQuality { get; set; }
        public bool LargeDomeTextures { get; set; }
        public int LastLookAtMode { get; set; }
        public bool LineSmoothing { get; set; }
        public bool ListenMode { get; set; }
        public bool LocalHorizonMode { get; set; }
        public double LocationAltitude { get; set; }
        public double LocationLat { get; set; }
        public double LocationLng { get; set; }
        public string LocationName { get; set; }
        public bool MasterController { get; set; }
        public bool ShowClouds { get; set; }
        public bool ShowConstellationBoundries { get; set; }
        public bool ShowConstellationFigures { get; set; }
        public bool ShowConstellationNames { get; set; }
        public bool ShowConstellationSelection { get; set; }
        public bool ShowCrosshairs { get; set; }
        public bool ShowDatasetNames { get; set; }
        public bool ShowEarthSky { get; set; }
        public bool ShowEcliptic { get; set; }
        public bool ShowElevationModel { get; set; }
        public bool ShowFieldOfView { get; set; }
        public bool ShowGrid { get; set; }
        public bool ShowSolarSystem { get; set; }
        public bool ShowTouchControls { get; set; }
        public bool ShowUTCTime { get; set; }
        public bool SolarSystemCosmos { get; set; }
        public bool SolarSystemLighting { get; set; }
        public bool SolarSystemMilkyWay { get; set; }
        public bool SolarSystemMinorOrbits { get; set; }
        public bool SolarSystemMinorPlanets { get; set; }
        public bool SolarSystemMultiRes { get; set; }
        public Color SolarSystemOrbitColor { get; set; }
        public bool SolarSystemOrbits { get; set; }
        public bool SolarSystemOverlays { get; set; }
        public bool SolarSystemPlanets { get; set; }
        public bool SolarSystemScale { get; set; }
        public bool SolarSystemStars { get; set; }
        public int StartUpLookAt { get; set; }
    }
}
