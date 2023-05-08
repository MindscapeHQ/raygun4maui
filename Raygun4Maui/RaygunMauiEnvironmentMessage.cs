﻿using Mindscape.Raygun4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raygun4Maui
{
    internal class RaygunMauiEnvironmentMessage : RaygunEnvironmentMessage
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public double ResolutionScale { get; set; }
        public int ColorDepth { get; set; }
        public string CurrentOrientation { get; set; }
        public string DeviceManufacturer { get; set; }
        public string Model { get; set; }
        public string DeviceName { get; set; }
        public string Platform { get; set; }
        public string OS { get; set; }
    }
}
