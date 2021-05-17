// Copyright (c) XRTK. All rights reserved.
// Copyright (c) 2020 Florent GIRAUD (Rufus31415) https://github.com/Rufus31415/Simple-WebXR-Unity
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using XRTK.Utilities.Async;
using Object = UnityEngine.Object;

namespace XRTK.WebXR.Native
{
    /// <summary>
    /// Based on https://github.com/Rufus31415/Simple-WebXR-Unity
    /// </summary>
    /// <remarks>
    /// The data (projection matrix, position and rotation) are shared with javascript via arrays.
    /// See also files SimpleWebXR.jslib and SimpleWebXR.jspre
    /// </remarks>
    public static class WebXRNativeBindings
    {
        #region Dll Calls

#if UNITY_WEBGL && !UNITY_EDITOR // If executed in browser

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void InternalStartSession();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void InternalEndSession();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void InitWebXR(float[] dataArray, int length, byte[] byteArray, int byteArrayLength, float[] handDataArray, int handDataArrayLength);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern bool InternalIsArSupported();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern bool InternalIsVrSupported();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern bool InternalIsOVRMultiview2Supported();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern bool InternalIsOculusMultiviewSupported();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void InternalHitTestStart();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void InternalHitTestCancel();

        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void InternalGetDeviceOrientation(float[] orientationArray, byte[] orientationInfo);

#else // if executed with Unity editor

        private static void InternalEndSession() { }

        private static void InternalStartSession() { }

        private static void InitWebXR(float[] dataArray, int length, byte[] byteArray, int byteArrayLength, float[] handDataArray, int handDataArrayLength) { }

        private static bool InternalIsArSupported() => false;

        private static bool InternalIsVrSupported() => false;

        private static bool InternalIsOVRMultiview2Supported() => false;

        private static bool InternalIsOculusMultiviewSupported() => false;

        private static void InternalGetDeviceOrientation(float[] orientationArray, byte[] orientationInfo) { }

        private static void InternalHitTestStart() { }

        private static void InternalHitTestCancel() { }
#endif

        #endregion Dll Calls

        /// <summary>
        /// Event triggered when a session has started
        /// </summary>
        public static event Action SessionStart;

        /// <summary>
        /// Event triggered when a session has ended
        /// </summary>
        public static event Action SessionEnd;

        /// <summary>
        /// Indicates if a WebXR session is running
        /// </summary>
        public static bool InSession { get; private set; }

        /// <summary>
        /// Event triggered when the browser triggers a XRSession.inputsourceschange event, which means a input sources has been added or removed.
        /// </summary>
        public static event Action InputSourcesChange;

        /// <summary>
        /// Left input controller data
        /// </summary>
        public static readonly WebXRInputSource LeftInput = new WebXRInputSource(WebXRHandedness.Left);

        /// <summary>
        /// Right input controller data
        /// </summary>
        public static readonly WebXRInputSource RightInput = new WebXRInputSource(WebXRHandedness.Right);

        /// <summary>
        /// Indicates that a hit test is available in HitTestPosition and HitTestRotation
        /// </summary>
        public static bool HitTestInProgress => ByteArray[48] != 0;

        /// <summary>
        /// Indicates hit test is supported
        /// The immersive session should be started to estimate this capability
        /// </summary>
        public static bool HitTestSupported => ByteArray[49] != 0;

        /// <summary>
        /// Position of the hit test between head ray and the real world
        /// </summary>
        public static Vector3 HitTestPosition { get; private set; }

        /// <summary>
        /// Orientation of the hit test normal between head ray and the real world
        /// </summary>
        public static Quaternion HitTestRotation { get; private set; }
        /// <summary>
        /// Check if Augmented Reality (AR) is supported
        /// </summary>
        /// <remarks>It returns the result of navigator.xr.isSessionSupported('immersive-ar')</remarks>
        /// <returns>True if AR is supported</returns>
        public static bool IsArSupported
        {
            get
            {
                Initialize();
                return InternalIsArSupported();
            }
        }

