using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RtGraphics;

namespace Railtime_v6
{
    [Activity(Label = "Railtime", Icon = "@drawable/IconLauncher", Theme = "@style/Theme.Custom", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Activity_SelectStation : Activity
    {
        public LinearLayout ContentRoot;

        private const string NAVBARTEXT = "Search Starting Station";
        private const int NAVBARHEIGHT = 140;
        private const int NAVBARPADDING = 40;
        private const int SMALLPADDING = 25;

        private RtGraphicsLayouts RtGraphicsLayouts;

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

            //Navbar
            LinearLayout NavbarLayout = new LinearLayout(this);
            NavbarLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, NAVBARHEIGHT);
            NavbarLayout.SetBackgroundColor(RtGraphicsColours.Orange);
            RootLayout.AddView(NavbarLayout);

            LinearLayout NavbarBack = new LinearLayout(this);
            NavbarBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(NAVBARHEIGHT, NAVBARHEIGHT);
            NavbarBack.SetBackgroundResource(Resource.Drawable.IconBack);
            NavbarBack.Click += NavbarBack_Click; ;
            NavbarLayout.AddView(NavbarBack);

            TextView NavBarTitle = new TextView(this);
            NavBarTitle.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-140, RtGraphicsLayouts.EXPAND);
            NavBarTitle.SetDpPadding(RtGraphicsLayouts, NAVBARPADDING, NAVBARPADDING, NAVBARHEIGHT + NAVBARPADDING, NAVBARPADDING);
            NavBarTitle.Gravity = GravityFlags.Center;
            NavBarTitle.Format(RtGraphicsExt.TextFormats.Heading);
            NavBarTitle.Text = NAVBARTEXT;
            NavbarLayout.AddView(NavBarTitle);

            //Content
            ScrollView ContentScrollerRoot = new ScrollView(this);
            ContentScrollerRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            RootLayout.AddView(ContentScrollerRoot);

            LinearLayout ContentScrollRoot = new LinearLayout(this);
            ContentScrollRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            ContentScrollRoot.Orientation = Orientation.Vertical;
            ContentScrollerRoot.AddView(ContentScrollRoot);

            LinearLayout NavbarTitleBorder = new LinearLayout(this);
            NavbarTitleBorder.SetBackgroundColor(RtGraphicsColours.OrangeDim);
            NavbarTitleBorder.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            NavbarTitleBorder.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, SMALLPADDING, SMALLPADDING, SMALLPADDING);
            ContentScrollRoot.AddView(NavbarTitleBorder);

            EditText NavBarTitleSearch = new EditText(this);
            NavBarTitleSearch.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            //NavBarTitle.SetDpPadding(RtGraphicsLayouts, NAVBARPADDING, NAVBARPADDING, NAVBARHEIGHT + NAVBARPADDING, NAVBARPADDING);
            //NavBarTitleSearch.SetBackgroundColor(RtGraphicsColours.GreyLightest);
            NavBarTitleSearch.Background.SetColorFilter(RtGraphicsColours.OrangeDim, PorterDuff.Mode.SrcIn);
            NavBarTitleSearch.SetHintTextColor(RtGraphicsColours.Orange);
            NavBarTitleSearch.Format(RtGraphicsExt.TextFormats.Heading);
            NavBarTitleSearch.SetTextColor(RtGraphicsColours.GreyLightest);
            NavBarTitleSearch.Hint = NAVBARTEXT;
            NavBarTitleSearch.AfterTextChanged += NavBarTitle_AfterTextChanged;
            NavbarTitleBorder.AddView(NavBarTitleSearch);

            ContentRoot = new LinearLayout(this);
            ContentRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            ContentRoot.Orientation = Orientation.Vertical;
            ContentRoot.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, SMALLPADDING, SMALLPADDING, SMALLPADDING);
            ContentScrollRoot.AddView(ContentRoot);
        }

        private void NavBarTitle_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            string InputText = (sender as EditText).Text;

            if (InputText.Length > 0)
            {
                RtStationData[] SearchData = (InputText.Length == 3 && InputText.ToUpper() == InputText) ? RtStations.SearchByCRS(InputText) : RtStations.SearchByName(InputText);

                ContentRoot.RemoveAllViews();
                for (int i = 0; i < SearchData.Length; i++)
                {
                    LinearLayout ResultBack = new LinearLayout(this);
                    ResultBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND , NAVBARHEIGHT);
                    ResultBack.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, SMALLPADDING, SMALLPADDING, SMALLPADDING);
                    ContentRoot.AddView(ResultBack);

                    TextView tResult = new TextView(this);
                    tResult.Format(RtGraphicsExt.TextFormats.Paragraph);
                    tResult.Text = SearchData[i].Code + " - " + SearchData[i].StationName;
                    ResultBack.AddView(tResult);
                }
            }
        }

        private void NavbarBack_Click(object sender, EventArgs e)
        {
            this.OnBackPressed();
        }
    }
}