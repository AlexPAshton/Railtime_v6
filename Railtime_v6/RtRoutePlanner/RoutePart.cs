using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;

namespace RtRoutePlanner
{
    public class RoutePart
    {
        private const int UPDATEPOLLINTERVAL = 60000;

        public enum RoutePartStatus
        {
            ONTIME, LATE
        }

        public string DepartureTime = "null";
        public string DepartureStationName = "null";
        public string DepartureStationCode = "null";
        public string DeparturePlatform = "null";

        public string ArrivalTime = "null";
        public string ArrivalStationName = "null";
        public string ArrivalStationCode = "null";
        public string ArrivalPlatform = "null";

        //Gets platform information from departures and arrivals pages. If theres no value, keep the previous.
        public RoutePart() => new Thread(() => UpdateStationPlatforms()).Start();

        public void UpdateStationPlatforms()
        {
            while (true)
            {
                //Get departing platform.
                string DownloadDepData = new WebClient().DownloadString("http://ojp.nationalrail.co.uk/service/ldbboard/dep/" + DepartureStationCode + "/" + ArrivalStationCode + "/To");

                //Get table
                string DepData = DownloadDepData.NullSplit("<tbody>", 1).NullSplit("</tbody>", 0);

                //Get table rows array
                string[] DepRowsData = DepData.Split(new string[] { "<tr class=\"" }, 0);

                //Search rows for this departure time
                for (int r = 0; r < DepRowsData.Length; r++)
                {
                    if (DepRowsData[r].Contains(">" + DepartureTime + "<"))
                    {
                        //This row is this departure. Get platform
                        string[] tdbits = DepRowsData[r].Split(new string[] { "<td" }, 0);

                        string oldDeparturePlatform = DeparturePlatform;
                        DeparturePlatform = tdbits[4].NullSplit("</td>", 0).Replace(">", "");

                        if (DeparturePlatform == "null" || DeparturePlatform == "")
                            DeparturePlatform = oldDeparturePlatform;
                    }
                }

                //Get arrival platform.
                string DownloadArrData = new WebClient().DownloadString("http://ojp.nationalrail.co.uk/service/ldbboard/arr/" + ArrivalStationCode + "/" + DepartureStationCode + "/From");

                //Get table
                string ArrData = DownloadArrData.NullSplit("<tbody>", 1).NullSplit("</tbody>", 0);

                //Get table rows array
                string[] ArrRowsData = ArrData.Split(new string[] { "<tr class=\"" }, 0);

                //Search rows for this departure time
                for (int r = 0; r < ArrRowsData.Length; r++)
                {
                    if (ArrRowsData[r].Contains(">" + ArrivalTime + "<"))
                    {
                        //This row is this departure. Get platform
                        string[] tdbits = ArrRowsData[r].Split(new string[] { "<td" }, 0);

                        string oldArrivalPlatform = ArrivalPlatform;
                        ArrivalPlatform = tdbits[4].NullSplit("</td>", 0).Replace(">", "");

                        if (ArrivalPlatform == "null" || ArrivalPlatform == "")
                            ArrivalPlatform = oldArrivalPlatform;
                    }
                }

                Thread.Sleep(UPDATEPOLLINTERVAL);
            }
        }
    }
}
