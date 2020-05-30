// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace XRTK.Definitions.Platforms
{
    /// <summary>
    /// Used by the XRTK to signal that the feature is available on the WebXR platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("014aed5a-a0d8-4db3-a3c0-0b33ff7c065b")]
    public class WebXRPlatform : BasePlatform
    {
        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override bool IsBuildTargetAvailable
        {
            get
            {
                return false;
            }
        }
    }
}