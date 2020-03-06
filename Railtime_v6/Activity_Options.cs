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
    public class Activity_Options : Activity
    {
        private const string NAVBARTEXT = "Options";
        private const string SETTINGTEXT = "Remove Routes if no Train";
        private const string SETTINGTEXT1 = "Remove Routes after a month";
        private const string SETTINGTEXT2 = "Supported Stations Only";
        private const string SETTINGTEXT3 = "Sort Stations by Distance";
        private const string SETTINGTEXT4 = "Commuter Notify";
        private const string SETTINGTEXT5 = "Show Service Disruption Info";
        private const string SETTINGTEXT6 = "Use GPS Location";
        private const string SETTINGDESC = "If the train you've selected for a route doesn't exist, remove the route.";
        private const string SETTINGDESC2 = "Only show stations in the search that the app can nagivate within.";
        private const string SETTINGDESC3 = "Sort stations in the search by closest first if GPS enabled.";
        private const string SETTINGDESC4 = "Notify of upcoming route based on how long it takes you to get to the station. Also show expected time of arrival at destination based on how long it takes you to get to destination.";
        private const string ONE = "1";
        private const int NAVBARHEIGHT = 140;
        private const int NAVBARPADDING = 40;
        private const int SMALLPADDING = 25;
        private const int BIGPADDING = 50;
        private const int ZERO = 0;


        private RtCheckboxView SettingsCheckbox;
        private RtCheckboxView SettingsCheckbox1;
        private RtCheckboxView SettingsCheckbox2;
        private RtCheckboxView SettingsCheckbox3;
        private RtCheckboxView SettingsCheckbox4;
        private RtCheckboxView SettingsCheckbox5;
        private RtCheckboxView SettingsCheckbox6;

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
            ContentScrollRoot.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, SMALLPADDING, SMALLPADDING, SMALLPADDING);
            ContentScrollerRoot.AddView(ContentScrollRoot);


            //Settings Panel 1
            LinearLayout SettingsBack = new LinearLayout(this);
            SettingsBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            SettingsBack.Orientation = Orientation.Vertical;
            SettingsBack.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            SettingsBack.SetDpPadding(RtGraphicsLayouts, BIGPADDING, BIGPADDING, BIGPADDING, BIGPADDING);
            ContentScrollRoot.AddView(SettingsBack);

            SettingsCheckbox = new RtCheckboxView(this);
            SettingsCheckbox.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            SettingsCheckbox.Callback += SettingsCheckbox_Callback;
            SettingsCheckbox.Checked = (RtSettings.ReadSetting("RRINT") == ONE);
            SettingsCheckbox.Text = SETTINGTEXT;
            SettingsCheckbox.Description = SETTINGDESC;
            SettingsBack.AddView(SettingsCheckbox);

            SettingsCheckbox1 = new RtCheckboxView(this);
            SettingsCheckbox1.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            SettingsCheckbox1.Callback += SettingsCheckbox_Callback;
            SettingsCheckbox1.Checked = (RtSettings.ReadSetting("RRAAM") == ONE);
            SettingsCheckbox1.Text = SETTINGTEXT1;
            SettingsBack.AddView(SettingsCheckbox1);

            LinearLayout SettingsBackSpacer = new LinearLayout(this);
            SettingsBackSpacer.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, SMALLPADDING);
            ContentScrollRoot.AddView(SettingsBackSpacer);

            //Settings Panel 2
            LinearLayout SettingsBack1 = new LinearLayout(this);
            SettingsBack1.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            SettingsBack1.Orientation = Orientation.Vertical;
            SettingsBack1.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            SettingsBack1.SetDpPadding(RtGraphicsLayouts, BIGPADDING, BIGPADDING, BIGPADDING, BIGPADDING);
            ContentScrollRoot.AddView(SettingsBack1);

            SettingsCheckbox2 = new RtCheckboxView(this);
            SettingsCheckbox2.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            SettingsCheckbox2.Callback += SettingsCheckbox_Callback;
            SettingsCheckbox2.Checked = (RtSettings.ReadSetting("SSO") == ONE);
            SettingsCheckbox2.Text = SETTINGTEXT2;
            SettingsCheckbox2.Description = SETTINGDESC2;
            SettingsBack1.AddView(SettingsCheckbox2);

            SettingsCheckbox3 = new RtCheckboxView(this);
            SettingsCheckbox3.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            SettingsCheckbox3.Callback += SettingsCheckbox_Callback;
            SettingsCheckbox3.Checked = (RtSettings.ReadSetting("SSBD") == ONE);
            SettingsCheckbox3.Text = SETTINGTEXT3;
            SettingsCheckbox3.Description = SETTINGDESC3;
            SettingsBack1.AddView(SettingsCheckbox3);

            LinearLayout SettingsBackSpacer1 = new LinearLayout(this);
            SettingsBackSpacer1.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, SMALLPADDING);
            ContentScrollRoot.AddView(SettingsBackSpacer1);

            //Settings Panel 3
            LinearLayout SettingsBack2 = new LinearLayout(this);
            SettingsBack2.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
            SettingsBack2.Orientation = Orientation.Vertical;
            SettingsBack2.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            SettingsBack2.SetDpPadding(RtGraphicsLayouts, BIGPADDING, BIGPADDING, BIGPADDING, BIGPADDING);
            ContentScrollRoot.AddView(SettingsBack2);

            SettingsCheckbox4 = new RtCheckboxView(this);
            SettingsCheckbox4.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            SettingsCheckbox4.Callback += SettingsCheckbox_Callback;
            SettingsCheckbox4.Checked = (RtSettings.ReadSetting("CN") == ONE);
            SettingsCheckbox4.Text = SETTINGTEXT4;
            SettingsCheckbox4.Description = SETTINGDESC4;
            SettingsBack2.AddView(SettingsCheckbox4);

            SettingsCheckbox5 = new RtCheckboxView(this);
            SettingsCheckbox5.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            SettingsCheckbox5.Callback += SettingsCheckbox_Callback;
            SettingsCheckbox5.Checked = (RtSettings.ReadSetting("SSDI") == ONE);
            SettingsCheckbox5.Text = SETTINGTEXT5;
            SettingsBack2.AddView(SettingsCheckbox5);

            SettingsCheckbox6 = new RtCheckboxView(this);
            SettingsCheckbox6.LayoutParameters = RtGraphicsLayouts.LayoutParameters(-(BIGPADDING + BIGPADDING + SMALLPADDING + SMALLPADDING), RtGraphicsLayouts.CONTAIN);
            SettingsCheckbox6.Callback += SettingsCheckbox_Callback;
            SettingsCheckbox6.Checked = (RtSettings.ReadSetting("UGL") == ONE);
            SettingsCheckbox6.Text = SETTINGTEXT6;
            SettingsBack2.AddView(SettingsCheckbox6);
        }

        private void SettingsCheckbox_Callback(string CheckboxText)
        {
            if (SettingsCheckbox != null && SettingsCheckbox1 != null && SettingsCheckbox2 != null && SettingsCheckbox3 != null &&
                SettingsCheckbox4 != null && SettingsCheckbox5 != null && SettingsCheckbox6 != null)
            {
                RtSettingPair[] Settings = new RtSettingPair[] { new RtSettingPair("RRINT", (SettingsCheckbox.Checked ? ONE: "0")),
                new RtSettingPair("RRAAM", (SettingsCheckbox1.Checked ? ONE: "0")), new RtSettingPair("SSO", (SettingsCheckbox2.Checked ? ONE: "0")),
                new RtSettingPair("SSBD", (SettingsCheckbox3.Checked ? ONE: "0")), new RtSettingPair("CN", (SettingsCheckbox4.Checked ? ONE: "0")),
                new RtSettingPair("SSDI", (SettingsCheckbox5.Checked ? ONE: "0")), new RtSettingPair("UGL", (SettingsCheckbox6.Checked ? ONE: "0")) };

                RtSettings.CreateSettings(Settings);
            }
        }

        private void NavbarBack_Click(object sender, EventArgs e)
        {
            this.OnBackPressed();
        }
    }
}