        /// <summary>
        /// Check if Virtual Reality (VR) is supported
        /// </summary>
        /// <remarks>It returns the result of navigator.xr.isSessionSupported('immersive-vr')</remarks>
        /// <returns>True if VR is supported</returns>
        public static bool IsVrSupported
        {
            get
            {
                Initialize();
                return InternalIsVrSupported();
            }
        }

        /// <summary>
        /// Check if OVR Multiview2 extension is supported.
        /// </summary>
        /// <returns>True, if multiview2 is supported.</returns>
        public static bool IsMultiview2Supported
        {
            get
            {
                Initialize();
                return InternalIsOVRMultiview2Supported();
            }
        }

        /// <summary>
        /// Check if Oculus Multiview with sampling is supported.
        /// </summary>
        /// <returns>True, if Oculus multiview with sampling is supported.</returns>
        public static bool IsOculusMultiviewSampledSupported
        {
            get
            {
                Initialize();
                return InternalIsOculusMultiviewSupported();
            }
        }

        /// <summary>
        /// Initialize the binding with the WebXR API, via shared arrays
        /// </summary>
        private static void Initialize()
        {
            if (isInitialized) { return; }
            InitWebXR(DataArray, DataArray.Length, ByteArray, ByteArray.Length, HandData, HandData.Length);
            isInitialized = true;
        }


        /// <summary>
        /// Triggers the start of a WebXR immersive session
        /// </summary>
        public static async Task<bool> StartSession()
        {
            if (!IsArSupported && !IsVrSupported)
            {
                Debug.LogWarning("WebXR not supported for this device!");
                return false;
            }

            await EnableDisableVRMode(true);

            Initialize();
            if (InternalInSession) { return true; }
            InternalStartSession();
            return true;
        }

        /// <summary>
        /// Ends the current WebXR immersive session
        /// </summary>
        public static async void EndSession()
        {
            if (!InternalInSession) { return; }

            await EnableDisableVRMode(false);

            InternalHitTestCancel();
            InternalEndSession();
        }

        private static IEnumerator EnableDisableVRMode(bool bEnable)
        {
            XRSettings.enabled = bEnable;

            if (bEnable)
            {
                yield return new WaitForEndOfFrame();
                XRSettings.LoadDeviceByName("MockHMD");
                yield return new WaitForEndOfFrame();
                XRSettings.enabled = true;
            }
            else
            {
                yield return new WaitForEndOfFrame();
                XRSettings.LoadDeviceByName("None");
                yield return new WaitForEndOfFrame();
                XRSettings.enabled = false;
            }

            yield return new WaitForEndOfFrame();

            Debug.Log($"UnityEngine.XR.XRSettings.enabled = {XRSettings.enabled}");
            Debug.Log($"UnityEngine.XR.XRSettings.loadedDeviceName = {XRSettings.loadedDeviceName}");
        }

        /// <summary>
        /// Starts hit test
        /// </summary>
        public static void HitTestStart()
        {
            Initialize();
            InternalHitTestStart();
        }

        /// <summary>
        /// Ends hit test
        /// </summary>
        public static void HitTestCancel()
        {
            Initialize();
            InternalHitTestCancel();
        }

        /// <summary>
        /// Update cameras (eyes), input sources and propagates WebXR events to Unity. This function should be called once per frame.
        /// </summary>
        public static void UpdateWebXR()
        {
            if (!InternalInSession) { return; }

            //UpdateCamera(WebXRViewEyes.Left);
            //UpdateCamera(WebXRViewEyes.Right);

            LeftInput.Available = ByteArray[44] != 0;
            RightInput.Available = ByteArray[45] != 0;

            if (LeftInput.Available)
            {
                UpdateInput(LeftInput);
            }

            if (RightInput.Available)
            {
                UpdateInput(RightInput);
            }

            // Input source change event
            if (ByteArray[3] != 0)
            {
                InputSourcesChange?.Invoke();
                ByteArray[3] = 0;
            }

            UpdateHitTest();

            // Session state changed invoked when all gamepads and cameras are updated
            if (InternalInSession && !InSession) //New session detected
            {
                InSession = true;
                SessionStart?.Invoke();
            }
            else if (InSession && !InternalInSession) // End of session detected
            {
                InSession = false;
                SessionEnd?.Invoke();
            }
        }

