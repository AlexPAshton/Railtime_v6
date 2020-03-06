using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using RtGraphics;
using RtRoutePlanner;

namespace Railtime_v6
{
    public static class JavascriptResultHolder
    {
        public static string Result = "";
    }

    public class RtTrainDeparturesView
    {
        //Public variables
        public delegate void CallbackEventHandler(RtTrain DepartureData);
        public event CallbackEventHandler Callback;

        //Private Variables
        private Context Context;
        private Activity Activity;
        private LinearLayout _RootLayout;
        private RtGraphicsLayouts RtGraphicsLayouts;
        private LinearLayout _TrainDeparturesLayout;
        private LinearLayout _TrainDeparturesLoading;

        //Initialiser
        public RtTrainDeparturesView(Context Context, Activity Activity)
        {
            this.Context = Context;//Set Context
            this.Activity = Activity;//Set Activity

            //Initialise graphics
            RtGraphicsLayouts = new RtGraphicsLayouts(this.Context);

            //Generate View
            _RootLayout = new LinearLayout(this.Context);
            _RootLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            _RootLayout.Orientation = Orientation.Vertical;
            _RootLayout.Visibility = ViewStates.Gone;

            //Generate Departures Root
            _TrainDeparturesLayout = new LinearLayout(this.Context);
            _TrainDeparturesLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            _TrainDeparturesLayout.Orientation = Orientation.Vertical;
            _TrainDeparturesLayout.Visibility = ViewStates.Gone;
            _RootLayout.AddView(_TrainDeparturesLayout);

            //Generate Departures Loading Root
            _TrainDeparturesLoading = new LinearLayout(this.Context);
            _TrainDeparturesLoading.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 400);
            _TrainDeparturesLoading.Orientation = Orientation.Vertical;
            _TrainDeparturesLoading.SetGravity(GravityFlags.Center);
            _TrainDeparturesLoading.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            _RootLayout.AddView(_TrainDeparturesLoading);

            ImageView LoadingIcon = new ImageView(this.Context);
            LoadingIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(120, 120);
            LoadingIcon.SetImageResource(Resource.Drawable.Icon_Loading);
            _TrainDeparturesLoading.AddView(LoadingIcon);

            //Thread for rotating loading icon
            new Thread(() => { int Z = 0; while (true) { Activity.RunOnUiThread(() => LoadingIcon.Rotation = Z); Z += 4; Thread.Sleep(20); } }).Start();
        }

        //Accessor
        public LinearLayout RootLayout
        {
            get { return _RootLayout; }
        }

