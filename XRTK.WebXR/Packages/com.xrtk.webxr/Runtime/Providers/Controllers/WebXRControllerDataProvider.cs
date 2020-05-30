// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Attributes;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces.InputSystem;
using XRTK.WebXR.Profiles;
using XRTK.Providers.Controllers;

namespace XRTK.WebXR.Providers.Controllers
{
    [RuntimePlatform(typeof(WebXRPlatform))]
    [System.Runtime.InteropServices.Guid("4fc04834-14f0-4dc6-b272-cbc0d1bc8964")]
    public class WebXRControllerDataProvider : BaseControllerDataProvider
    {
        /// <inheritdoc />
        public WebXRControllerDataProvider(string name, uint priority, WebXRControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }
    }
}