        /// <summary>
        /// Applies haptic pulse feedback to a controller
        /// </summary>
        /// <param name="hand">Controller to apply feedback</param>
        /// <param name="intensity">Feedback strength between 0 and 1</param>
        /// <param name="duration">Feedback duration in milliseconds</param>
        internal static void HapticPulse(WebXRHandedness hand, float intensity, float duration)
        {
            if (isInitialized)
            {
                DataArray[101 + (int)hand] = intensity;
                DataArray[103 + (int)hand] = duration;
            }
        }

        /// <summary>
        /// A human-readable presentation of the WebXR session and capabilities
        /// </summary>
        public new static string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(nameof(WebXRNativeBindings));
            sb.Append("  In session : ");
            sb.AppendLine(InSession ? "Yes" : "No");

            sb.Append("  AR supported : ");
            sb.AppendLine(IsArSupported ? "Yes" : "No");

            sb.Append("  VR supported : ");
            sb.AppendLine(IsVrSupported ? "Yes" : "No");

            sb.Append("  User height : ");
            sb.AppendLine(UserHeight.ToString("0.0"));

            sb.AppendLine(Stringify(cameras[0], "left"));
            sb.AppendLine(Stringify(cameras[1], "right"));

            sb.Append(LeftInput);
            sb.Append(RightInput);

            return sb.ToString();
        }

        // Indicate that InitWebXR() has been called
        private static bool isInitialized = false;

        // Shared float array with javascript.
        // [0] -> [15] : projection matrix of view 1
        // [16], [17], [18] : X, Y, Z position in m  of view 1
        // [19], [20], [21], [22] : RX, RY, RZ, RW rotation (quaternion)  of view 1
        // [23] -> [26] : Viewport X, Y, width, height  of view 1
        // [27] -> [42] : projection matrix of view 2
        // [43], [44], [45] : X, Y, Z position in m  of view 2
        // [46], [47], [48], [49] : RX, RY, RZ, RW rotation (quaternion)  of view 2
        // [50] -> [53] : Viewport X, Y, width, height  of view 2
        // [54] -> [60] : Left input x, y, z, rx, ry, rz, rw
        // [61] -> [68] : left input axes
        // [69] -> [76] : left gamepad value
        // [77] -> [83] : right input x, y, z, rx, ry, rz, rw
        // [84] -> [91] : right input axes
        // [92] -> [99] : right gamepad value
        // [100] : user height
        // [101] : left input haptic pulse value
        // [102] : right input haptic pulse value
        // [103] : left input haptic pulse duration
        // [104] : right input haptic pulse duration
        // [105] -> [111] : hit test x, y, z, rx, ry, rz, rw
        private static readonly float[] DataArray = new float[112];

        // Shared float array with javascript.
        // [0] : number of views (0 : session is stopped)
        // [1] : left controller events
        // [2] : right controller events
        // [3] : input change event
        // [4] : left input has position info
        // [5] : left input target ray mode
        // [6] : left input gamepad axes count
        // [7] : left input gamepad buttons count
        // [8] -> [15] : left inputs touched
        // [16] -> [23] : left inputs pressed
        // [24] : right input has position info
        // [25] : right input target ray mode
        // [26] : right input gamepad axes count
        // [27] : right input gamepad buttons count
        // [28] -> [35] : right inputs touched
        // [36] -> [43] : right inputs pressed
        // [44] : Left controller active
        // [45] : Right controller active
        // [46] : Left hand active
        // [47] : Right hand active
        // [48] : Hit test in progress
        // [49] : Hit test supported
        private static readonly byte[] ByteArray = new byte[50];

        // Hand data
        // [0] -> [7] : Left Wrist x, y, z, rx, ry, rz, zw, radius
        // [8] -> [199] : Left other fingers ...
        // [200] -> [399] : right wrist and fingers
        private static readonly float[] HandData = new float[8 * 25 * 2];

        /// <summary>
        /// XR Space in which positions are returned
        /// </summary>
        private static WebXRReferenceSpaces ReferenceSpace { get; set; } = WebXRReferenceSpaces.Viewer;

        /// <summary>
        /// User height in meter (startup distance from floor to device)
        /// </summary>
        public static float UserHeight => DataArray[100];

