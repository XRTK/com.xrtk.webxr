// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Rufus31415.WebXR;
using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.Devices;
using XRTK.Definitions.InputSystem;
using XRTK.Definitions.Platforms;
using XRTK.Definitions.Utilities;
using XRTK.Interfaces.InputSystem;
using XRTK.Providers.Controllers;
using XRTK.Providers.Controllers.Hands;
using XRTK.Services;
using XRTK.WebXR.Profiles;

namespace XRTK.WebXR.Providers.Controllers
{
    [RuntimePlatform(typeof(WebXRPlatform))]
    [System.Runtime.InteropServices.Guid("4fc04834-14f0-4dc6-b272-cbc0d1bc8964")]
    public class WebXRControllerDataProvider : BaseHandControllerDataProvider
    {
        private readonly Dictionary<Handedness, WebXRHandController> trackedHands = new Dictionary<Handedness, WebXRHandController>();
        private readonly Dictionary<Handedness, WebXRController> trackedControllers = new Dictionary<Handedness, WebXRController>();

        private readonly HandDataPostProcessor postProcessor;


        /// <inheritdoc />
        public WebXRControllerDataProvider(string name, uint priority, WebXRControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (!MixedRealityToolkit.TryGetSystemProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                throw new ArgumentException($"Unable to get a valid {nameof(MixedRealityInputSystemProfile)}!");
            }

            var isGrippingThreshold = profile.GripThreshold != inputSystemProfile.GripThreshold
                ? profile.GripThreshold
                : inputSystemProfile.GripThreshold;

            postProcessor = new HandDataPostProcessor(TrackedPoses, isGrippingThreshold)
            {
                PlatformProvidesPointerPose = true
            };
        }

        public override void Disable()
        {
            base.Disable();

            RemoveAllControllerDevices();
        }

        public override void Update()
        {
            base.Update();

            UpdateController(SimpleWebXR.LeftInput, Handedness.Left);
            UpdateController(SimpleWebXR.RightInput, Handedness.Right);
        }

        #region Controller Management

        protected void UpdateController(WebXRInputSource controller, Handedness handedness)
        {
            if (controller.Available && (controller.IsPositionTracked || controller.Hand.Available))
            {

                if (controller.Hand.Available)
                {

                    RemoveControllerDevice(handedness);
                    GetOrAddHand(handedness)?.UpdateController(postProcessor, controller);
                }
                else
                {
                    RemoveHandDevice(handedness);
                    GetOrAddController(handedness)?.UpdateController(controller);
                }
            }
            else
            {
                RemoveControllerDevice(handedness);
                RemoveHandDevice(handedness);
            }
        }

        private WebXRController GetOrAddController(Handedness handedness)
        {
            if (trackedControllers.ContainsKey(handedness))
            {
                return trackedControllers[handedness];
            }

            var controller = new WebXRController(this, TrackingState.NotTracked, handedness, GetControllerMappingProfile(typeof(WebXRController), handedness));

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            controller.TryRenderControllerModel();

            InputSystem?.RaiseSourceDetected(controller.InputSource, controller);

            trackedControllers.Add(handedness, controller);

            return controller;
        }

        private WebXRHandController GetOrAddHand(Handedness handedness)
        {
            if (trackedHands.ContainsKey(handedness))
            {
                return trackedHands[handedness];
            }

            var controller = new WebXRHandController(this, TrackingState.NotTracked, handedness, GetControllerMappingProfile(typeof(WebXRHandController), handedness));

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            controller.TryRenderControllerModel();

            InputSystem?.RaiseSourceDetected(controller.InputSource, controller);

            trackedHands.Add(handedness, controller);

            return controller;
        }

        private void RemoveControllerDevice(Handedness handedness)
        {
            if (trackedControllers.TryGetValue(handedness, out WebXRController controller))
            {
                RemoveControllerDevice(controller);
            }
        }
        private void RemoveHandDevice(Handedness handedness)
        {
            if (trackedHands.TryGetValue(handedness, out WebXRHandController controller))
            {
                RemoveHandDevice(controller);
            }
        }

        private void RemoveAllControllerDevices()
        {
            if (trackedControllers.Count == 0) return;

            // Create a new list to avoid causing an error removing items from a list currently being iterated on.
            foreach (var controller in new List<WebXRController>(trackedControllers.Values))
            {
                RemoveControllerDevice(controller);
            }
            trackedControllers.Clear();
        }

        private void RemoveControllerDevice(WebXRController controller)
        {
            if (controller == null) return;
            InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            trackedControllers.Remove(controller.ControllerHandedness);
        }

        private void RemoveHandDevice(WebXRHandController controller)
        {
            if (controller == null) return;
            InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            trackedHands.Remove(controller.ControllerHandedness);
        }
        #endregion

    }
}
