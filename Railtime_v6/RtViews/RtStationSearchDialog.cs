using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using RtGraphics;

namespace Railtime_v6
{
    /*
     * Station Search Dialog Class creates a station search dialog and shades the window contents.
     * On selection of a result its returned via an event handler.
     * 
     * This class takes a Dialog ID upon initialisation and when returning a result this is also returned,
     * this helps identify the dialog if multiple exist and all call same event handler.
     */
    public class RtStationSearchDialog
    {
        //Constants
        private const int ZERO = 0;
        private const int DIALOGPADDING = 50;
        private const int SMALLPADDING = 25;
        private const int DIALOGHORIZONTALSHEIGHT = 140;
        private const int DIALOGRESULTHEIGHT = 100;
        private const int MAXSEARCHRESULTS = 100;
        private const int ICONSWIDTH = -515;
        private const int ICONSHEIGHT = 90;
        private const string ERROR_NOSTATIONSFOUND = "No Stations Found.";
        private const string ERROR_GPSTURNEDOFF = "You need to turn GPS on.";
        private const string ERROR_GPSLOADING = "GPS Loading, try again.";

        //Public variables
        public delegate void StationSelectedEventHandler(int DialogID, RtStationData RtStationData);
        public event StationSelectedEventHandler StationSelected;

        //Private Vairbales
        private Context Context;
        private ContextWrapper ContextWrapper;
        private Window Window;
        private RtGraphicsLayouts RtGraphicsLayouts;
        private LinearLayout _RootView;
        private EditText SearchInput;
        private bool _DialogShowing;
        private int DialogID = 0;
        private bool Option_SearchLocation;
        private bool Option_SearchNavigable;

        //Initialiser
        public RtStationSearchDialog(Context Context, ContextWrapper ContextWrapper, Window Window)
        {
            //Set variables
            this.Context = Context;
            this.ContextWrapper = ContextWrapper;
            this.Window = Window;

            //Set variables from settings
            Option_SearchLocation = RtSettings.ReadSetting("SSBD") == "1";
            Option_SearchNavigable = RtSettings.ReadSetting("SSO") == "1";

            //Initialise Graphics Layouts
            RtGraphicsLayouts = new RtGraphicsLayouts(this.Context);

            //Create the view
            GenerateView();
        }

        //Getters
        public LinearLayout RootLayout
        {
            get { return this._RootView; }
        }

        public bool DialogShowing
        {
            get { return this._DialogShowing; }
        }

        //Creates the view
        private void GenerateView()
        {
            //Dialog Shaded Back
            _RootView = new LinearLayout(this.Context);
            _RootView.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            _RootView.SetDpPadding(RtGraphicsLayouts, DIALOGPADDING, DIALOGPADDING, DIALOGPADDING, DIALOGPADDING);
            _RootView.SetBackgroundColor(RtGraphicsColours.SemiTransparentBlack);
            _RootView.Visibility = ViewStates.Gone;
            _RootView.SetGravity(GravityFlags.Center);

            //Dialog Back
            LinearLayout DialogLayout = new LinearLayout(this.Context);
            DialogLayout.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.EXPAND);
            DialogLayout.Orientation = Orientation.Vertical;
            DialogLayout.SetBackgroundResource(Resource.Drawable.StyleCornerBox);
            _RootView.AddView(DialogLayout);

            //Dialog Top Bar
            LinearLayout DialogTopBar = new LinearLayout(this.Context);
            DialogTopBar.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, DIALOGHORIZONTALSHEIGHT);
            DialogTopBar.SetDpPadding(RtGraphicsLayouts, ZERO, SMALLPADDING, ZERO, SMALLPADDING);
            DialogLayout.AddView(DialogTopBar);

            //Location Icon
            ImageView DialogLocation = new ImageView(this.Context);
            DialogLocation.LayoutParameters = RtGraphicsLayouts.LayoutParameters(ICONSWIDTH, ICONSHEIGHT);
            DialogLocation.SetImageResource((Option_SearchLocation) ? Resource.Drawable.Icon_Location : Resource.Drawable.Icon_LocationFaded);
            DialogTopBar.AddView(DialogLocation);

