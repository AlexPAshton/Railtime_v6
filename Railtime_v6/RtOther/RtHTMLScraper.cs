using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Railtime_v6
{
    class RtHTMLScraped
    {
        private string _Value;

        public RtHTMLScraped(string RAWhtml)
        {
            _Value = RAWhtml;
        }

        public static implicit operator RtHTMLScraped(string RAWhtml)
        {
            return new RtHTMLScraped(RAWhtml);
        }

        public static implicit operator string(RtHTMLScraped RtHTMLScraped)
        {
            return RtHTMLScraped._Value;
        }
    }

    static class RtHTMLScrapedExt
    {
        private const int ZERO = 0;
        private const int FIRSTOFFSET = 1;

        public static RtHTMLScraped ScrapeBetweenTags(this RtHTMLScraped RtHTMLScraped, string StartTag, string EndTag, int Index = ZERO)
        {
            string HTMLraw = RtHTMLScraped;
            string HTMLSplitRaw = HTMLraw.Split(new string[] { StartTag }, ZERO)[Index + FIRSTOFFSET];
            return HTMLSplitRaw.Split(new string[] { EndTag }, ZERO)[ZERO];
        }

        public static JSONScraped[] ScrapeJSONinArray(this RtHTMLScraped RtHTMLScraped, string Identifier, string EndIdentifier)
        {
            string RAWhtml = RtHTMLScraped;
            int JSONCount = RAWhtml.Split(new string[] { Identifier }, ZERO).Length;
            JSONScraped[] JSONScrapedinArray = new JSONScraped[JSONCount - FIRSTOFFSET];

            for (int i = FIRSTOFFSET; i < JSONCount; i++)
            {
                JSONScrapedinArray[i - FIRSTOFFSET] = RAWhtml.Split(new string[] { Identifier }, ZERO)[i].Split(new string[] { EndIdentifier }, ZERO)[ZERO];
            }

            return JSONScrapedinArray;
        }
    }

    public class JSONScraped
    {
        private const int ZERO = 0;
        private const int VALUEINDEX = 1;
        private const char PAIRSPLIT = ',';
        private const string KEYVALUESPLIT = "\":";
        private const string EMPTY = "";
        private const string QUOTATIONMARK = "\"";

        private string _Value;

        public JSONScraped(string Value)
        {
            _Value = Value;
        }

        public static implicit operator JSONScraped(string RAWhtml)
        {
            return new JSONScraped(RAWhtml);
        }

        public string GetJSONValue(string Key)
        {
            string[] JSONDataSplit = _Value.Split(PAIRSPLIT);

            for (int i = ZERO; i<JSONDataSplit.Length;i++)
            {
                if (JSONDataSplit[i].Split(new string[] { KEYVALUESPLIT }, ZERO)[ZERO].Replace(QUOTATIONMARK, EMPTY) == Key)
                    return JSONDataSplit[i].Split(new string[] { KEYVALUESPLIT }, ZERO)[VALUEINDEX].Replace(QUOTATIONMARK, EMPTY);
            }

            return null;
        }
    }

    class RtHTMLScraper
    {
        public enum InputType
        {
            URL, SOURCE
        }

        private RtHTMLScraped HTMLSource;

        public RtHTMLScraper(string HTMLUrl)
        {
            HTMLSource = new WebClient().DownloadString(HTMLUrl);
        }

        public RtHTMLScraper(RtHTMLScraped RtHTMLScraped)
        {
            HTMLSource = RtHTMLScraped;
        }

        public RtHTMLScraped Source
        {
            get { return HTMLSource; }
            set { HTMLSource = value; }
        }
    }
}