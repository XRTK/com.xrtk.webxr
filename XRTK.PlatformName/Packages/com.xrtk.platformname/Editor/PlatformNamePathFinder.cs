// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Utilities.Editor;

namespace XRTK.PlatformName.Editor
{
    /// <summary>
    /// Dummy scriptable object used to find the relative path of the com.xrtk.platformname.
    /// </summary>
    ///// <inheritdoc cref="IPathFinder" />
    public class PlatformNamePathFinder : ScriptableObject, IPathFinder
    {
        ///// <inheritdoc />
        public string Location => $"/Editor/{nameof(PlatformNamePathFinder)}.cs";
    }
}
