// Copyright (c) XRTK. All rights reserved.
// Copyright (c) 2020 Florent GIRAUD (Rufus31415) https://github.com/Rufus31415/Simple-WebXR-Unity
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace XRTK.WebXR.Native
{
    /// <summary>
    /// Describes spaces that application can use to establish a spatial relationship with the user�s physical environment
    /// </summary>
    internal enum WebXRReferenceSpaces
    {
        /// <summary>
        /// Represents a tracking space with a native origin which tracks the position and orientation of the viewer. The y axis equals 0 at head level when session starts.
        /// </summary>
        Viewer,

        /// <summary>
        /// Represents a tracking space with a native origin at the floor in a safe position for the user to stand. The y axis equals 0 at floor level, with the x and z position and orientation initialized based on the conventions of the underlying platform.
        /// </summary>
        LocalFloor
    }
}
