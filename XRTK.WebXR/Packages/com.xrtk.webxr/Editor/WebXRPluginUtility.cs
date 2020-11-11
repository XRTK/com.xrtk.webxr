// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XRTK.Extensions;
using XRTK.Utilities.Editor;

namespace XRTK.WebXR.Editor
{
    [InitializeOnLoad]
    public static class WebXRPluginUtility
    {
        private const string GIT_ROOT = "../../../../";
        private const string NATIVE_ROOT_PATH = "/Submodules/Simple-WebXR-Unity/Assets/SimpleWebXR";
        private static readonly string RootPath = PathFinderUtility.ResolvePath<IPathFinder>(typeof(WebXRPathFinder));
        private static readonly string PluginPath = Path.GetFullPath($"{RootPath}/Runtime/Plugins");

        private static string NativeRootPath
        {
            get
            {
                var path = Path.GetFullPath($"{RootPath}{GIT_ROOT}{NATIVE_ROOT_PATH}");

                if (!Directory.Exists(path))
                {
                    path = Path.GetFullPath($"{RootPath}{GIT_ROOT}/Submodules/WebXR{NATIVE_ROOT_PATH}");
                }

                return path;
            }
        }

        private static string NativePluginPath => Path.GetFullPath($"{NativeRootPath}/Plugins");

        static WebXRPluginUtility()
        {
            if (!Directory.Exists(PluginPath) || EditorPreferences.Get($"Reimport_{nameof(WebXRPluginUtility)}", false))
            {
                EditorPreferences.Set($"Reimport_{nameof(WebXRPluginUtility)}", false);

                Debug.Assert(Directory.Exists(NativeRootPath), "Submodule not found! Did you make sure to recursively checkout this branch?");

                if (Directory.Exists(PluginPath))
                {
                    var oldFiles = Directory.GetFiles(PluginPath, "*", SearchOption.AllDirectories).ToList();

                    foreach (var oldFile in oldFiles)
                    {
                        File.Delete(oldFile);
                    }

                    var oldDirectories = Directory.GetDirectories(PluginPath, "*", SearchOption.AllDirectories);

                    foreach (var oldDirectory in oldDirectories)
                    {
                        Directory.Delete(oldDirectory);
                    }

                    Directory.Delete(PluginPath);
                }

                Directory.CreateDirectory(PluginPath);

                var files = new List<string>
                {
                    $"{NativePluginPath}/SimpleWebXR.cs",
                    $"{NativePluginPath}/SimpleWebXR.jslib",
                    $"{NativePluginPath}/SimpleWebXR.jspre"
                };

                foreach (var file in files)
                {
                    File.Copy(file, file.ToForwardSlashes().Replace(NativePluginPath.ToForwardSlashes(), PluginPath.ToForwardSlashes()));
                }

                EditorApplication.delayCall += () => AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }

        [MenuItem("Mixed Reality Toolkit/Tools/WebXR/Reimport Plugins", true)]
        private static bool UpdatePluginValidation() => Directory.Exists(NativeRootPath);

        [MenuItem("Mixed Reality Toolkit/Tools/WebXR/Reimport Plugins", false)]
        private static void UpdatePlugins()
        {
            if (EditorUtility.DisplayDialog("Attention!",
                "In order to reimport the WebXR plugins, we'll need to restart the editor, is this ok?", "Restart", "Cancel"))
            {
                EditorPreferences.Set($"Reimport_{nameof(WebXRPluginUtility)}", true);
                EditorApplication.OpenProject(Directory.GetParent(Application.dataPath).FullName);
            }
        }
    }
}
