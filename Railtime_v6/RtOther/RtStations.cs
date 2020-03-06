using System;
using System.Collections.Generic;
using System.Linq;

namespace Railtime_v6
{
    public class RtStations
    {
        private const int ONEOFFSET = 1;
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
                //Check if input is three letters
                if (SearchQuery.Length == THREELENGTH)
                {
                    //Search by CRS Code
                    StationResults = SearchByCRS(SearchQuery.ToUpper()).ToList();
                }
                else
                {
                    //Search by Name
                    StationResults = SearchByName(SearchQuery, MaxResults).ToList();
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
                    for (int z = 0; z < StationResults.Count - ONEOFFSET; z++)
                    {
                        for (int i = 0; i < StationResults.Count - ONEOFFSET; i++)
                        {
                            if (SortDistance.DistanceFromLatLonInKm(StationResults[i + ONEOFFSET].Latitude, StationResults[i + ONEOFFSET].Longitude, SortDistance.Latitude, SortDistance.Longitude) <
                                SortDistance.DistanceFromLatLonInKm(StationResults[i].Latitude, StationResults[i].Longitude, SortDistance.Latitude, SortDistance.Longitude))
                            {
                                RtStationData tmp = StationResults[i];
                                StationResults[i] = StationResults[i + ONEOFFSET];
                                StationResults[i + ONEOFFSET] = tmp;
                            }
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
            string SearchQueryUpper = StationName.ToUpper();

            if (StationName.Length > ZEROLENGTH)
            {
                //Get index's to search between from indexer
                RtStationsData.StationDatasAlphabeticIndex Dat = RtStationsData.GetSearchIndexs(SearchQueryUpper[ZEROLENGTH]);

                for (int i = Dat.StartingIndex; i < Dat.EndingIndex; i++)
                {
                    if (RtStationsData.RtStationDatas[i].StationNameUpper.Contains(SearchQueryUpper))
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

            for (int i = ZEROLENGTH; i < RtStationsData.RtStationDatas.Length; i++)
            {
                if (RtStationsData.RtStationDatas[i].Code == StationCode)
                    SearchData.Add(RtStationsData.RtStationDatas[i]);
            }

            return SearchData.ToArray();
        }

    }
}