        /// <summary>
        /// Y axis offset to apply when local floor XR space is not supported
        /// </summary>
        public static float FallbackUserHeight { get; set; } = 1.8f;

        // Number of views (i.e. cameras)
        private static WebXRViewEyes ViewEye => (WebXRViewEyes)ByteArray[0];

        // A session is running
        private static bool InternalInSession => ByteArray[0] != 0;

        // Cameras created for each eyes ([0]:left, [1]:right)
        private static readonly Camera[] cameras = new Camera[2];

        // Indicates that main camera properties should be restored after sessions ends
        private static bool shouldRestoreMainCameraProperties;

        // Remember main camera background color to restore value after sessions ends
        private static Color mainCameraBackgroundColor;

        // Remember main camera clear flags to restore value after sessions ends
        private static CameraClearFlags mainCameraClearFlags;

        private static Matrix4x4 mainCameraProjectionMatrix;

        private static Rect mainCameraRect;

        // Create and update camera pose
        private static void UpdateCamera(WebXRViewEyes eye)
        {
            // TODO Figure out how we wanna handle the camera rig setup in the scene.
            // IF we could use a single camera with stereoscopic settings this would be ideal.
            var id = (eye == WebXRViewEyes.Left) ? 0 : 1;

            // If the camera for this id should not exist
            if ((ViewEye & eye) != eye)
            {
                if (cameras[id])
                {
                    if (eye == WebXRViewEyes.Left) // don't destroy main camera
                    {
                        if (shouldRestoreMainCameraProperties)
                        {
                            Camera.main.backgroundColor = mainCameraBackgroundColor;
                            Camera.main.clearFlags = mainCameraClearFlags;
                            Camera.main.projectionMatrix = mainCameraProjectionMatrix;
                            Camera.main.rect = mainCameraRect;
                        }
                    }
                    else
                    {
                        Object.Destroy(cameras[id].gameObject);
                    }

                    cameras[id] = null;
                }

                return;
            }

            // Create camera
            if (!cameras[id])
            {
                if (id > 0)
                {
                    // clone main camera
                    cameras[id] = Object.Instantiate(Camera.main, Camera.main.gameObject.transform.parent);
                    cameras[id].name = $"WebXRCamera_{id}";
                    cameras[id].depth = Camera.main.depth - 1;
                }
                else
                {
                    shouldRestoreMainCameraProperties = false;

                    if (Camera.main)
                    {
                        cameras[0] = Camera.main;
                        shouldRestoreMainCameraProperties = true;
                        mainCameraBackgroundColor = Camera.main.backgroundColor;
                        mainCameraClearFlags = Camera.main.clearFlags;
                        mainCameraProjectionMatrix = Camera.main.projectionMatrix;
                        mainCameraRect = Camera.main.rect;
                    }
                    else
                    {
                        var go = new GameObject("WebXRCamera_0");
                        cameras[0] = go.AddComponent<Camera>();
                    }
                }

                if (IsArSupported)
                {
                    cameras[id].clearFlags = CameraClearFlags.SolidColor;
                    cameras[id].backgroundColor = new Color(0, 0, 0, 0);
                }
            }

            var floatStartId = id * 27;

            var rect = new Rect(DataArray[floatStartId + 23], DataArray[floatStartId + 24], DataArray[floatStartId + 25], DataArray[floatStartId + 26]);

            if (id > 0)
            {
                if (cameras[0] && cameras[0].rect == rect)
                {
                    cameras[id].gameObject.SetActive(false);
                    return;
                }

                cameras[id].gameObject.SetActive(true);
            }

            cameras[id].rect = rect;

            // Get and transpose projection matrix
            var pm = new Matrix4x4
            {
                m00 = DataArray[floatStartId + 0],
                m01 = DataArray[floatStartId + 4],
                m02 = DataArray[floatStartId + 8],
                m03 = DataArray[floatStartId + 12],
                m10 = DataArray[floatStartId + 1],
                m11 = DataArray[floatStartId + 5],
                m12 = DataArray[floatStartId + 9],
                m13 = DataArray[floatStartId + 13],
                m20 = DataArray[floatStartId + 2],
                m21 = DataArray[floatStartId + 6],
                m22 = DataArray[floatStartId + 10],
                m23 = DataArray[floatStartId + 14],
                m30 = DataArray[floatStartId + 3],
                m31 = DataArray[floatStartId + 7],
                m32 = DataArray[floatStartId + 11],
                m33 = DataArray[floatStartId + 15]
            };

            cameras[id].projectionMatrix = pm;

            // Get position and rotation Z, RX and RY are inverted
            cameras[id].transform.localPosition = ToUnityPosition(DataArray[floatStartId + 16], DataArray[floatStartId + 17], DataArray[floatStartId + 18]);
            cameras[id].transform.localRotation = ToUnityRotation(DataArray[floatStartId + 19], DataArray[floatStartId + 20], DataArray[floatStartId + 21], DataArray[floatStartId + 22]);
        }

