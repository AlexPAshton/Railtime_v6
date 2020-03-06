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

namespace Railtime_v6
{
    public class RtSettingNotFoundException : Exception
    {
        public RtSettingNotFoundException(string message) : base(message) { }
    }

    public class RtGraphicsLayoutsInvalidLayout : Exception
    {
        public RtGraphicsLayoutsInvalidLayout() { }
    }
    }