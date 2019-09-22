using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace Nasiye.CustomRenderers
{
    public class CustomMap : Map
    {
        internal List<CustomPin> CustomPins { get; set; }
    }
}
