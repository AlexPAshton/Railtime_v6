using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RtGraphics;

namespace Railtime_v6
{
    public static class RtCheckboxViewExt
    {
        public static void AddView(this ViewGroup ViewGroup, RtCheckboxView RtCheckboxView)
        {
            ViewGroup.AddView(RtCheckboxView.CheckboxBack);
        }
    }

    public class RtCheckboxView
    {
        private const int CHECKBOXSIZE = 40;
        private const int SMALLPADDING = 15;
        private const int BIGPADDING = 50;

        public delegate void CallbackEventHandler(string CheckboxText);
        public event CallbackEventHandler Callback;
        public LinearLayout CheckboxBack;

        private bool _Checked;
        private string _Text;
        private string _Description;
        private ViewGroup.LayoutParams _LayoutParameters;
        private Context Context;
        private RtGraphicsLayouts RtGraphicsLayouts;
        private LinearLayout LeftSide;
        private TextView TextView;
        private TextView DescriptionView;
        private LinearLayout Checkbox;

        public RtCheckboxView(Context Context)
        {
            this.Context = Context;

            RtGraphicsLayouts = new RtGraphicsLayouts(Context);

            CheckboxBack = new LinearLayout(Context);
            CheckboxBack.SetDpPadding(RtGraphicsLayouts, 0, SMALLPADDING, 0, SMALLPADDING);
            CheckboxBack.Click += CheckboxBack_Click;

            LeftSide = new LinearLayout(Context);
            LeftSide.Orientation = Orientation.Vertical;

            TextView = new TextView(Context);
            TextView.Format(RtGraphicsExt.TextFormats.Paragraph);

            DescriptionView = new TextView(Context);
            DescriptionView.Format(RtGraphicsExt.TextFormats.Paragraph1);
            DescriptionView.Visibility = ViewStates.Gone;

            Checkbox = new LinearLayout(Context);
            Checkbox.SetBackgroundResource(Resource.Drawable.IconCheckbox);
        }

        public ViewGroup.LayoutParams LayoutParameters
        {
            get { return _LayoutParameters; }
            set
            {
                _LayoutParameters = value;
                CheckboxBack.LayoutParameters = _LayoutParameters;

                LeftSide.LayoutParameters = new ViewGroup.LayoutParams(_LayoutParameters.Width - RtGraphicsLayouts.ConvertPxDp(CHECKBOXSIZE), RtGraphicsLayouts.CONTAIN);
                CheckboxBack.AddView(LeftSide);

                TextView.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
                LeftSide.AddView(TextView);

                DescriptionView.LayoutParameters = RtGraphicsLayouts.LayoutParameters(RtGraphicsLayouts.EXPAND, RtGraphicsLayouts.CONTAIN);
                LeftSide.AddView(DescriptionView);

                Checkbox.LayoutParameters = RtGraphicsLayouts.LayoutParameters(CHECKBOXSIZE, CHECKBOXSIZE);
                CheckboxBack.AddView(Checkbox);

                if (_LayoutParameters.Width == RtGraphicsLayouts.EXPAND)
                    throw new RtGraphicsLayoutsInvalidLayout();
            }
        }

        public string Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                TextView.Text = _Text;
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                DescriptionView.Text = _Description;
                DescriptionView.Visibility = ViewStates.Visible;
            }
        }

        public bool Checked
        {
            get {  return _Checked; }
            set
            {
                _Checked = value;
                UpdateStateImage();
            }
        }

        private void CheckboxBack_Click(object sender, EventArgs e)
        {
            _Checked = !_Checked;
            UpdateStateImage();
        }


        private void UpdateStateImage()
        {
            if (_Checked)
                Checkbox.SetBackgroundResource(Resource.Drawable.IconCheckbox_Checked);
            else
                Checkbox.SetBackgroundResource(Resource.Drawable.IconCheckbox);

            Callback?.Invoke(_Text);
        }
    }
}