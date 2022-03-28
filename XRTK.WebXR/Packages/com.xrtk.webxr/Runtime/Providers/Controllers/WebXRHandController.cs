// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Providers.Controllers.Hands;
using XRTK.WebXR.Native;

namespace XRTK.WebXR.Providers.Controllers
{
    [System.Runtime.InteropServices.Guid("6e771251-8fb5-4742-a279-adbf7a25f6dd")]
    public class WebXRHandController : MixedRealityHandController
    {
        public WebXRHandController() { }

        public WebXRHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }

        internal void UpdateController(HandDataPostProcessor postProcessor, WebXRInputSource controller)
        {
            if (!Enabled)
            {
                return;
            }

            var handData = new HandData
            {
                TrackingState = TrackingState.Tracked,
                UpdatedAt = DateTimeOffset.UtcNow.Ticks,
                RootPose = MixedRealityPose.ZeroIdentity
            };

            var joints = new MixedRealityPose[HandData.JointCount];

            for (int i = 0; i < WebXRHand.JOINT_COUNT; i++)
            {
                joints[(i + 1)] = new MixedRealityPose(controller.Hand.Joints[i].Position, controller.Hand.Joints[i].Rotation);
            }

            joints[0] = joints[1];

            handData.Joints = joints;
            handData.PointerPose = controller.IsPositionTracked ? new MixedRealityPose(controller.Position, controller.Rotation) : new MixedRealityPose(controller.Hand.Joints[WebXRHand.WRIST].Position, controller.Hand.Joints[WebXRHand.WRIST].Rotation);

            handData.Mesh = HandMeshData.Empty;

            var data = postProcessor.PostProcess(ControllerHandedness, handData);

            UpdateController(data);
        }
    }
}
