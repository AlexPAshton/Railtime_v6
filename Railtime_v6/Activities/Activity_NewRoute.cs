using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using RtGraphics;

namespace Railtime_v6
{
    [Activity(Label = "Railtime", Icon = "@drawable/IconLauncher", Theme = "@style/Theme.Custom", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Activity_NewRoute : Activity
    {
        //Constant Variables
        private const string NAVBARTEXT = "New Route";
        private const int NAVBARHEIGHT = 140;
        private const int NAVBARPADDING = 40;
        private const int SMALLPADDING = 25;
        private const int BIGPADDING = 50;
        private const int ZERO = 0;

        //Private Variables
        private string FromStationCode = "";
        private string FromStationname = "";
        private string ToStationCode = "";
        private string ToStationname = "";

        private RtGraphicsLayouts RtGraphicsLayouts;
        //private LinearLayout TrainsResultsBack;
        //private LinearLayout TrainResultsBox;
        RtTrainDeparturesView TrainDepartures;
        private ScrollView ContentScrollerRoot;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Initialisation
            RtGraphicsLayouts = new RtGraphicsLayouts(this);
            RtGraphicsLayouts.SetColourStatusBar(Window, RtGraphicsColours.Orange);

            //Root
            LinearLayout RootLayout = new LinearLayout(this);
            RootLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            RootLayout.Orientation = Orientation.Vertical;
            SetContentView(RootLayout);

            //Navbar Root
            LinearLayout NavbarLayout = new LinearLayout(this);
            NavbarLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, NAVBARHEIGHT);
            NavbarLayout.SetBackgroundColor(RtGraphicsColours.Orange);
            RootLayout.AddView(NavbarLayout);

            //Navbar Back Icon
            LinearLayout NavbarBack = new LinearLayout(this);
            NavbarBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(NAVBARHEIGHT, NAVBARHEIGHT);
            NavbarBack.SetBackgroundResource(Resource.Drawable.IconBack);
            NavbarBack.Click += NavbarBack_Click; ;
            NavbarLayout.AddView(NavbarBack);

            //Navbar Title
            TextView NavBarTitle = new TextView(this);
            NavBarTitle.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-140, RtGraphicsLayouts.EXPAND);
            NavBarTitle.SetDpPadding(RtGraphicsLayouts, NAVBARPADDING, NAVBARPADDING, NAVBARHEIGHT + NAVBARPADDING, NAVBARPADDING);
            NavBarTitle.Gravity = GravityFlags.Center;
            NavBarTitle.Format(RtGraphicsExt.TextFormats.Heading);
            NavBarTitle.Text = NAVBARTEXT;
            NavbarLayout.AddView(NavBarTitle);

            //Screen Content Scroller
            ContentScrollerRoot = new ScrollView(this);
            ContentScrollerRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            RootLayout.AddView(ContentScrollerRoot);

            //Screen Content 
            LinearLayout ContentScrollRoot = new LinearLayout(this);
            ContentScrollRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            ContentScrollRoot.Orientation = Orientation.Vertical;
            ContentScrollRoot.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, SMALLPADDING, SMALLPADDING, SMALLPADDING);
            ContentScrollerRoot.AddView(ContentScrollRoot);

            //From Station Box
            ContentScrollRoot.AddView(GenerateStationSelectionBox(StationSelectionBoxType.From));

            ContentScrollRoot.AddView(GenerateSpacer());

            //Train Results Box
            TrainDepartures = new RtTrainDeparturesView(this, this);
            TrainDepartures.Callback += TrainDepartures_Callback;
            ContentScrollRoot.AddView(TrainDepartures);

            ContentScrollRoot.AddView(GenerateSpacer());

