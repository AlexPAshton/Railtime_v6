using System;
using System.Collections.Generic;
using System.Linq;

namespace Railtime_v6
{
    public class RtStations
    {     
        private const int ZEROLENGTH = 0;
        private const int THREELENGTH = 3;
        private static readonly string[] NAVIGABLESTATIONS = { "LAN", "PRE" };

        //Main thing
        public RtStationData[] SearchStations(string SearchQuery, int MaxResults, RtGPS SortDistance, bool NavigableOnly)
        {
            //Create empty return variable
            List<RtStationData> StationResults = new List<RtStationData>();

            //Check input parameter is valid
            if (SearchQuery.Length != ZEROLENGTH)
            {
                //Check if input is three capital letters
                if (SearchQuery.Length == THREELENGTH && SearchQuery.ToUpper() == SearchQuery)
                {
                    //Search by CRS Code
                    StationResults = RtStations.SearchByCRS(SearchQuery).ToList();
                }
                else
                {
                    //Search by Name
                    StationResults = RtStations.SearchByName(SearchQuery, MaxResults).ToList();
                }
            }

            //If Navigable Only, filter
            if (NavigableOnly)
            {
                //Loop through results, if not navigable, remove and minus iterator
                for (int i = 0; i < StationResults.Count; i++)
                {
                    if (!NAVIGABLESTATIONS.Contains(StationResults[i].Code))
                    {
                        StationResults.Remove(StationResults[i]);
                        i--;
                    }
                }
            }

            if (SortDistance != null)
            {
                //Create GPS Instance in another thread.
                for (int z = 0; z < StationResults.Count - 1; z++)
                {
                    for (int i = 0; i < StationResults.Count - 1; i++)
                    {
                        if (SortDistance.DistanceFromLatLonInKm(StationResults[i + 1].Latitude, StationResults[i + 1].Longitude, SortDistance.Latitude, SortDistance.Longitude) <
                            SortDistance.DistanceFromLatLonInKm(StationResults[i].Latitude, StationResults[i].Longitude, SortDistance.Latitude, SortDistance.Longitude))
                        {
                            RtStationData tmp = StationResults[i];
                            StationResults[i] = StationResults[i + 1];
                            StationResults[i + 1] = tmp;
                        }
                    }
                }
            }

            //SearchQuery Invalid
            return StationResults.ToArray();
        }

        //Return array of all stations data that contain station name
        public static RtStationData[] SearchByName(string StationName, int MaxResults = 10)
        {
            List<RtStationData> SearchData = new List<RtStationData>();
            int StartIndex = -1;
            int EndIndex = -1;

            if (StationName.Length > 0)
            {
                //Get index's to search between from indexer
                for (int i = 0; i < RtStationsData.StationDatasAlphabeticIndexs.Length; i++)
                {
                    if (RtStationsData.StationDatasAlphabeticIndexs[i].Letter == StationName[0])
                    {
                        StartIndex = RtStationsData.StationDatasAlphabeticIndexs[i].StartingIndex;
                        EndIndex = RtStationsData.StationDatasAlphabeticIndexs[i].EndingIndex;
                    }
                }

                for (int i = StartIndex; i < EndIndex; i++)
                {
                    if (RtStationsData.RtStationDatas[i].StationName.Contains(StationName))
                    {
                        SearchData.Add(RtStationsData.RtStationDatas[i]);

                        if (SearchData.Count == MaxResults)
                            break;
                    }
                }
            }

            return SearchData.ToArray();
        }

        //Return array of all stations data that equal crs code
        public static RtStationData[] SearchByCRS(string StationCode)
        {
            List<RtStationData> SearchData = new List<RtStationData>();

            for (int i = 0; i < RtStationsData.RtStationDatas.Length; i++)
            {
                if (RtStationsData.RtStationDatas[i].Code == StationCode)
                    SearchData.Add(RtStationsData.RtStationDatas[i]);
            }

            return SearchData.ToArray();
        }

    }
}