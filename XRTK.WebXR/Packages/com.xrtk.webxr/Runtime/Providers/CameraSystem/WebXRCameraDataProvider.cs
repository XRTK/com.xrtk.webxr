// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Platforms;
using XRTK.Extensions;
using XRTK.Interfaces.CameraSystem;
using XRTK.Providers.CameraSystem;
using XRTK.WebXR.Native;

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
            hideStartButton = profile.HideStartButton;
            startButtonPrefab = profile.StartButtonPrefab;
            WebXRNativeBindings.FallbackUserHeight = profile.DefaultHeadHeight;
        }

        private bool hideStartButton;

        private GameObject startButtonObject;
        private GameObject startButtonPrefab;

        #region IMixedRealityService

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (Application.isPlaying)
            {
                if (!hideStartButton && startButtonPrefab != null)
                {
                    startButtonObject = Object.Instantiate(startButtonPrefab);
                }
                else
                {
                    WebXRNativeBindings.StartSession();
                }
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            WebXRNativeBindings.UpdateWebXR();
        }

        public override void Disable()
        {
            base.Disable();

            if (Application.isPlaying)
            {
                startButtonObject.Destroy();
                WebXRNativeBindings.EndSession();
            }
        }

        #endregion IMixedRealityService
    }
}