            //To Station Box
            ContentScrollRoot.AddView(GenerateStationSelectionBox(StationSelectionBoxType.To));
        }

        private void TrainDepartures_Callback(RtTrain DepartureData)
        {
            Console.WriteLine(DepartureData.departureTime);
        }

        private LinearLayout GenerateSpacer()
        {
            LinearLayout Spacer1 = new LinearLayout(this);
            Spacer1.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, SMALLPADDING);
            return Spacer1;
        }

        private void NavbarBack_Click(object sender, EventArgs e) => this.OnBackPressed();

        private void DrawSearchResults(LinearLayout Parent, EditText FromSearchInput, LinearLayout FromSearchBack, LinearLayout FromStationBack, TextView FromStationText, TextView FromStationNoNav, string Search, StationSelectionBoxType StationSelectionBoxType, StationFacilitiesView StationFacilitiesView)
        {
            RtStationData[] StationResults = (Search.Length == 3 && Search.ToUpper() == Search) ? RtStations.SearchByCRS(Search) : RtStations.SearchByName(Search);

            Parent.RemoveAllViews();
            for (int i = 0; i < StationResults.Length; i++)
            {
                int ilocal = i;
                LinearLayout ResultBack = new LinearLayout(this);
                ResultBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 100);
                ResultBack.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, SMALLPADDING, SMALLPADDING, SMALLPADDING);
                ResultBack.SetGravity(GravityFlags.CenterVertical);
                Parent.AddView(ResultBack);

                TextView tResult = new TextView(this);
                tResult.Format(RtGraphicsExt.TextFormats.Paragraph);
                tResult.Text = StationResults[i].Code + " - " + StationResults[i].StationName;
                ResultBack.AddView(tResult);

                ResultBack.Click += delegate
                {
                    FromStationBack.Visibility = ViewStates.Visible;
                    FromStationText.Text = tResult.Text;
                    FromSearchBack.Visibility = ViewStates.Gone;
                    HideKeyboard(FromSearchInput);

                    if (StationResults[ilocal].Code == "LAN" || StationResults[ilocal].Code == "PRE")
                    {
                        FromStationNoNav.Visibility = ViewStates.Gone;
                        StationFacilitiesView.Redraw(StationResults[ilocal].Code, ContentScrollerRoot);
                    }
                    else
                    {
                        FromStationNoNav.Visibility = ViewStates.Visible;
                    }

                    if (StationSelectionBoxType == StationSelectionBoxType.From)
                    {
                        FromStationCode = StationResults[ilocal].Code;
                        FromStationname = StationResults[ilocal].StationName;
                    }
                    else
                    {
                        ToStationCode = StationResults[ilocal].Code;
                        ToStationname = StationResults[ilocal].StationName;
                    }

                    StationSelectionChanged();
                };
            }
        }

        protected void HideKeyboard(View view)
        {
            InputMethodManager inp = (InputMethodManager)GetSystemService(InputMethodService);
            inp.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.NotAlways);
        }

        private enum StationSelectionBoxType
        {
            From, To
        }

        void StationSelectionChanged()
        {
            //Handle station selection changing/adding/removing (E.g display train times in new box);
            if (FromStationCode != "" && ToStationCode != "" && FromStationCode != ToStationCode)
            {
                TrainDepartures.ShowDepartures(FromStationCode, ToStationCode);
            }
            else
                TrainDepartures.HideDepartures();
        }
        
        private LinearLayout GenerateStationSelectionBox(StationSelectionBoxType StationSelectionBoxType)
        {
            //From Back
            LinearLayout FromBack = new LinearLayout(this);
            FromBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 400);
            FromBack.Orientation = Orientation.Vertical;
            FromBack.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            //FromBack.SetDpPadding(RtGraphicsLayouts, BIGPADDING, BIGPADDING, BIGPADDING, BIGPADDING);

            //From Station
            LinearLayout FromStationBack = new LinearLayout(this);
            FromStationBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromStationBack.Orientation = Orientation.Vertical;
            FromStationBack.Visibility = ViewStates.Gone;
            FromBack.AddView(FromStationBack);

            LinearLayout FromStationBar = new LinearLayout(this);
            FromStationBar.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 100);
            FromStationBar.SetDpPadding(RtGraphicsLayouts, 10, 10, 10, 10);
            FromStationBack.AddView(FromStationBar);

            TextView FromStationText = new TextView(this);
            FromStationText.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(10 + 10 + SMALLPADDING + SMALLPADDING + 80), RtGraphicsLayouts.EXPAND);
            FromStationText.Format(RtGraphicsExt.TextFormats.Paragraph2);
            FromStationText.SetDpPadding(RtGraphicsLayouts, 25, 0, 0, 0);
            FromStationText.Gravity = GravityFlags.CenterVertical;
            FromStationText.Text = "Not Set";
            FromStationBar.AddView(FromStationText);

            ImageView FromStationIcon = new ImageView(this);
            FromStationIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(80, 80);
            FromStationIcon.SetImageResource(Resource.Drawable.IconCancel);
            FromStationBar.AddView(FromStationIcon);

            LinearLayout FromStationBarSeperator = new LinearLayout(this);
            FromStationBarSeperator.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 2);
            FromStationBarSeperator.SetBackgroundColor(RtGraphicsColours.GreyLightest);
            FromStationBack.AddView(FromStationBarSeperator);

            TextView FromStationNoNav = new TextView(this);
            FromStationNoNav.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromStationNoNav.Gravity = GravityFlags.Center;
            FromStationNoNav.Format(RtGraphicsExt.TextFormats.Paragraph1);
            FromStationNoNav.Text = "This station doesn't support navigation.";
            FromStationBack.AddView(FromStationNoNav);

            StationFacilitiesView StationFacilitiesView = new StationFacilitiesView(this);
            StationFacilitiesView.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromStationBack.AddView(StationFacilitiesView);

            //From Search
            LinearLayout FromSearchBack = new LinearLayout(this);
            FromSearchBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromSearchBack.Orientation = Orientation.Vertical;
            FromBack.AddView(FromSearchBack);

            RelativeLayout FromSearchBar = new RelativeLayout(this);
            FromSearchBar.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 100);
            FromSearchBar.SetDpPadding(RtGraphicsLayouts, 10, 10, 10, 10);
            FromSearchBack.AddView(FromSearchBar);

            ImageView FromSearchbarIcon = new ImageView(this);
            FromSearchbarIcon.LayoutParameters = RtGraphicsLayouts.LayoutParameters(80, 80);
            FromSearchbarIcon.SetImageResource(Resource.Drawable.IconSearch);
            FromSearchBar.AddView(FromSearchbarIcon);

            EditText FromSearchBox = new EditText(this);
            FromSearchBox.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromSearchBox.SetDpPadding(RtGraphicsLayouts, 80 + SMALLPADDING, 0, 0, 0);
            FromSearchBox.Background.SetColorFilter(Android.Graphics.Color.White, Android.Graphics.PorterDuff.Mode.SrcIn);
            FromSearchBox.Format(RtGraphicsExt.TextFormats.Paragraph2);
            FromSearchBox.Hint = (StationSelectionBoxType == StationSelectionBoxType.From) ? "From" : "To";
            FromSearchBar.AddView(FromSearchBox);

            LinearLayout FromSearchBarSeperator = new LinearLayout(this);
            FromSearchBarSeperator.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, 2);
            FromSearchBarSeperator.SetBackgroundColor(RtGraphicsColours.GreyLightest);
            FromSearchBack.AddView(FromSearchBarSeperator);

            TextView FromSearchEmpty = new TextView(this);
            FromSearchEmpty.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            FromSearchEmpty.Gravity = GravityFlags.Center;
            FromSearchEmpty.Format(RtGraphicsExt.TextFormats.Paragraph1);
            FromSearchEmpty.Text = "Search above for a station.";
            FromSearchBack.AddView(FromSearchEmpty);

            ScrollView FromSearchResultsScroll = new ScrollView(this);
            FromSearchResultsScroll.Visibility = ViewStates.Gone;
            FromSearchBack.AddView(FromSearchResultsScroll);

            LinearLayout FromSearchResultsBack = new LinearLayout(this);
            FromSearchResultsBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            FromSearchResultsBack.Orientation = Orientation.Vertical;
            FromSearchResultsScroll.AddView(FromSearchResultsBack);

            //Event Handlers
            FromStationIcon.Click += delegate
            {
                FromStationBack.Visibility = ViewStates.Gone;
                FromSearchBack.Visibility = ViewStates.Visible;

                if (StationSelectionBoxType == StationSelectionBoxType.From)
                {
                    FromStationCode = "";
                    FromStationname = "";
                }
                else
                {
                    ToStationCode = "";
                    ToStationname = "";
                }

                StationSelectionChanged();
            };
            FromSearchBox.AfterTextChanged += delegate
            {
                if (FromSearchBox.Length() > 1)
                {
                    DrawSearchResults(FromSearchResultsBack, FromSearchBox, FromSearchBack, FromStationBack, FromStationText, FromStationNoNav, FromSearchBox.Text, StationSelectionBoxType, StationFacilitiesView); FromSearchResultsScroll.Visibility = ViewStates.Visible; FromSearchEmpty.Visibility = ViewStates.Gone;
                }
                else
                {
                    FromSearchResultsScroll.Visibility = ViewStates.Gone;
                    FromSearchEmpty.Visibility = ViewStates.Visible;
                }

                if (StationSelectionBoxType == StationSelectionBoxType.From)
                {
                    ContentScrollerRoot.SmoothScrollTo(0, 0);
                }
                else
                {
                    ContentScrollerRoot.SmoothScrollTo(0, 1000000);
                }
            };

            return FromBack;
        }
    }
}