            //Navigable Icon
            ImageView DialogNavigatable = new ImageView(this.Context);
            DialogNavigatable.LayoutParameters = RtGraphicsLayouts.LayoutParameters(ICONSWIDTH, ICONSHEIGHT);
            DialogNavigatable.SetImageResource((Option_SearchNavigable) ? Resource.Drawable.Icon_Navigation : Resource.Drawable.Icon_NavigationFaded);
            DialogTopBar.AddView(DialogNavigatable);

            //Cancel Icon
            ImageView DialogCancel = new ImageView(this.Context);
            DialogCancel.LayoutParameters = RtGraphicsLayouts.LayoutParameters(ICONSWIDTH, ICONSHEIGHT);
            DialogCancel.SetImageResource(Resource.Drawable.IconCancel);
            DialogTopBar.AddView(DialogCancel);

            //Search Input
            SearchInput = new EditText(this.Context);
            SearchInput.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, DIALOGHORIZONTALSHEIGHT);
            SearchInput.SetDpPadding(RtGraphicsLayouts, DIALOGPADDING, SMALLPADDING, DIALOGPADDING, SMALLPADDING);
            SearchInput.SetBackgroundColor(RtGraphicsColours.Orange);
            SearchInput.Format(RtGraphicsExt.TextFormats.Heading);
            DialogLayout.AddView(SearchInput);

            //Results Scroller
            ScrollView ResultsScroll = new ScrollView(this.Context);
            DialogLayout.AddView(ResultsScroll);

            //Results Root
            LinearLayout ResultsRoot = new LinearLayout(this.Context);
            ResultsRoot.Orientation = Orientation.Vertical;
            ResultsScroll.AddView(ResultsRoot);

            //GPS Instance
            RtGPS GPS = new RtGPS((LocationManager)ContextWrapper.GetSystemService(ContextWrapper.LocationService));

            //Event Handlers
            _RootView.Click += delegate { CloseDialog(); };
            DialogCancel.Click += delegate { CloseDialog(); };
            SearchInput.AfterTextChanged += delegate { DrawResults(ResultsRoot, GPS); };
            DialogLocation.Click += delegate { ToggleLocationOption(DialogLocation); };
            DialogNavigatable.Click += delegate { ToggleNavigableOption(DialogNavigatable); };
        }

        //Draw results
        private void DrawResults(LinearLayout ResultsRoot, RtGPS GPS)
        {
            //If GPS sending position
            if ((GPS.Ready && Option_SearchLocation) || !Option_SearchLocation)
            {
                //Search stations
                RtStationData[] data = new RtStations().SearchStations(SearchInput.Text, MAXSEARCHRESULTS, (Option_SearchLocation) ? GPS : null, Option_SearchNavigable);

                //Clear Results View
                ResultsRoot.RemoveAllViews();

                //Loop through stations data
                for (int i = 0; i < data.Length; i++)
                {
                    //Create a local copy of variable for click event
                    RtStationData localdat = data[i];

                    //Result Back
                    LinearLayout ResultBack = new LinearLayout(this.Context);
                    ResultBack.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, DIALOGRESULTHEIGHT);
                    ResultBack.SetDpPadding(RtGraphicsLayouts, SMALLPADDING, SMALLPADDING, SMALLPADDING, SMALLPADDING);
                    ResultBack.SetGravity(GravityFlags.CenterVertical);
                    ResultsRoot.AddView(ResultBack);

                    //Result Station Name
                    TextView ResultStationName = new TextView(this.Context);
                    ResultStationName.Format(RtGraphicsExt.TextFormats.Paragraph);
                    ResultStationName.Text = data[i].StationName;
                    ResultBack.AddView(ResultStationName);

                    //Result Station CRS
                    TextView ResultCRS = new TextView(this.Context);
                    ResultCRS.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
                    ResultCRS.SetDpPadding(RtGraphicsLayouts, ZERO, ZERO, SMALLPADDING, ZERO);
                    ResultCRS.Format(RtGraphicsExt.TextFormats.Paragraph1);
                    ResultCRS.Text = data[i].Code;
                    ResultCRS.Gravity = GravityFlags.Right;
                    ResultBack.AddView(ResultCRS);
                    
                    //Result Back Event
                    ResultBack.Click += delegate
                    {
                        //If callback isn't null, invoke it, causing sleection to be returned and dialog hidden.
                        StationSelected?.Invoke(DialogID, localdat);
                        CloseDialog();
                    };
                }

                //If theres no stations
                if (data.Length == ZERO)
                {
                    ResultsRoot.RemoveAllViews();
                    ResultsRoot.AddView(DrawErrorMessage(ERROR_NOSTATIONSFOUND));
                    GPS = new RtGPS((LocationManager)ContextWrapper.GetSystemService(ContextWrapper.LocationService));
                }
            }
            else if (GPS.NoGPS)
            {
                //If no GPS
                ResultsRoot.RemoveAllViews();
                ResultsRoot.AddView(DrawErrorMessage(ERROR_GPSTURNEDOFF));
                GPS = new RtGPS((LocationManager)ContextWrapper.GetSystemService(ContextWrapper.LocationService));
            }
            else
            {
                //If GPS not ready
                ResultsRoot.RemoveAllViews();
                ResultsRoot.AddView(DrawErrorMessage(ERROR_GPSLOADING));
            }
        }

        //Returns text view error mesage
        public TextView DrawErrorMessage(string Message)
        {
            TextView ErrorMessage = new TextView(this.Context);
            ErrorMessage.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, DIALOGHORIZONTALSHEIGHT);
            ErrorMessage.Format(RtGraphicsExt.TextFormats.Paragraph1);
            ErrorMessage.Text = Message;
            ErrorMessage.Gravity = GravityFlags.Center;
            return ErrorMessage;
        }

        //Method for toggling use location in search
        private void ToggleLocationOption(ImageView View)
        {
            Option_SearchLocation = !Option_SearchLocation;
            View.SetImageResource((Option_SearchLocation) ? Resource.Drawable.Icon_Location : Resource.Drawable.Icon_LocationFaded);
            RtSettings.ChangeSetting("SSBD", Option_SearchLocation ? "1" : "0");
        }

        //Method for toggling use navagble only in search
        private void ToggleNavigableOption(ImageView View)
        {
            Option_SearchNavigable = !Option_SearchNavigable;
            View.SetImageResource((Option_SearchNavigable) ? Resource.Drawable.Icon_Navigation : Resource.Drawable.Icon_NavigationFaded);
            RtSettings.ChangeSetting("SSO", Option_SearchLocation ? "1" : "0");
        }

        //Method to show dialog
        public void ShowDialog(string SearchValue = "", int DialogID = 0)
        {
            this.DialogID = DialogID;

            SearchInput.Text = SearchValue;
            SearchInput.SetSelection(SearchValue.Length);
            RtGraphicsLayouts.SetColourStatusBar(Window, RtGraphicsColours.DimBlack);
            _RootView.Visibility = ViewStates.Visible;
            SearchInput.RequestFocus();
            InputMethodManager imm = (InputMethodManager)ContextWrapper.GetSystemService(Context.InputMethodService);
            imm.ShowSoftInput(SearchInput, ShowFlags.Implicit);

            _DialogShowing = true;
        }

        //Method to hide dialog
        public void CloseDialog()
        {
            RtGraphicsLayouts.SetColourStatusBar(Window, RtGraphicsColours.Orange);
            _RootView.Visibility = ViewStates.Gone;
            HideKeyboard(SearchInput);
            _DialogShowing = false;
        }

        //Hide Keyboard
        protected void HideKeyboard(View view)
        {
            InputMethodManager inp = (InputMethodManager)ContextWrapper.GetSystemService(ContextWrapper.InputMethodService);
            inp.HideSoftInputFromWindow(view.WindowToken, HideSoftInputFlags.NotAlways);
        }
    }

    public static class RtStationSearchDialogExt
    {
        public static void AddView(this ViewGroup ViewGroup, RtStationSearchDialog RtStationSearchDialog)
        {
            ViewGroup.AddView(RtStationSearchDialog.RootLayout);
        }
    }
}