// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.PlatformName.Providers.Controllers;

namespace XRTK.PlatformName.Profiles
{
    public class PlatformNameControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(PlatformNameController), Handedness.Left),
                new ControllerDefinition(typeof(PlatformNameController), Handedness.Right)
            };
        }
    }
}
