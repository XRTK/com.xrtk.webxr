// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Attributes;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces.InputSystem;
using XRTK.WebXR.Profiles;
using XRTK.Providers.Controllers;
using Rufus31415.WebXR;
using System;

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
            SimpleWebXR.SessionStart.AddListener(OnSessionStart);
            SimpleWebXR.SessionEnd.AddListener(OnSessionEnd);
        }

        private void OnSessionStart()
        {
            //RaiseSourceDetected
        }
        private void OnSessionEnd()
        {
            //RaiseSourceLost
        }

        public override void Destroy()
        {
            base.Destroy();

            // remove handlers
        }

    }
}
