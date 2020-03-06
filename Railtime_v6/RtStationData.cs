using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Railtime_v6
{
    //Custom Data Type Class for Stations
    public class RtStationData
    {
        private string _StationName;//Name of station
        private string _Code;//Code for station e.g. LAN
        private double _Easting;//Easting for station
        private double _Northing;//Northing for station
        private double _Latitude;//Latitude for station
        private double _Longitude;//Longitude for station

        //Set and Get for StationName
        public string StationName { get { return _StationName; } set { _StationName = value; } }

        //Set and Get for Code
        public string Code { get { return _Code; } set { _Code = value; } }

        //Set and Get for Easting
        public double Easting { get { return _Easting; } set { _Easting = value; } }

        //Set and Get for Northing
        public double Northing { get { return _Northing; } set { _Northing = value; } }

        //Set and Get for Latitude
        public double Latitude { get { return _Latitude; } set { _Latitude = value; } }

        //Set and Get for Longitude
        public double Longitude { get { return _Longitude; } set { _Longitude = value; } }

        //Initialiser
        public RtStationData (string StationName, string Code, double Easting, double Northing, LatLon LatLon)
        {
            this._StationName = StationName;
            this._Code = Code;
            this._Easting = Easting;
            this._Northing = Northing;
            this._Latitude = LatLon.Latitude;
            this._Longitude = LatLon.Longitude;
        }
    }
}