        //Show Departures Method
        public void ShowDepartures(string FromCRS, string ToCRS)
        {
            //Show this view and hide the empty departures list.
            _TrainDeparturesLayout.Visibility = ViewStates.Gone;
            _TrainDeparturesLoading.Visibility = ViewStates.Visible;
            _RootLayout.Visibility = ViewStates.Visible;

            //Download departures data
            new Thread(() =>
            {

                RoutePlanner.RequestStatus RequestStatus;
                RouteSummary[] DeparturesData = new RoutePlanner().GetRouteInfo(FromCRS, ToCRS, out RequestStatus);

                //On the UI thread, create result views.
                if (RequestStatus == RoutePlanner.RequestStatus.OK)
                {
                    Activity.RunOnUiThread(() =>
                    {

                        _TrainDeparturesLayout.RemoveAllViews();


                        //Draw departures list
                        for (int i = 0; i < DeparturesData.Length; i++)
                        {
                            int LocalIndex = i;

                            LinearLayout lTrainTimeBack = new LinearLayout(this.Context);
                            lTrainTimeBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 150);
                            lTrainTimeBack.Orientation = Orientation.Vertical;
                            lTrainTimeBack.SetDpPadding(RtGraphicsLayouts, 2, 2, 2, 2);
                            lTrainTimeBack.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
                            _TrainDeparturesLayout.AddView(lTrainTimeBack);

                            LinearLayout lTrainTimeContent = new LinearLayout(this.Context);
                            lTrainTimeContent.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 100);
                            lTrainTimeBack.AddView(lTrainTimeContent);

                            LinearLayout lTrainTimeContentTOCBar = new LinearLayout(this.Context);
                            lTrainTimeContentTOCBar.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 44);
                            lTrainTimeBack.AddView(lTrainTimeContentTOCBar);


                            LinearLayout lTrainTimeArrival = new LinearLayout(this.Context);
                            lTrainTimeArrival.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-550, RtGraphicsLayouts.EXPAND);
                            lTrainTimeArrival.Orientation = Orientation.Vertical;
                            lTrainTimeArrival.SetDpPadding(RtGraphicsLayouts, 0, 50, 0, 0);
                            lTrainTimeContent.AddView(lTrainTimeArrival);

                            LinearLayout lTrainTimeDuration = new LinearLayout(this.Context);
                            lTrainTimeDuration.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-400, RtGraphicsLayouts.EXPAND);
                            lTrainTimeContent.AddView(lTrainTimeDuration);

                            LinearLayout lTrainTimeDeparture = new LinearLayout(this.Context);
                            lTrainTimeDeparture.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-550, RtGraphicsLayouts.EXPAND);
                            lTrainTimeDeparture.Orientation = Orientation.Vertical;
                            lTrainTimeDeparture.SetDpPadding(RtGraphicsLayouts, 0, 50, 0, 0);
                            lTrainTimeContent.AddView(lTrainTimeDeparture);

                            TextView DepartureTime = new TextView(this.Context);
                            DepartureTime.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
                            DepartureTime.TextSize = 16;
                            DepartureTime.Text = DeparturesData[i].DepartureTime;
                            DepartureTime.Gravity = GravityFlags.Center;
                            lTrainTimeDeparture.AddView(DepartureTime);

                            TextView ArrivalTime = new TextView(this.Context);
                            ArrivalTime.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
                            ArrivalTime.TextSize = 16;
                            ArrivalTime.Text = DeparturesData[i].ArrivalTime;
                            ArrivalTime.Gravity = GravityFlags.Center;
                            lTrainTimeArrival.AddView(ArrivalTime);

                            //////////////////////////////////////////////////
                            //          //                  //              //
                            //          //                  //              //
                            //          //                  //              //
                            //////////////////////////////////////////////////

                            // Textview      Stuff              textview
                            // Departure Time                   Arrival Time


                            LinearLayout lVert = new LinearLayout(this.Context);
                            lVert.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
                            lVert.SetDpPadding(RtGraphicsLayouts, 25, 0, 0, 0);
                            lVert.Orientation = Orientation.Vertical;
                            lTrainTimeContent.AddView(lVert);

                            LinearLayout lHori1 = new LinearLayout(this.Context);
                            lVert.AddView(lHori1);



                            ImageView DepartureIcon = new ImageView(this.Context);
                            DepartureIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(50, 50);
                            DepartureIcon.SetImageResource(Resource.Drawable.Icon_DeparturesFrom);
                            lHori1.AddView(DepartureIcon);

                            TextView JourneyTime = new TextView(this.Context);
                            JourneyTime.TextSize = 16;
                            //JourneyTime.Text = DeparturesData[i].durationHours + "h " + ((DeparturesData[i].durationMinutes.Length == 1) ? "0" + DeparturesData[i].durationMinutes : DeparturesData[i].durationMinutes) + "m";
                            JourneyTime.SetTextColor(RtGraphicsColours.GreyLightest);
                            lHori1.AddView(JourneyTime);

                            ImageView ArrivalIcon = new ImageView(this.Context);
                            ArrivalIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(50, 50);
                            ArrivalIcon.SetImageResource(Resource.Drawable.Icon_DeparturesTo);
                            lHori1.AddView(ArrivalIcon);

                            LinearLayout lHori = new LinearLayout(this.Context);
                            lVert.AddView(lHori);

                            TextView tPlannedArrival = new TextView(this.Context);
                            tPlannedArrival.SetTextColor(Android.Graphics.Color.Gray);
                            //tPlannedArrival.Text = DeparturesData[i].changes + " Changes";
                            lHori.AddView(tPlannedArrival);

                            TextView tActualArrival = new TextView(this.Context);
                            tActualArrival.SetTextColor(Android.Graphics.Color.LightGray);
                            tActualArrival.SetDpPadding(RtGraphicsLayouts, 25, 0, 0, 0);
                            //tActualArrival.Text = DeparturesData[i].statusMessage;
                            lHori.AddView(tActualArrival);

                            if (tActualArrival.Text == "on time")
                            {
                                tActualArrival.Text = "On Time";
                                tActualArrival.SetTextColor(Android.Graphics.Color.DarkGreen);
                            }
                            else if (tActualArrival.Text == "Ca:nc")
                                tActualArrival.SetTextColor(Android.Graphics.Color.DarkRed);
                            else if (tActualArrival.Text == "null")
                                tActualArrival.Visibility = ViewStates.Gone;

                            LinearLayout lSpacer = new LinearLayout(this.Context);
                            lSpacer.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 20);
                            _TrainDeparturesLayout.AddView(lSpacer);
                        }
                        //lTrainTimeBack.Click += delegate { Callback?.Invoke(DeparturesData[LocalIndex]); };

                        //Once all drawn, show departures list
                        _TrainDeparturesLayout.Visibility = ViewStates.Visible;
                        _TrainDeparturesLoading.Visibility = ViewStates.Gone;
                    });
                }
                else if (RequestStatus == RoutePlanner.RequestStatus.ERROR)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        _TrainDeparturesLayout.RemoveAllViews();

                        TextView FromSearchEmpty = new TextView(this.Context);
                        FromSearchEmpty.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 400);
                        FromSearchEmpty.Gravity = GravityFlags.Center;
                        FromSearchEmpty.Format(RtGraphicsExt.TextFormats.Paragraph1);
                        FromSearchEmpty.Text = "No Connection.";
                        _TrainDeparturesLayout.AddView(FromSearchEmpty);

                        _TrainDeparturesLayout.Visibility = ViewStates.Visible;
                        _TrainDeparturesLoading.Visibility = ViewStates.Gone;
                    });
                }
                else if (RequestStatus == RoutePlanner.RequestStatus.EMPTY)
                {
                    Activity.RunOnUiThread(() =>
                    {
                        _TrainDeparturesLayout.RemoveAllViews();

                        TextView FromSearchEmpty = new TextView(this.Context);
                        FromSearchEmpty.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 400);
                        FromSearchEmpty.Gravity = GravityFlags.Center;
                        FromSearchEmpty.Format(RtGraphicsExt.TextFormats.Paragraph1);
                        FromSearchEmpty.Text = "No Departures.";
                        _TrainDeparturesLayout.AddView(FromSearchEmpty);

                        _TrainDeparturesLayout.Visibility = ViewStates.Visible;
                        _TrainDeparturesLoading.Visibility = ViewStates.Gone;
                    });
                }

            }).Start();
        }

        public void HideDepartures()
        {
            _RootLayout.Visibility = ViewStates.Gone;
        }
    }

    public static class RtTrainDeparturesViewExt
    {
        public static void AddView(this ViewGroup ViewGroup, RtTrainDeparturesView RtTrainDeparturesView)
        {
            ViewGroup.AddView(RtTrainDeparturesView.RootLayout);
        }
    }

    public class JavascriptResult : Java.Lang.Object, IValueCallback
    {
        public void OnReceiveValue(Java.Lang.Object value)
        {
            string thing = (string)value;

            if (thing.Contains("tbody"))
            {
                JavascriptResultHolder.Result = thing.Replace("\\\"", "\"").Replace("\\u003C", "<").Replace("\\t", "    ").Replace("\\n", "\n").Split(new string[] { "<tbody>" }, 0)[1].Split(new string[] { "</tbody>" }, 0)[0];
            }
        }
    }}