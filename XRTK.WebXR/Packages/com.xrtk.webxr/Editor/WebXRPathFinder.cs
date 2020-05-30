// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Utilities.Editor;

namespace XRTK.WebXR.Editor
{
    /// <summary>
    /// Dummy scriptable object used to find the relative path of the com.xrtk.webxr.
    /// </summary>
    ///// <inheritdoc cref="IPathFinder" />
    public class WebXRPathFinder : ScriptableObject, IPathFinder
    {
        ///// <inheritdoc />
        public string Location => $"/Editor/{nameof(WebXRPathFinder)}.cs";
    }
}
