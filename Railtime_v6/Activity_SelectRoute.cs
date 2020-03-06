using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RtGraphics;

namespace Railtime_v6
{
    [Activity(Label = "Railtime", Icon = "@drawable/IconLauncher", Theme = "@style/Theme.Custom", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize)]
    public class Activity_SelectRoute : Activity
    {
        private const int NAVBARHEIGHT = 250;
        private const int NAVBARPADDING = 40;

        TextView FromSearchText;
        TextView FromSearchHint;
        TextView ToSearchText;
        TextView ToSearchHint;

        RtStationData FromStation;
        RtStationData ToStation;

        RtTrainDeparturesView RtTrainDeparturesView;

        RtStationSearchDialog FromStationSearchDialog;
        RtStationSearchDialog ToStationSearchDialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Initialisation
            RtGraphicsLayouts RtGraphicsLayouts = new RtGraphicsLayouts(this);
            RtGraphicsLayouts.SetColourStatusBar(Window, RtGraphicsColours.Orange);

            //Root
            RelativeLayout AbsoluteRootLayout = new RelativeLayout(this);
            AbsoluteRootLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            SetContentView(AbsoluteRootLayout);

            //Root Layout
            LinearLayout RootLayout = new LinearLayout(this);
            RootLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            RootLayout.Orientation = Orientation.Vertical;
            AbsoluteRootLayout.AddView(RootLayout);

            //Station Selector Dialog (after screen contents, so it overlays)
            FromStationSearchDialog = new RtStationSearchDialog(this, this, Window);
            FromStationSearchDialog.StationSelected += StationSearchDialog_StationSelected;
            AbsoluteRootLayout.AddView(FromStationSearchDialog);

            ToStationSearchDialog = new RtStationSearchDialog(this, this, Window, 1);
            ToStationSearchDialog.StationSelected += StationSearchDialog_StationSelected;
            AbsoluteRootLayout.AddView(ToStationSearchDialog);

            //Navbar Root
            LinearLayout NavBarSearchBack = new LinearLayout(this);
            NavBarSearchBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, NAVBARHEIGHT);
            NavBarSearchBack.SetBackgroundColor(RtGraphicsColours.Orange);
            RootLayout.AddView(NavBarSearchBack);

            //Left Image
            ImageView NavbarSearchLeft = new ImageView(this);
            NavbarSearchLeft.LayoutParameters = RtGraphicsLayouts.LayoutParameters(100, NAVBARHEIGHT);
            NavbarSearchLeft.SetImageResource(Resource.Drawable.Icon_SelectRouteLeft);
            NavBarSearchBack.AddView(NavbarSearchLeft);