        // Update input source pose
        private static void UpdateInput(WebXRInputSource inputSource)
        {
            var floatStartId = (int)inputSource.Handedness * 23 + 54;
            var byteStartId = (int)inputSource.Handedness * 20 + 4;

            inputSource.Position = ToUnityPosition(DataArray[floatStartId + 0], DataArray[floatStartId + 1], DataArray[floatStartId + 2]);
            inputSource.Rotation = ToUnityRotation(DataArray[floatStartId + 3], DataArray[floatStartId + 4], DataArray[floatStartId + 5], DataArray[floatStartId + 6]);
            inputSource.IsPositionTracked = (ByteArray[byteStartId + 0] != 0);

            var eventId = (int)inputSource.Handedness + 1;
            var mask = ByteArray[eventId];

            if (mask != 0)
            {
                inputSource.RaiseInputSourceEvent(mask, WebXRInputSourceEventTypes.Select);
                inputSource.RaiseInputSourceEvent(mask, WebXRInputSourceEventTypes.SelectEnd);
                inputSource.RaiseInputSourceEvent(mask, WebXRInputSourceEventTypes.SelectStart);
                inputSource.RaiseInputSourceEvent(mask, WebXRInputSourceEventTypes.Squeeze);
                inputSource.RaiseInputSourceEvent(mask, WebXRInputSourceEventTypes.SqueezeEnd);
                inputSource.RaiseInputSourceEvent(mask, WebXRInputSourceEventTypes.SqueezeStart);

                ByteArray[eventId] = 0;
            }

            inputSource.TargetRayMode = (WebXRTargetRayModes)ByteArray[byteStartId + 1];
            inputSource.AxesCount = ByteArray[byteStartId + 2];

            for (int i = 0; i < WebXRInputSource.AXES_BUTTON_COUNT; i++)
            {
                inputSource.Axes[i] = DataArray[floatStartId + 7 + i];
            }

            inputSource.ButtonsCount = ByteArray[byteStartId + 3];

            for (int i = 0; i < WebXRInputSource.AXES_BUTTON_COUNT; i++)
            {
                var button = inputSource.Buttons[i];
                button.Value = DataArray[floatStartId + 15 + i];
                button.Touched = ByteArray[byteStartId + 4 + i] != 0;
                button.Pressed = ByteArray[byteStartId + 12 + i] != 0;
            }

            var handAvailable = 0 != ByteArray[46 + (int)inputSource.Handedness];
            inputSource.Hand.Available = handAvailable;

            if (handAvailable)
            {
                for (int j = 0; j < 25; j++)
                {
                    var i = (int)inputSource.Handedness * 200 + j * 8;
                    inputSource.Hand.Joints[j] = new WebXRJoint(
                        ToUnityPosition(HandData[i], HandData[i + 1], HandData[i + 2]),
                        ToUnityRotation(HandData[i + 3], HandData[i + 4], HandData[i + 5], HandData[i + 6]),
                        HandData[i + 7]
                    );
                }
            }
        }

        // Update the hit test infos
        private static void UpdateHitTest()
        {
            if (HitTestInProgress)
            {
                HitTestPosition = ToUnityPosition(DataArray[105], DataArray[106], DataArray[107]);
                HitTestRotation = ToUnityRotation(DataArray[108], DataArray[109], DataArray[110], DataArray[111]);
            }
        }

