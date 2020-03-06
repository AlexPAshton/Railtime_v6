using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RtGraphics;

namespace Railtime_v6
{
    [Activity(Label = "Railtime", Icon = "@drawable/IconLauncher", Theme = "@style/Theme.Custom", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Activity_Home : Activity
    {
        private const int NAVBARHEIGHT = 140;
        private const int NAVBARPADDING = 40;
        private const int SMALLPADDING = 25;
        private const int BIGPADDING = 50;
        private const int ZERO = 0;
        private const int ADDROUTESPACERHEIGHT = 500;
        private const int ADDROUTEBTNNWIDTH = 380;
        private const int ADDROUTEBTNHEIGHT = 240;
        private const int NOROUTESICONSIZE = 120;

        private const string NAVBARTEXT = "My Routes";
        private const string ADDROUTTBNTEXT = "+";
        private const string NOROUTESTEXT = "You don't have any routes yet, tap above to get started.";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Initialisation
            RtGraphicsLayouts RtGraphicsLayouts = new RtGraphicsLayouts(this);
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

            TextView NavBarTitle = new TextView(this);
            NavBarTitle.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-140, RtGraphicsLayouts.EXPAND);
            NavBarTitle.SetDpPadding(RtGraphicsLayouts, NAVBARHEIGHT + NAVBARPADDING, NAVBARPADDING, NAVBARPADDING, NAVBARPADDING);
            NavBarTitle.Gravity = GravityFlags.Center;
            NavBarTitle.Format(RtGraphicsExt.TextFormats.Heading);
            NavBarTitle.Text = NAVBARTEXT;
            NavbarLayout.AddView(NavBarTitle);

            LinearLayout NavbarOptions = new LinearLayout(this);
            NavbarOptions.LayoutParameters = RtGraphicsLayouts.LayoutParameters(NAVBARHEIGHT, NAVBARHEIGHT);
            NavbarOptions.SetBackgroundResource(Resource.Drawable.IconOptions);
            NavbarOptions.Click += NavbarOptions_Click;
            NavbarLayout.AddView(NavbarOptions);

            //Content
            ScrollView ContentScrollerRoot = new ScrollView(this);
            ContentScrollerRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            RootLayout.AddView(ContentScrollerRoot);

            LinearLayout ContentScrollRoot = new LinearLayout(this);
            ContentScrollRoot.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            ContentScrollRoot.Orientation = Orientation.Vertical;
            ContentScrollRoot.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, ZERO, SMALLPADDING, SMALLPADDING);
            ContentScrollerRoot.AddView(ContentScrollRoot);

            //Add Route Button
            LinearLayout AddRouteSpacer = new LinearLayout(this);
            AddRouteSpacer.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, ADDROUTESPACERHEIGHT);
            AddRouteSpacer.SetGravity(GravityFlags.Center);
            ContentScrollRoot.AddView(AddRouteSpacer);

            LinearLayout AddRouteButton = new LinearLayout(this);
            AddRouteButton.LayoutParameters = RtGraphicsLayouts.LayoutParameters(ADDROUTEBTNNWIDTH, ADDROUTEBTNHEIGHT);
            AddRouteButton.Orientation = Orientation.Vertical;
            AddRouteButton.Click += AddRouteButton_Click;
            AddRouteSpacer.AddView(AddRouteButton);

            LinearLayout AddRouteButtonTop = new LinearLayout(this);
            AddRouteButtonTop.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, BIGPADDING);
            AddRouteButtonTop.SetBackgroundResource(Resource.Drawable.StyleTicketTop);
            AddRouteButton.AddView(AddRouteButtonTop);

            LinearLayout AddRouteButtonMiddle = new LinearLayout(this);
            AddRouteButtonMiddle.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, NAVBARHEIGHT);
            AddRouteButtonMiddle.SetBackgroundResource(Resource.Drawable.StyleTicketMiddle);
            AddRouteButtonMiddle.SetGravity(GravityFlags.Center);
            AddRouteButton.AddView(AddRouteButtonMiddle);

            LinearLayout AddRouteButtonBottom = new LinearLayout(this);
            AddRouteButtonBottom.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, BIGPADDING);
            AddRouteButtonBottom.SetBackgroundResource(Resource.Drawable.StyleTicketBottom);
            AddRouteButton.AddView(AddRouteButtonBottom);

            TextView AddRouteText = new TextView(this);
            AddRouteText.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.CONTAIN, RtGraphicsLayouts.CONTAIN);
            AddRouteText.Format(RtGraphicsExt.TextFormats.Heading2);
            AddRouteText.Text = ADDROUTTBNTEXT;
            AddRouteButtonMiddle.AddView(AddRouteText);

            //No Roots Panel
            LinearLayout NoRootsBack = new LinearLayout(this);
            NoRootsBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            NoRootsBack.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            NoRootsBack.SetDpPadding(RtGraphicsLayouts, BIGPADDING, BIGPADDING, BIGPADDING, BIGPADDING);
            ContentScrollRoot.AddView(NoRootsBack);

            TextView NoRoutesText = new TextView(this);
            NoRoutesText.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(NOROUTESICONSIZE + BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            NoRoutesText.Format(RtGraphicsExt.TextFormats.Paragraph);
            NoRoutesText.Text = NOROUTESTEXT;
            NoRootsBack.AddView(NoRoutesText);

            ImageView NoRoutesArt = new ImageView(this);
            NoRoutesArt.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            NoRoutesArt.SetImageResource(Resource.Drawable.IconNoRoutes);
            NoRootsBack.AddView(NoRoutesArt);
        }

        private void NavbarOptions_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(Activity_Options));
        }

        private void AddRouteButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(Activity_SelectRoute));
        }
    }
}