using System;
using System.Collections.Generic;
using System.Globalization;
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
    //Class handles retrieving of realtime departure information
    public class CRScode
    {
        //Value
        string _value;

        //Initialiser
        public CRScode(string CRSstring)
        {
            _value = CRSstring;
        }

        //Get setter
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        //Type casters
        public static implicit operator CRScode(string CRSstring)
        {
            return new CRScode(CRSstring);
        }

        public static implicit operator string(CRScode CRScode)
        {
            return CRScode.Value;
        }
    }

    public class RtTrain
    {
        public string departureStationName;
        public string departureStationCRS;
        public string arrivalStationName;
        public string arrivalStationCRS;
        public string statusMessage; //On-time ect
        public string departureTime;
        public string arrivalTime;
        public string estimateddepartureTime;
        public string estimatedarrivalTime;
        public string durationHours;
        public string durationMinutes;
        public string changes;
        public string journeyId;
        public string tocName;
        public string fullFarePrice;

        public RtTrain() { }

        public RtTrain(JSONScraped JSONTrainInfoNationalRail, JSONScraped JSONTrainInfoRealimeTrains)
        {
            this.departureStationName = JSONTrainInfoNationalRail.GetJSONValue("departureStationName");
            this.departureStationCRS = JSONTrainInfoNationalRail.GetJSONValue("departureStationCRS");
            this.arrivalStationName = JSONTrainInfoNationalRail.GetJSONValue("arrivalStationName");
            this.arrivalStationCRS = JSONTrainInfoNationalRail.GetJSONValue("arrivalStationCRS");
            this.statusMessage = JSONTrainInfoNationalRail.GetJSONValue("statusMessage");
            this.arrivalTime = JSONTrainInfoNationalRail.GetJSONValue("departureTime");
            this.departureTime = JSONTrainInfoNationalRail.GetJSONValue("arrivalTime");
            this.durationHours = DateTime.Parse(this.arrivalTime).Subtract(DateTime.Parse(this.departureTime)).Hours.ToString();
            this.durationMinutes = DateTime.Parse(this.arrivalTime).Subtract(DateTime.Parse(this.departureTime)).Minutes.ToString();
            this.changes = JSONTrainInfoNationalRail.GetJSONValue("changes");
            this.journeyId = JSONTrainInfoNationalRail.GetJSONValue("journeyId");
            this.tocName = JSONTrainInfoNationalRail.GetJSONValue("tocName");
            this.fullFarePrice = JSONTrainInfoNationalRail.GetJSONValue("fullFarePrice");

            if (this.statusMessage.Contains("mins late"))
            {
                DateTime dt = DateTime.ParseExact(this.arrivalTime, "HH:mm", CultureInfo.GetCultureInfo("en-gb"));
                this.estimatedarrivalTime = dt.AddMinutes(Convert.ToInt32(this.statusMessage.Replace(" mins late", ""))).ToString("HH:mm");
                this.estimateddepartureTime = dt.AddMinutes(Convert.ToInt32(this.statusMessage.Replace(" mins late", "")) + 2).ToString("HH:mm");
            }
        }
    }

    public static class RtTrainData
    {
        private const string NATIONALRAILURL = "http://ojp.nationalrail.co.uk/service/timesandfares/";
        private const char URLSLASH = '/';

        public static RtTrain[] GetLiveDepartures(CRScode From, CRScode To)
        {
            RtHTMLScraper HTMLScraper = new RtHTMLScraper(NATIONALRAILURL + From + URLSLASH + To + "/today/" + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + "/dep");
            RtHTMLScraped HTMLDocument = HTMLScraper.Source;
            JSONScraped[] JSONDepartures = HTMLDocument.ScrapeJSONinArray("jsonJourneyBreakdown", "returnJsonFareBreakdowns");
            
            RtTrain[] TrainDepartures = new RtTrain[JSONDepartures.Length];

            for (int i = 0; i < JSONDepartures.Length; i++)
                TrainDepartures[i] = new RtTrain(JSONDepartures[i], null);

            return TrainDepartures;
        }
    }
}