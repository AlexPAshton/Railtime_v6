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
using Android.Widget;
using RtGraphics;

namespace Railtime_v6
{
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

            //Now on a new thread download the data.
            new Thread(() =>
            {
                //Download departures data
                RtTrain[] DeparturesData = RtTrainData.GetLiveDepartures(FromCRS, ToCRS);

                //On the UI thread, create result views.
                Activity.RunOnUiThread(() =>
                {

                    _TrainDeparturesLayout.RemoveAllViews();

                    if (DeparturesData.Length == 0)
                    {
                        //If no departures
                        TextView FromSearchEmpty = new TextView(this.Context);
                        FromSearchEmpty.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
                        FromSearchEmpty.Gravity = GravityFlags.Center;
                        FromSearchEmpty.Format(RtGraphicsExt.TextFormats.Paragraph1);
                        FromSearchEmpty.Text = "No Trains Currently.";
                        _TrainDeparturesLayout.AddView(FromSearchEmpty);
                    }
                    else
                    {
                        //Draw departures list
                        for (int i = 0; i < DeparturesData.Length; i++)
                        {
                            int LocalIndex = i;

                            LinearLayout lTrainTimeBack = new LinearLayout(this.Context);
                            lTrainTimeBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 150);
                            lTrainTimeBack.SetDpPadding(RtGraphicsLayouts, 25, 25, 25, 25);
                            lTrainTimeBack.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
                            _TrainDeparturesLayout.AddView(lTrainTimeBack);

                            ImageView iTrainTimeIcon = new ImageView(this.Context);
                            iTrainTimeIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(100, 100);
                            if (DeparturesData[i].tocName == "Virgin Trains")
                                iTrainTimeIcon.SetImageResource(Resource.Drawable.Icon_VT);
                            else if (DeparturesData[i].tocName == "TransPennine Express")
                                iTrainTimeIcon.SetImageResource(Resource.Drawable.Icon_TP);
                            else if (DeparturesData[i].tocName == "Northern")
                                iTrainTimeIcon.SetImageResource(Resource.Drawable.Icon_NR);
                            //else if (sTOC == "SR")
                            //    iTrainTimeIcon.SetImageResource(Resource.Drawable.Icon_SR);
                            else
                                iTrainTimeIcon.SetImageResource(Resource.Drawable.Icon_UNKNOWN);

                            lTrainTimeBack.AddView(iTrainTimeIcon);

                            LinearLayout lVert = new LinearLayout(this.Context);
                            lVert.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(100 + 25 + 25 + 25 + 25), 150);
                            lVert.SetDpPadding(RtGraphicsLayouts, 25, 0, 0, 0);
                            lVert.Orientation = Orientation.Vertical;
                            lTrainTimeBack.AddView(lVert);

                            LinearLayout lHori1 = new LinearLayout(this.Context);
                            lVert.AddView(lHori1);

                            TextView DepartureTime = new TextView(this.Context);
                            DepartureTime.TextSize = 16;
                            DepartureTime.Text = DeparturesData[i].departureTime;
                            DepartureTime.SetDpPadding(RtGraphicsLayouts, 0, 0, -680, 0);
                            lHori1.AddView(DepartureTime);

                            ImageView DepartureIcon = new ImageView(this.Context);
                            DepartureIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(50, 50);
                            DepartureIcon.SetImageResource(Resource.Drawable.Icon_DeparturesFrom);
                            lHori1.AddView(DepartureIcon);

                            TextView JourneyTime = new TextView(this.Context);
                            JourneyTime.TextSize = 16;
                            JourneyTime.Text = DeparturesData[i].durationHours + "h " + ((DeparturesData[i].durationMinutes.Length == 1) ? "0" + DeparturesData[i].durationMinutes : DeparturesData[i].durationMinutes) + "m";
                            lHori1.AddView(JourneyTime);

                            ImageView ArrivalIcon = new ImageView(this.Context);
                            ArrivalIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(50, 50);
                            ArrivalIcon.SetImageResource(Resource.Drawable.Icon_DeparturesTo);
                            lHori1.AddView(ArrivalIcon);

                            TextView ArrivalTime = new TextView(this.Context);
                            ArrivalTime.TextSize = 16;
                            ArrivalTime.Text = DeparturesData[i].arrivalTime;
                            ArrivalTime.SetDpPadding(RtGraphicsLayouts, -680, 0, 0, 0);
                            lHori1.AddView(ArrivalTime);

                            LinearLayout lHori = new LinearLayout(this.Context);
                            lVert.AddView(lHori);

                            TextView tPlannedArrival = new TextView(this.Context);
                            tPlannedArrival.SetTextColor(Android.Graphics.Color.Gray);
                            tPlannedArrival.Text = DeparturesData[i].changes + " Changes";
                            lHori.AddView(tPlannedArrival);

                            TextView tActualArrival = new TextView(this.Context);
                            tActualArrival.SetTextColor(Android.Graphics.Color.LightGray);
                            tActualArrival.SetDpPadding(RtGraphicsLayouts, 25, 0, 0, 0);
                            tActualArrival.Text = DeparturesData[i].statusMessage;
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
                            lSpacer.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 10);
                            _TrainDeparturesLayout.AddView(lSpacer);

                            lTrainTimeBack.Click += delegate { Callback?.Invoke(DeparturesData[LocalIndex]); };
                        }
                    }

                    //Once all drawn, show departures list
                    _TrainDeparturesLayout.Visibility = ViewStates.Visible;
                    _TrainDeparturesLoading.Visibility = ViewStates.Gone;
                });

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
}