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
    //Station Facility Marker
    public class StationFacilitiesMarker
    {
        //Marker Types Enum
        public enum MarkerTypes
        {
            EnteranceExit
        }

        //Private Variables
        private float _XNorm;
        private float _YNorm;
        private MarkerTypes _MarkerType;
        private ImageView _MarkerView;
        private RtGraphicsLayouts RtGraphicsLayouts;

        //Getters and Setters
        public float XNorm
        {
            set { _XNorm = value; }
            get { return _XNorm; }
        }

        public float YNorm
        {
            set { _YNorm = value; }
            get { return _YNorm; }
        }

        public MarkerTypes MarkerType
        {
            set { _MarkerType = value; }
            get { return _MarkerType; }
        }

        public ImageView MarkerView
        {
            set { _MarkerView = value; }
            get { return _MarkerView; }
        }

        //Initialiser
        public StationFacilitiesMarker(Context Context, float XNorm, float YNorm, MarkerTypes MarkerType)
        {
            //Set Variables
            _XNorm = XNorm;
            _YNorm = YNorm;
            _MarkerType = MarkerType;
            RtGraphicsLayouts = new RtGraphicsLayouts(Context);

            _MarkerView = new ImageView(Context);
            _MarkerView.SetBackgroundResource(Resource.Drawable.IconMapDoor);
        }

        public void AddtoView(RelativeLayout Root)
        {
            RelativeLayout.LayoutParams params1 = new RelativeLayout.LayoutParams(RtGraphicsLayouts.ConvertPxDp(120), RtGraphicsLayouts.ConvertPxDp(120));
            params1.TopMargin = ((int)(RtGraphicsLayouts.ConvertPxDp(1800) * YNorm)) - RtGraphicsLayouts.ConvertPxDp(120);
            params1.LeftMargin = ((int)(RtGraphicsLayouts.ConvertPxDp(1800) * XNorm)) - RtGraphicsLayouts.ConvertPxDp(60);
            Root.AddView(_MarkerView, params1);
        }
    }

    public class StationFacilitiesView : Activity, View.IOnTouchListener
    {
        private RtGraphicsLayouts RtGraphicsLayouts;
        private Context Context;
        private LinearLayout _RootLayout;
        private RelativeLayout _RootPan;
        private RelativeLayout _RootMap;
        private ImageView _MapImage;
        private ScrollView _ParentScrollView;

        public StationFacilitiesView(Context Context)
        {
            this.Context = Context;

            RtGraphicsLayouts = new RtGraphicsLayouts(this.Context);

            _RootLayout = new LinearLayout(this.Context);
            _RootLayout.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            _RootLayout.SetOnTouchListener(this);

            _RootPan = new RelativeLayout(this.Context);
            _RootPan.LayoutParameters = new ViewGroup.LayoutParams(10000, 10000);
            _RootLayout.AddView(_RootPan);

            _RootMap = new RelativeLayout(this.Context);
            _RootMap.LayoutParameters = new ViewGroup.LayoutParams(10000, 10000);
            RelativeLayout.LayoutParams params1 = new RelativeLayout.LayoutParams(10000, 10000);
            params1.TopMargin = -RtGraphicsLayouts.ConvertPxDp(900);
            params1.LeftMargin = -RtGraphicsLayouts.ConvertPxDp(900);
            _RootPan.AddView(_RootMap, params1);

            Redraw("LAN", null);
        }

        public ViewGroup.LayoutParams LayoutParameters
        {
            get { return _RootLayout.LayoutParameters; }
            set { _RootLayout.LayoutParameters = value; }
        }

        public LinearLayout RootLayout
        {
            get { return _RootLayout; }
        }

        double x;
        double y;

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                if (this._ParentScrollView != null)
                    this._ParentScrollView.RequestDisallowInterceptTouchEvent(true);
            }
            else if (e.Action == MotionEventActions.Move)
            {
                double velx = -(e.RawX - x);
                double vely = -(e.RawY - y);

                _RootMap.SetX(_RootMap.GetX() - Convert.ToInt32(velx));
                _RootMap.SetY(_RootMap.GetY() - Convert.ToInt32(vely));

                Console.WriteLine("heeeeeeey");
            }
            else if (e.Action == MotionEventActions.Up)
            {
                if (this._ParentScrollView != null)
                    this._ParentScrollView.RequestDisallowInterceptTouchEvent(false);
            }

            x = e.RawX;
            y = e.RawY;

            return true;
        }

        public void Redraw(string StationCRSCode, ScrollView ParentScroller)
        {
            this._ParentScrollView = ParentScroller;

            _RootMap.RemoveAllViews();

            _MapImage = new ImageView(this.Context);
            float XPos, YPos = 0.0f;

            if (StationCRSCode == "LAN")
            {
                _MapImage.SetBackgroundResource(Resource.Drawable.Map_LAN);

                XPos = 0.4f;
                YPos = -0.1f;

                _RootMap.SetX(RtGraphicsLayouts.ConvertPxDp(1800) * (XPos / 2.0f));
                _RootMap.SetY(RtGraphicsLayouts.ConvertPxDp(1800) * (YPos / 2.0f));

                _MapImage.LayoutParameters = RtGraphicsLayouts.LayoutParameters(1800, 1800);
                _RootMap.AddView(_MapImage);

                //Draw Markers for LAN
                StationFacilitiesMarker MarkerDoor1 = new StationFacilitiesMarker(Context, 0.60f, 0.645f, StationFacilitiesMarker.MarkerTypes.EnteranceExit);
                MarkerDoor1.AddtoView(_RootMap);

                StationFacilitiesMarker MarkerDoor2 = new StationFacilitiesMarker(Context, 0.425f, 0.635f, StationFacilitiesMarker.MarkerTypes.EnteranceExit);
                MarkerDoor2.AddtoView(_RootMap);
            }
            else if (StationCRSCode == "PRE")
            {
                _MapImage.SetBackgroundResource(Resource.Drawable.Map_PRE);

                XPos = 0.4f;
                YPos = -0.1f;

                _RootMap.SetX(RtGraphicsLayouts.ConvertPxDp(1800) * (XPos / 2.0f));
                _RootMap.SetY(RtGraphicsLayouts.ConvertPxDp(1800) * (YPos / 2.0f));

                _MapImage.LayoutParameters = RtGraphicsLayouts.LayoutParameters(1800, 1800);
                _RootMap.AddView(_MapImage);

                //Draw Markers for LAN
                //StationFacilitiesMarker MarkerDoor1 = new StationFacilitiesMarker(Context, 0.60f, 0.645f, StationFacilitiesMarker.MarkerTypes.EnteranceExit);
                //MarkerDoor1.AddtoView(_RootMap);

                //StationFacilitiesMarker MarkerDoor2 = new StationFacilitiesMarker(Context, 0.425f, 0.635f, StationFacilitiesMarker.MarkerTypes.EnteranceExit);
                //MarkerDoor2.AddtoView(_RootMap);
            }
        }
    }

    public static class StationFacilitiesViewExt
    {
        public static void AddView(this ViewGroup ViewGroup, StationFacilitiesView StationFacilitiesView)
        {
            ViewGroup.AddView(StationFacilitiesView.RootLayout);
        }
    }
}