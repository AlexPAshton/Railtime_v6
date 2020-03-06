using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RtRoutePlanner
{
    //Jourey Class, contains journey route functions
    public class RoutePlanner
    {
        //Constants
        public enum RequestStatus
        {
            OK, EMPTY, ERROR
        }

        readonly string[] DEPARTURESSPLITTAGS = { "<tr class=\"first mtx\">", "<tr class=\" alt mtx\">", "<tr class=\" mtx\">", "<tr class=\"last mtx\">" };
        const string NATIONALRAILURL0 = "http://ojp.nationalrail.co.uk/service/timesandfares/";
        const string NATIONALRAILURL1 = "/";
        const string NATIONALRAILURL2 = "/today/";
        const string NATIONALRAILURL3 = "/dep";
        const string TWODIGITFORMAT = "00";
        const int ZERO = 0;

        //Get route info function. Taking from and to crs codes and getting data
        //then parsing that into objects by screen scraping.
        //Status can be OKAY, EMPTY or ERROR
        public RouteSummary[] GetRouteInfo(string From_StationName, string To_StationName, out RequestStatus Status)
        {
            try
            {
                string DownloadData = new WebClient().DownloadString(GetRouteDataURL(From_StationName, To_StationName));
                string DeparturesData = DownloadData
                    .NullSplit("<table cellspacing=\"0\" cellpadding=\"0\" id=\"oft\">", 1)
                    .NullSplit("<div class=\"ctf-bar after\">", 0)
                    .Replace("\n", "")
                    .Replace("\t", "");

                string[] DeparturesBits = SplitSourceAtTags(DeparturesData, DEPARTURESSPLITTAGS);

                List<RouteSummary> Joruneys = new List<RouteSummary>();

                for (int i = 1; i < DeparturesBits.Length; i++)
                {
                    //Get journey info
                    RouteSummary NewJoruney = new RouteSummary();
                    NewJoruney.DepartureTime = DeparturesBits[i]
                        .NullSplit("<td class=\"dep\">", 1)
                        .NullSplit("</td>", 0);

                    NewJoruney.DepartureStationName = DeparturesBits[i]
                        .NullSplit("<td class=\"from\">", 1)
                        .NullSplit("</abbr>]", 0)
                        .NullSplit("  [<abbr>", 0);

                    NewJoruney.DepartureStationCode = DeparturesBits[i]
                        .NullSplit("<td class=\"from\">", 1)
                        .NullSplit("</abbr>]", 0)
                        .NullSplit("  [<abbr>", 1);

                    NewJoruney.DepartureStationPlat = DeparturesBits[i]
                        .NullSplit("Departs from Platform ", 1)
                        .NullSplit("\">", 0);

                    NewJoruney.ArrivalStationName = DeparturesBits[i]
                        .NullSplit("<td class=\"to\"", 1)
                        .NullSplit("</abbr>]", 0)
                        .NullSplit("/>", 1)
                        .NullSplit("  [<abbr>", 0);

                    NewJoruney.ArrivalStationCode = DeparturesBits[i]
                        .NullSplit("<td class=\"to\"", 1)
                        .NullSplit("</abbr>]", 0)
                        .NullSplit("/>", 1)
                        .NullSplit("  [<abbr>", 1);

                    NewJoruney.ArrivalStationPlat = DeparturesBits[i]
                       .NullSplit("Arrives at Platform ", 1)
                       .NullSplit("\">", 0);

                    NewJoruney.ArrivalTime = DeparturesBits[i]
                        .NullSplit("<td class=\"arr\">", 1)
                        .NullSplit("</td>", 0);

                    NewJoruney.DurationHrs = DeparturesBits[i]
                        .NullSplit("<td class=\"dur\">", 1)
                        .NullSplit("<abbr", 0);

                    NewJoruney.DurationMins = DeparturesBits[i]
                        .NullSplit("<td class=\"dur\">", 1)
                        .NullSplit(">h</abbr>", 1).NullSplit("<abbr", 0);

                    NewJoruney.Changes = DeparturesBits[i]
                        .NullSplit("<td class=\"chg\"><div class=\"changestip tooltip\"><a href=\"#\" class=\"changestip-link\">", 1)
                        .NullSplit("</a>", 0);

                    //If no minutes, the minutes has been inserted into hours, so switch them...
                    if (NewJoruney.DurationMins == "null")
                    {
                        NewJoruney.DurationMins = NewJoruney.DurationHrs;
                        NewJoruney.DurationHrs = "null";
                    }

                    //Get changes info
                    if (NewJoruney.Changes != "null")
                    {
                        //Get changes table
                        string[] ChangesData = DeparturesBits[i]
                            .NullSplit("<tbody>", 1)
                            .NullSplit("</tbody>", 0)
                            .Split(new string[] { "<tr class" }, 0);

                        RoutePart[] Parts = new RoutePart[ChangesData.Length - 1];

                        for (int p = 1; p < ChangesData.Length; p++)
                        {
                            Parts[p - 1] = new RoutePart();

                            Parts[p - 1].DepartureTime = ChangesData[p]
                                .NullSplit("<td>", 2)
                                .NullSplit("</td>", 0);

                            Parts[p - 1].DepartureStationName = ChangesData[p]
                                .NullSplit("<td class=\"origin\">", 1)
                                .NullSplit("</abbr>", 0)
                                .NullSplit(" [<abbr>", 0);

                            Parts[p - 1].DepartureStationCode = ChangesData[p]
                                .NullSplit("<td class=\"origin\">", 1)
                                .NullSplit("</abbr>", 0)
                                .NullSplit(" [<abbr>", 1);

                            Parts[p - 1].ArrivalTime = ChangesData[p]
                                .NullSplit("<td>", 3)
                                .NullSplit("</td>", 0);

                            Parts[p - 1].ArrivalStationName = ChangesData[p]
                                .NullSplit("<td class=\"destination\">", 1)
                                .NullSplit("/>", 1)
                                .NullSplit("</abbr>", 0)
                                .NullSplit(" [<abbr>", 0);

                            Parts[p - 1].ArrivalStationCode = ChangesData[p]
                                .NullSplit("<td class=\"destination\">", 1)
                                .NullSplit("/>", 1)
                                .NullSplit("</abbr>", 0)
                                .NullSplit(" [<abbr>", 1);
                        }

                        NewJoruney.JourneyParts = Parts;
                    }
                    else
                    {
                        //No changes
                        NewJoruney.JourneyParts = new RoutePart[1];
                        NewJoruney.JourneyParts[0] = new RoutePart();

                        NewJoruney.JourneyParts[0].DepartureTime = NewJoruney.DepartureTime;
                        NewJoruney.JourneyParts[0].DepartureStationName = NewJoruney.DepartureStationName;
                        NewJoruney.JourneyParts[0].DepartureStationCode = NewJoruney.DepartureStationCode;
                        NewJoruney.JourneyParts[0].ArrivalTime = NewJoruney.ArrivalTime;
                        NewJoruney.JourneyParts[0].ArrivalStationName = NewJoruney.ArrivalStationName;
                        NewJoruney.JourneyParts[0].ArrivalStationCode = NewJoruney.ArrivalStationCode;
                    }

                    Joruneys.Add(NewJoruney);
                }

                if (Joruneys.Count == ZERO)
                    Status = RequestStatus.EMPTY;
                else
                    Status = RequestStatus.OK;
                return Joruneys.ToArray();
            }
            catch (Exception ex)
            {
                Status = RequestStatus.ERROR;
                return new RouteSummary[ZERO];
            }
        }

        //Split the source at the primary tags, these being the ones that hold each route.
        string[] SplitSourceAtTags(string source, string[] Tags)
        {
            return source.Split(Tags, ZERO);
        }

        string GetRouteDataURL(string From_StationName, string To_StationName)
        {
            return NATIONALRAILURL0 + From_StationName + NATIONALRAILURL1 + To_StationName + NATIONALRAILURL2 +
                DateTime.Now.Hour.ToString(TWODIGITFORMAT) + DateTime.Now.Minute.ToString(TWODIGITFORMAT) + NATIONALRAILURL3;
        }

        public static string SaveRoute(RouteSummary[] Route)
        {
            return SerializeObject(Route);
        }

        public static RouteSummary[] LoadRoute(string Route)
        {
            return DeserializeObject<RouteSummary[]>(Route);
        }

        private static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        private static T DeserializeObject<T>(string textWriter)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (XmlReader reader = XmlReader.Create(new StringReader(textWriter)))
            {
                return (T)ser.Deserialize(reader);
            }
        }
    }
}
