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
    [Activity(Label = "Railtime", MainLauncher =true, Icon = "@drawable/IconLauncher", Theme = "@style/Theme.Custom", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]

    public class Activity_Splash : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RtGraphicsLayouts RtGraphicsLayouts = new RtGraphicsLayouts(this);
            RtGraphicsLayouts.SetColourStatusBar(Window, RtGraphicsColours.Orange);

            LinearLayout RootLayout = new LinearLayout(this);
            RootLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            RootLayout.SetBackgroundColor(RtGraphicsColours.Orange);
            SetContentView(RootLayout);

            GenerateSettingsIfDontExist();

            StartActivity(typeof(Activity_Home));
        }

        private void GenerateSettingsIfDontExist()
        {
            if (!RtSettings.DoSettingsExist())
            {
                RtSettingPair[] Settings = new RtSettingPair[] { new RtSettingPair("RRINT", "0"), new RtSettingPair("RRAAM", "0"),
                new RtSettingPair("SSO", "0"),new RtSettingPair("SSBD", "1"), new RtSettingPair("CN", "1"), new RtSettingPair("SSDI", "1"), new RtSettingPair("UGL", "1") };

                RtSettings.CreateSettings(Settings);
            }
        }
    }
}