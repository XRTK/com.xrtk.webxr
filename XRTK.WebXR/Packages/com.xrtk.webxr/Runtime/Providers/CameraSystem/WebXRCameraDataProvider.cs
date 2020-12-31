// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Rufus31415.WebXR;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces.CameraSystem;
using XRTK.Providers.CameraSystem;

namespace XRTK.WebXR.Providers.CameraSystem
{
    [RuntimePlatform(typeof(WebXRPlatform))]
    [System.Runtime.InteropServices.Guid("ae391bc1-67b5-4074-9972-b401b0b9801c")]
    public class WebXRCameraDataProvider : BaseCameraDataProvider
    {
        /// <inheritdoc />
        public WebXRCameraDataProvider(string name, uint priority, WebXRCameraDataProviderProfile profile, IMixedRealityCameraSystem parentService)
            : base(name, priority, profile, parentService)
        {
          var instance =  SimpleWebXR.EnsureInstance();

            instance.HideStartButton = profile.HideStartButton;
        }

        public override void Destroy()
        {
            base.Destroy();
            var webxr = SimpleWebXR.GetInstance();
            if (webxr) Object.DestroyImmediate(webxr.gameObject);
        }
    }
}