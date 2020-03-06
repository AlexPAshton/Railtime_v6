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

namespace RtGraphics
{
    //Class handles layout of controls by providing pixel independant 
    //sizes and widths based on screen width minus dp value.
    public class RtGraphicsLayouts
    {
        public const int CONTAIN = -2;
        public const int EXPAND = -1;

        private const float DEFAULTSCREENWIDTH = 720.0f;
        private const int ZERO = 0;

        private float ConvertPxDpMultiplier = 0.0f;
        private float ScreenWidthPixels = 0.0f;
        private float ScreenHeightPixels = 0.0f;

        //Constructor sets the dp conversion values
        public RtGraphicsLayouts(Context Context)
        {
            this.ConvertPxDpMultiplier = Context.Resources.DisplayMetrics.WidthPixels / DEFAULTSCREENWIDTH;
            this.ScreenWidthPixels = Context.Resources.DisplayMetrics.WidthPixels;
            this.ScreenHeightPixels = Context.Resources.DisplayMetrics.HeightPixels;
        }

        //Returns layout parameters which are pixel independant
        public ViewGroup.LayoutParams LayoutParameters(int Width, int Height)
        {
            Width = ConvertPxDp(Width);
            Height = ConvertPxDp(Height);
            
            return new ViewGroup.LayoutParams(Width, Height);
        }

        //Returns the int dp value of a pixel input
        public int ConvertPxDp(int Px)
        {
            if (Px > ZERO)
                Px = (int)Math.Round(Px * ConvertPxDpMultiplier);
            else if (Px < CONTAIN)
                Px = (int)ScreenWidthPixels + (int)Math.Round(Px * ConvertPxDpMultiplier);

            return Px;
        }

        //Sets the status bar colour of the passed window to the passed color
        public void SetColourStatusBar(Window Window, Android.Graphics.Color Colour)
        {
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.SetStatusBarColor(Colour);
        }
    }

    //Class handled graphics related extention methods
    public static class RtGraphicsExt
    {
        public enum TextFormats
        {
            Heading, Heading1, Heading2, Paragraph, Paragraph1, Paragraph2
        }

        private const int BIGFONT = 28;
        private const int MEDFONT = 20;
        private const int SMALLFONT = 16;
        private const int TINYFONT = 14;

        //Extention method that formats a textviews font
        public static void Format(this TextView TextView, TextFormats Format)
        {
            if (Format == TextFormats.Heading)
            {
                TextView.TextSize = MEDFONT;
                TextView.SetTextColor(RtGraphicsColours.White);
            }
            else if (Format == TextFormats.Heading1)
            {
                TextView.TextSize = MEDFONT;
                TextView.SetTextColor(RtGraphicsColours.Orange);
            }
            else if (Format == TextFormats.Heading2)
            {
                TextView.TextSize = BIGFONT;
                TextView.SetTextColor(RtGraphicsColours.Orange);
            }
            else if (Format == TextFormats.Paragraph)
            {
                TextView.TextSize = SMALLFONT;
                TextView.SetTextColor(RtGraphicsColours.Grey);
            }
            else if (Format == TextFormats.Paragraph1)
            {
                TextView.TextSize = TINYFONT;
                TextView.SetTextColor(RtGraphicsColours.GreyLight);
            }
            else if (Format == TextFormats.Paragraph2)
            {
                TextView.TextSize = SMALLFONT;
                TextView.SetTextColor(RtGraphicsColours.Orange);
            }
        }

        //Set dp padding for a view of any of the below types
        public static void SetDpPadding(this TextView TextView, RtGraphicsLayouts RtGraphicsLayouts, int l, int t, int r, int b)
        {
            TextView.SetPadding(RtGraphicsLayouts.ConvertPxDp(l), RtGraphicsLayouts.ConvertPxDp(t), RtGraphicsLayouts.ConvertPxDp(r), RtGraphicsLayouts.ConvertPxDp(b));
        }

