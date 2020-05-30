// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Attributes;
using XRTK.Definitions.CameraSystem;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces.CameraSystem;
using XRTK.Providers.CameraSystem;

namespace XRTK.PlatformName.Providers.CameraSystem
{
    [RuntimePlatform(typeof(PlatformNamePlatform))]
    [System.Runtime.InteropServices.Guid("#INSERT_GUID_HERE#")]
    public class PlatformNameCameraDataProvider : BaseCameraDataProvider
    {
        /// <inheritdoc />
        public PlatformNameCameraDataProvider(string name, uint priority, BaseMixedRealityCameraDataProviderProfile profile, IMixedRealityCameraSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }
    }
}