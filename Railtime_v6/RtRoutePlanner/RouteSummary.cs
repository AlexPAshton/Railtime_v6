using System;
using System.Collections.Generic;
using System.Text;

namespace RtRoutePlanner
{
    //Jourey Summary Class, stores overall journey variables
    public class RouteSummary
    {
        public string DepartureTime = "null";
        public string DepartureStationName = "null";
        public string DepartureStationCode = "null";
        public string DepartureStationPlat = "null";

        public string ArrivalTime = "null";
        public string ArrivalStationName = "null";
        public string ArrivalStationCode = "null";
        public string ArrivalStationPlat = "null";

        public string DurationHrs = "null";
        public string DurationMins = "null";
        public string Changes = "null";
        public string Status = "null";

        public RoutePart[] JourneyParts = null;
    }
}