        // Converts a WebGL position coordinate to a Unity position coordinate
        private static Vector3 ToUnityPosition(float x, float y, float z)
        {
            float yOffset = 0;

            if (ReferenceSpace == WebXRReferenceSpaces.LocalFloor)
            {
                yOffset = UserHeight <= 0 ? FallbackUserHeight : UserHeight;
            }

            return new Vector3(x, y + yOffset, -z);
        }

        // Converts a WebGL rotation to a Unity rotation
        private static Quaternion ToUnityRotation(float x, float y, float z, float w) => new Quaternion(-x, -y, z, w);

        [Flags]
        private enum WebXRViewEyes
        {
            None = 0,
            Left = 1,
            Right = 2,
            Both = Left | Right
        }

        private static string Stringify(Camera camera, string name)
        {
            if (camera)
            {
                var sb = new StringBuilder();
                sb.Append(name);
                sb.AppendLine("eye : ");
                sb.Append("  Position :");
                sb.AppendLine(camera.transform.position.ToString());
                sb.Append("  Rotation :");
                sb.AppendLine(camera.transform.rotation.eulerAngles.ToString());
                return sb.ToString();
            }

            return $"No {name} eye";
        }

        #region Device Orientation

        // Orientation float data (shared array with javascript)
        // [0] : alpha
        // [1] : beta
        // [2] : gamma
        private static readonly float[] OrientationArray = new float[3];

        // Orientation byte data (shared array with javascript)
        // [0] : 1=valid angle values, 0=angles not available yet
        private static readonly byte[] OrientationInfo = new byte[1];

        // Indicates that InternalGetDeviceOrientation was already called
        private static bool orientationDeviceStarted = false;

        /// <summary>
        /// Get the orientation of the device. This feature is independent of WebXR and can be used as a fallback if WebXR is not supported. 
        /// </summary>
        /// <remarks>
        /// Values come from the javascript event "deviceorientation" obtained by : window.addEventListener("deviceorientation", _onDeviceOrientation);
        /// The x axis is in the plane of the screen and is positive toward the right and negative toward the left.
        /// The y axis is in the plane of the screen and is positive toward the top and negative toward the bottom.
        /// The z axis is perpendicular to the screen or keyboard, and is positive extending outward from the screen.
        /// </remarks>
        /// <see href="https://developer.mozilla.org/en-US/docs/Web/Guide/Events/Orientation_and_motion_data_explained#About_rotation"/>
        /// <param name="alpha">Rotation around the z axis -- that is, twisting the device -- causes the alpha rotation angle to change. The alpha angle is 0° when top of the device is pointed directly toward the Earth's north pole, and increases as the device is rotated toward the left</param>
        /// <param name="beta">Rotation around the x axis -- that is, tipping the device away from or toward the user -- causes the beta rotation angle to change. The beta angle is 0° when the device's top and bottom are the same distance from the Earth's surface; it increases toward 180° as the device is tipped forward toward the user, and it decreases toward -180° as the device is tipped backward away from the user.</param>
        /// <param name="gamma">Rotation around the y axis -- that is, tilting the device toward the left or right -- causes the gamma rotation angle to change.The gamma angle is 0° when the device's left and right sides are the same distance from the surface of the Earth, and increases toward 90° as the device is tipped toward the right, and toward -90° as the device is tipped toward the left.</param>
        /// <returns>True if valid angles are returned</returns>
        public static bool TryGetDeviceOrientation(out float alpha, out float beta, out float gamma)
        {
            if (!orientationDeviceStarted)
            {
                InternalGetDeviceOrientation(OrientationArray, OrientationInfo);
                orientationDeviceStarted = true;
            }

            alpha = OrientationArray[0];
            beta = OrientationArray[1];
            gamma = OrientationArray[2];

            return OrientationInfo[0] != 0;
        }

        #endregion Device Orientation

        #region Simulation

        public static float[] SimulatedDataArray => DataArray;

        public static byte[] SimulatedByteArray => ByteArray;

        public static Quaternion SimulatedToUnityRotation(Quaternion q) => ToUnityRotation(q.x, q.y, q.z, q.w);

        #endregion Simulation
    }
}