        public static void SetDpPadding(this LinearLayout LinearLayout, RtGraphicsLayouts RtGraphicsLayouts, int l, int t, int r, int b)
        {
            LinearLayout.SetPadding(RtGraphicsLayouts.ConvertPxDp(l), RtGraphicsLayouts.ConvertPxDp(t), RtGraphicsLayouts.ConvertPxDp(r), RtGraphicsLayouts.ConvertPxDp(b));
        }

        public static void SetDpPadding(this RelativeLayout RelativeLayout, RtGraphicsLayouts RtGraphicsLayouts, int l, int t, int r, int b)
        {
            RelativeLayout.SetPadding(RtGraphicsLayouts.ConvertPxDp(l), RtGraphicsLayouts.ConvertPxDp(t), RtGraphicsLayouts.ConvertPxDp(r), RtGraphicsLayouts.ConvertPxDp(b));
        }
    }

    //Class that contains static variables of custom colours
    public static class RtGraphicsColours
    {
        public static Android.Graphics.Color Orange = new Android.Graphics.Color(225, 107, 29);//#e16b1d
        public static Android.Graphics.Color OrangeDim = new Android.Graphics.Color(181, 92, 32);
        public static Android.Graphics.Color White = new Android.Graphics.Color(255, 255, 255);
        public static Android.Graphics.Color Grey = new Android.Graphics.Color(100, 100, 100);
        public static Android.Graphics.Color GreyLight = new Android.Graphics.Color(180, 180, 180);
        public static Android.Graphics.Color GreyLightest = new Android.Graphics.Color(210, 210, 210);
        public static Android.Graphics.Color GreyVeryLightest = new Android.Graphics.Color(232, 232, 232);
        public static Android.Graphics.Color SemiTransparentBlack = new Android.Graphics.Color(0, 0, 0, 200);
        public static Android.Graphics.Color DimBlack = new Android.Graphics.Color(49, 23, 6);
        public static Android.Graphics.Color Salmon = new Android.Graphics.Color(215, 205, 148);
        public static Android.Graphics.Color Red = new Android.Graphics.Color(215, 0, 0);

        public static Android.Graphics.Color GetTOCColour(string TOCName, bool Faded = false)
        {
            string TOCColourData = "Arriva Trains Wales=48,197,184#c2c=42,69,73#Caledonian Sleeper=42,69,73#Chiltern Railways=0,191,255#CrossCountry=102,15,33#East Midlands Trains=255,165,0#Eurostar=255,215,0#Gatwick Express=235,30,45#Grand Central=44,56,56#Great Northern=44,56,56#Great Western Railway=10,73,62#Greater Anglia=215,4,40#Heathrow Connect=247,143,30#Heathrow Express=83,46,99#Hull Trains=222,0,92#Island Line=30,144,255#London Northwestern Railway=0,76,69#London Overground=255,117,24#London Underground=255,0,0#Merseyrail=255,242,0#Northern=38,34,98#ScotRail=28,64,116#South Western Railway=43,44,56#Southeastern=0,175,232#Southern=140,198,62#Stansted Express=107,113,122#TfL Rail=163,130,220#Thameslink=233,67,141#TransPennine Express=1,3,133#Virgin Trains=255,0,0#Virgin Trains East Coast=215,14,53#West Midlands Railway=255,130,0#West Midlands Trains=255,130,0";
            string[] TOCColourDatas = TOCColourData.Split(new string[] { "#" }, StringSplitOptions.None);
            
            for (int i = 0;i< TOCColourDatas.Length;i++)
            {
                string[] TOCColourDataParts = TOCColourDatas[i].Split(new string[] { "=" }, StringSplitOptions.None);

                if (TOCColourDataParts[0] == TOCName)
                {
                    string[] ColorParts = TOCColourDataParts[1].Split(new string[] { "," }, StringSplitOptions.None);

                    if (Faded)
                        return new Android.Graphics.Color(byte.Parse(ColorParts[0]), byte.Parse(ColorParts[1]), byte.Parse(ColorParts[2]), (byte)25);
                    else
                        return new Android.Graphics.Color(byte.Parse(ColorParts[0]), byte.Parse(ColorParts[1]), byte.Parse(ColorParts[2]));
                }
            }

            return new Android.Graphics.Color(240, 240, 240);
        }
    }
}