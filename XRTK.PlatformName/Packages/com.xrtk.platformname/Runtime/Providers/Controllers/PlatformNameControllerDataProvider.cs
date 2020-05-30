// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Attributes;
using XRTK.Definitions.Platforms;
using XRTK.Interfaces.InputSystem;
using XRTK.PlatformName.Profiles;
using XRTK.Providers.Controllers;

namespace XRTK.PlatformName.Providers.Controllers
{
    [RuntimePlatform(typeof(PlatformNamePlatform))]
    [System.Runtime.InteropServices.Guid("#INSERT_GUID_HERE#")]
    public class PlatformNameControllerDataProvider : BaseControllerDataProvider
    {
        /// <inheritdoc />
        public PlatformNameControllerDataProvider(string name, uint priority, PlatformNameControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }
    }
}
