// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Attributes;
using XRTK.Definitions.Platforms;
using XRTK.Editor.BuildPipeline;

namespace XRTK.WebXR.Editor.BuildPipeline
{
    [RuntimePlatform(typeof(WebXRPlatform))]
    public class WebXRBuildInfo : BuildInfo
    {
    }
}
