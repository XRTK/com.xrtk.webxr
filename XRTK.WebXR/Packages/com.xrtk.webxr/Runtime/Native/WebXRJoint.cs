// Copyright (c) XRTK. All rights reserved.
// Copyright (c) 2020 Florent GIRAUD (Rufus31415) https://github.com/Rufus31415/Simple-WebXR-Unity
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace XRTK.WebXR.Native
{
    /// <summary>
    /// Joint of a hand. Each hand is made up many bones, connected by joints.
    /// </summary>
    public struct WebXRJoint
    {
        public WebXRJoint(Vector3 position, Quaternion quaternion, float radius = float.NaN)
        {
            Position = position;
            Rotation = quaternion;
            Radius = radius;
        }

        /// <summary>
        /// Position of the joint
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Rotation of the joint
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        /// Optional joint radius that can be used to represent the joint has a sphere.
        /// </summary>
        /// <remarks>float.NaN if not supported</remarks>
        public float Radius { get; }
    }
}