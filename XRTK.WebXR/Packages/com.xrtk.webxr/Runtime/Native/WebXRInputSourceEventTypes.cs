using System;

namespace XRTK.WebXR.Native
{
    [Flags]
    internal enum WebXRInputSourceEventTypes
    {
        None = 0,
        SqueezeStart = 1,
        Squeeze = 2,
        SqueezeEnd = 4,
        SelectStart = 8,
        Select = 16,
        SelectEnd = 32
    }
}