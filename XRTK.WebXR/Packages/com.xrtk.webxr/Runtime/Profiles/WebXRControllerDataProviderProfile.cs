// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Definitions.Utilities;
using XRTK.WebXR.Providers.Controllers;

namespace XRTK.WebXR.Profiles
{
    public class WebXRControllerDataProviderProfile : BaseHandControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(WebXRController), Handedness.Left),
                new ControllerDefinition(typeof(WebXRController), Handedness.Right)
            };
        }
    }
}