            //Centre
            LinearLayout NavBarSearchCentre = new LinearLayout(this);
            NavBarSearchCentre.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-200, NAVBARHEIGHT);
            NavBarSearchCentre.Orientation = Orientation.Vertical;
            NavBarSearchBack.AddView(NavBarSearchCentre);

            //Right Image
            ImageView NavbarSearchRight = new ImageView(this);
            NavbarSearchRight.LayoutParameters = RtGraphicsLayouts.LayoutParameters(100, NAVBARHEIGHT);
            NavbarSearchRight.SetImageResource(Resource.Drawable.Icon_SelectRouteRight);
            NavBarSearchBack.AddView(NavbarSearchRight);

            //Right Event
            NavbarSearchRight.Click += delegate { SwapFromTo(); };

            //From Search Box Back
            LinearLayout FromSearchBack = new LinearLayout(this);
            FromSearchBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-200, NAVBARHEIGHT / 2);
            FromSearchBack.SetDpPadding(RtGraphicsLayouts, 0, 35, 0, 10);
            NavBarSearchCentre.AddView(FromSearchBack);

            //From Search Box
            RelativeLayout FromSearchBox = new RelativeLayout(this);
            FromSearchBox.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromSearchBox.SetDpPadding(RtGraphicsLayouts, NAVBARPADDING, 0, NAVBARPADDING, 0);
            FromSearchBox.SetBackgroundResource(Resource.Drawable.StyleRoundShaded);
            FromSearchBack.AddView(FromSearchBox);

            //From Search Hint
            FromSearchHint = new TextView(this);
            FromSearchHint.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromSearchHint.Format(RtGraphicsExt.TextFormats.Heading1);
            FromSearchHint.SetTextColor(RtGraphicsColours.Salmon);
            FromSearchHint.Gravity = GravityFlags.CenterVertical;
            FromSearchHint.Text = "From";
            FromSearchBox.AddView(FromSearchHint);

            //From Search Text
            FromSearchText = new TextView(this);
            FromSearchText.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromSearchText.Format(RtGraphicsExt.TextFormats.Heading1);
            FromSearchText.Gravity = GravityFlags.CenterVertical;
            FromSearchBox.AddView(FromSearchText);

            //From Events
            FromSearchBox.Click += delegate { FromStationSearchDialog.ShowDialog(FromSearchText.Text); };

            //To Search Box Back
            LinearLayout ToSearchBack = new LinearLayout(this);
            ToSearchBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-200, NAVBARHEIGHT / 2);
            ToSearchBack.SetDpPadding(RtGraphicsLayouts, 0, 10, 0, 35);
            NavBarSearchCentre.AddView(ToSearchBack);

            //To Search Box
            RelativeLayout ToSearchBox = new RelativeLayout(this);
            ToSearchBox.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            ToSearchBox.SetDpPadding(RtGraphicsLayouts, NAVBARPADDING, 0, NAVBARPADDING, 0);
            ToSearchBox.SetBackgroundResource(Resource.Drawable.StyleRoundShaded);
            ToSearchBack.AddView(ToSearchBox);

            //To Search Hint
            ToSearchHint = new TextView(this);
            ToSearchHint.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            ToSearchHint.Format(RtGraphicsExt.TextFormats.Heading1);
            ToSearchHint.SetTextColor(RtGraphicsColours.Salmon);
            ToSearchHint.Gravity = GravityFlags.CenterVertical;
            ToSearchHint.Text = "To";
            ToSearchBox.AddView(ToSearchHint);

            //To Search Text
            ToSearchText = new TextView(this);
            ToSearchText.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            ToSearchText.Format(RtGraphicsExt.TextFormats.Heading1);
            ToSearchText.Gravity = GravityFlags.CenterVertical;
            ToSearchBox.AddView(ToSearchText);

            //To Events
            ToSearchBox.Click += delegate { ToStationSearchDialog.ShowDialog(ToSearchText.Text); };
            
            //Screen Content Scroller
            ScrollView ContentScrollerRoot = new ScrollView(this);
            ContentScrollerRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            RootLayout.AddView(ContentScrollerRoot);

            //Screen Content 
            LinearLayout ContentScrollRoot = new LinearLayout(this);
            ContentScrollRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            ContentScrollRoot.Orientation = Orientation.Vertical;
            ContentScrollRoot.SetDpPadding(RtGraphicsLayouts, 25, 25, 25, 25);
            ContentScrollerRoot.AddView(ContentScrollRoot);

            //Train Results Box
            RtTrainDeparturesView = new RtTrainDeparturesView(this, this);
            RtTrainDeparturesView.Callback += RtTrainDeparturesView_Callback;
            ContentScrollRoot.AddView(RtTrainDeparturesView);
        }

        private void RtTrainDeparturesView_Callback(RtTrain DepartureData)
        {
            //throw new NotImplementedException();
        }

        private void SwapFromTo()
        {
            //Swap Text in boxes.
            string TmpText = FromSearchText.Text;
            FromSearchText.Text = ToSearchText.Text;
            ToSearchText.Text = TmpText;

            //Swap visibility states of hints.
            ViewStates TmpViewState = FromSearchHint.Visibility;
            FromSearchHint.Visibility = ToSearchHint.Visibility;
            ToSearchHint.Visibility = TmpViewState;

            //Swap Data stored in variables
            RtStationData TmpStation = FromStation;
            FromStation = ToStation;
            ToStation = TmpStation;

            //If both data not null, start a departure search.
            if (FromStation != null && ToStation != null)
                RtTrainDeparturesView.ShowDepartures(FromStation.Code, ToStation.Code);
        }

        private void StationSearchDialog_StationSelected(int DialogID, RtStationData RtStationData)
        {
            if (DialogID == 0)
            {
                FromSearchText.Text = RtStationData.StationName;
                FromSearchHint.Visibility = ViewStates.Gone;
                FromStation = RtStationData;
            }
            else
            {
                ToSearchText.Text = RtStationData.StationName;
                ToSearchHint.Visibility = ViewStates.Gone;
                ToStation = RtStationData;
            }

            if (FromStation != null && ToStation != null)
                RtTrainDeparturesView.ShowDepartures(FromStation.Code, ToStation.Code);
        }

        public override void OnBackPressed()
        {
            if (FromStationSearchDialog.DialogShowing)
                FromStationSearchDialog.CloseDialog();
            else if (ToStationSearchDialog.DialogShowing)
                ToStationSearchDialog.CloseDialog();
            else
                base.OnBackPressed();
        }
    }
}