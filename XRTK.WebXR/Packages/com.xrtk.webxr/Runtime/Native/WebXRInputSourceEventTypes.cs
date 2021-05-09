// Copyright (c) XRTK. All rights reserved.
// Copyright (c) 2020 Florent GIRAUD (Rufus31415) https://github.com/Rufus31415/Simple-WebXR-Unity
// Licensed under the MIT License. See LICENSE in the project root for license information.

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