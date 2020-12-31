// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Rufus31415.WebXR;
using System;
using UnityEngine;
using XRTK.Definitions.Controllers;
using XRTK.Definitions.Devices;
using XRTK.Definitions.Utilities;
using XRTK.Extensions;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Providers.Controllers;
using XRTK.Services;

namespace XRTK.WebXR.Providers.Controllers
{
    [System.Runtime.InteropServices.Guid("6e771251-8fb3-4742-a279-eebf7a25f6dd")]
    public class WebXRController : BaseController
    {

        public WebXRController() { }

        public WebXRController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
    : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
        }



        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;

        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
                               new MixedRealityInteractionMapping("Spatial Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
 new MixedRealityInteractionMapping("Trigger", AxisType.Digital, "Trigger", DeviceInputType.Select),
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;


        /// <inheritdoc />
        public override void UpdateController()
        {
                if (!Enabled) return;        

            base.UpdateController();



            var controller = this.ControllerHandedness == Handedness.Left ? SimpleWebXR.LeftInput : SimpleWebXR.RightInput;

            if (Interactions == null)
            {
               Enabled = false;
            }

            var lastState = TrackingState;

            IsPositionAvailable = IsRotationAvailable = SimpleWebXR.InSession && controller.Available && controller.IsPositionTracked;

            TrackingState = IsPositionAvailable ? TrackingState.Tracked : TrackingState.NotTracked;

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            // Raise pose changed
            if (IsPositionAvailable)
            {
                var currentControllerPose = new MixedRealityPose(controller.Position, controller.Rotation);

                if (currentControllerPose != lastControllerPose)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentControllerPose);
                    lastControllerPose = currentControllerPose;
                }
            }

            // Raise interactions
            for (int i = 0; i < Interactions?.Length; i++)
            {
                var interactionMapping = Interactions[i];

                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Select:
                        interactionMapping.BoolData = controller.Selected;
                        break;
                    case DeviceInputType.SpatialPointer:
                        interactionMapping.PoseData = lastControllerPose;
                        break;
                    default:
                        Debug.LogError($"Input [{interactionMapping.InputType}] is not handled for this controller [{GetType().Name}]");
                        continue;
                }

                interactionMapping.RaiseInputAction(InputSource, ControllerHandedness);
            }
        }

    }
}
