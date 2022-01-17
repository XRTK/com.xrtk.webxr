// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Attributes;
using XRTK.Definitions.CameraSystem;

namespace XRTK.WebXR.Providers.CameraSystem
{
    public class WebXRCameraDataProviderProfile : BaseMixedRealityCameraDataProviderProfile
    {
        [SerializeField]
        private bool hideStartButton = false;

        public bool HideStartButton => hideStartButton;

        [Prefab]
        [SerializeField]
        private GameObject startButtonPrefab = null;

        public GameObject StartButtonPrefab => startButtonPrefab;
    }
}
