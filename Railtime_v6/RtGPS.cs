using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Railtime_v6
{
    //GPS Class for getting GPS info and doing distance calculations
    public class RtGPS : Activity, ILocationListener
    {
        //Private Variables
        private string LocationProvider = "";
        private double _Latitude;
        private double _Longitude;
        private bool _Ready;
        private bool _NoGPS;

        //Sorta initialiser
        public RtGPS(LocationManager LocationManager)
        {
            Criteria LocationServiceCriteria = new Criteria { Accuracy = Accuracy.Fine };
            IList<string> LocationProviders = LocationManager.GetProviders(LocationServiceCriteria, true);

            if (LocationProviders.Any())
            {
                LocationProvider = LocationProviders.First();

                LocationManager.RequestLocationUpdates(LocationProvider, 0, 0, this);
            }
            else
            {
                _NoGPS = true;
            }
        }

        //Getters
        public double Latitude
        {
            get { return _Latitude; }
        }

        public double Longitude
        {
            get { return _Longitude; }
        }

        public bool Ready
        {
            get { return _Ready; }
        }

        public bool NoGPS
        {
            get { return _NoGPS; }
        }

        //Location Change Event
        public void OnLocationChanged(Location Location)
        {
            _Latitude = Location.Latitude;
            _Longitude = Location.Longitude;
            _Ready = true;
        }

        //Lat Lon Distance
        public double DistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        public double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
    }
}