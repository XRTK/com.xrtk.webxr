// Copyright (c) XRTK. All rights reserved.
// Copyright (c) 2020 Florent GIRAUD (Rufus31415) https://github.com/Rufus31415/Simple-WebXR-Unity
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEngine;

namespace XRTK.WebXR.Native
{
    /// <summary>
    /// Contains WebXR input source controller state and events
    /// </summary>
    public class WebXRInputSource
    {
        #region Events

        /// <summary>
        /// Event triggered when the browser triggers a XRSession.selectend event, which means the input source has fully completed its primary action.
        /// </summary>
        /// <remarks>
        /// On Oculus Quest : Back trigger button was pressed
        /// On HoloLens 2 : A air tap has been was performed
        /// On smartphones : The screen was touched
        /// </remarks>
        public event Action Select;

        /// <summary>
        /// Event triggered when the browser triggers a XRSession.selectstart event, which means the input source begins its primary action.
        /// </summary>
        public event Action SelectStart;

        /// <summary>
        /// Event triggered when the browser triggers a XRSession.selectend event, which means the input source ends its primary action.
        /// </summary>
        public event Action SelectEnd;

        /// <summary>
        /// Event triggered when the browser triggers a XRSession.selectend event, which means the input source has fully completed its primary squeeze action.
        /// </summary>
        /// <remarks>
        /// On Oculus Quest : Side grip button was pressed
        /// </remarks>
        public event Action Squeeze;

        /// <summary>
        /// Event triggered when the browser triggers a XRSession.selectstart event, which means the input source begins its primary squeeze action.
        /// </summary>
        public event Action SqueezeStart;

        /// <summary>
        /// Event triggered when the browser triggers a XRSession.selectend event, which means the input source ends its primary squeeze action.
        /// </summary>
        public event Action SqueezeEnd;

        #endregion Events

        public const int AXES_BUTTON_COUNT = 8;

        #region State

        /// <summary>
        /// Indicates if the input source exists
        /// </summary>
        public bool Available { get; internal set; }

        /// <summary>
        /// Handedness of the input source
        /// </summary>
        internal readonly WebXRHandedness Handedness;

        /// <summary>
        /// Indicates that the input source is detected and its position is tracked
        /// </summary>
        public bool IsPositionTracked { get; internal set; } = false;

        /// <summary>
        /// Current position of the input source if the position is tracked
        /// </summary>
        public Vector3 Position { get; internal set; }

        /// <summary>
        /// Current rotation of the input source if the position is tracked
        /// </summary>
        public Quaternion Rotation { get; internal set; }

        /// <summary>
        /// Number of axes available for this input source
        /// </summary>
        public int AxesCount { get; internal set; }

        /// <summary>
        /// Current value of each axes
        /// </summary>
        public readonly float[] Axes = new float[AXES_BUTTON_COUNT];

        /// <summary>
        /// Number of button for this input source
        /// </summary>
        public int ButtonsCount { get; internal set; }

        /// <summary>
        /// Current state of each buttons
        /// </summary>
        public readonly WebXRGamepadButton[] Buttons = new WebXRGamepadButton[AXES_BUTTON_COUNT];

        /// <summary>
        /// Describes the method used to produce the target ray, and indicates how the application should present the target ray to the user if desired.
        /// </summary>
        internal WebXRTargetRayModes TargetRayMode = WebXRTargetRayModes.None;

        /// <summary>
        /// The input source primary action is active
        /// </summary>
        /// <remarks>
        /// On Oculus Quest : Back trigger button is pressed
        /// On HoloLens 2 : A air tap is performed
        /// On smartphones : The screen is touched
        /// </remarks>
        public bool Selected { get; private set; }

        /// <summary>
        /// The input source primary squeeze action is active
        /// </summary>
        /// <remarks>
        /// On Oculus Quest : Side grip button is pressed
        /// </remarks>
        public bool Squeezed { get; private set; }

        /// <summary>
        /// Contains hand joints poses, if hand tracking is enabled
        /// </summary>
        public readonly WebXRHand Hand = new WebXRHand();

        #endregion State

        /// <summary>
        /// Constructor that initialize the input source
        /// </summary>
        /// <param name="handedness">Handedness of the input source</param>
        internal WebXRInputSource(WebXRHandedness handedness)
        {
            Handedness = handedness;

            for (int i = 0; i < AXES_BUTTON_COUNT; i++)
            {
                Buttons[i] = new WebXRGamepadButton();
            }
        }

        // Raise input sources select and squeeze events
        internal void RaiseInputSourceEvent(byte mask, WebXRInputSourceEventTypes type)
        {
            if (((WebXRInputSourceEventTypes)mask & type) == type)
            {
                switch (type)
                {
                    case WebXRInputSourceEventTypes.SelectStart:
                        Selected = true;
                        SelectStart?.Invoke();
                        break;
                    case WebXRInputSourceEventTypes.SelectEnd:
                        Selected = false;
                        SelectEnd?.Invoke();
                        break;
                    case WebXRInputSourceEventTypes.SqueezeStart:
                        Squeezed = true;
                        SqueezeStart?.Invoke();
                        break;
                    case WebXRInputSourceEventTypes.SqueezeEnd:
                        Squeezed = false;
                        SqueezeEnd?.Invoke();
                        break;
                    case WebXRInputSourceEventTypes.Squeeze:
                        Squeeze?.Invoke();
                        break;
                    case WebXRInputSourceEventTypes.Select:
                        Select?.Invoke();
                        break;
                }
            }
        }

        /// <summary>
        /// Applies haptic pulse feedback
        /// </summary>
        /// <param name="intensity">Feedback strength between 0 and 1</param>
        /// <param name="duration">Feedback duration in milliseconds</param>
        public void HapticPulse(float intensity, float duration)
        {
            WebXRNativeBindings.HapticPulse(Handedness, intensity, duration);
        }

        /// <summary>
        /// Return a string that represent current input source state
        /// </summary>
        /// <returns>String that represent current input source state</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Handedness);
            sb.AppendLine("controller");

            sb.Append("  Available : ");
            sb.AppendLine(Available ? "Yes" : "No");

            sb.Append("  Mode : ");
            sb.AppendLine(TargetRayMode.ToString());

            if (IsPositionTracked)
            {
                sb.Append("  Position : ");
                sb.AppendLine(Position.ToString());

                sb.Append("  Rotation : ");
                sb.AppendLine(Rotation.eulerAngles.ToString());
            }
            else sb.AppendLine("  No position info");

            sb.Append("  Hand : ");
            sb.AppendLine(Hand.Available ? "Yes" : "No");

            sb.AppendLine("  Axes :");
            if (AxesCount > 0)
            {
                for (int i = 0; i < Math.Min(AxesCount, Axes.Length); i++)
                {
                    sb.Append("    [");
                    sb.Append(i);
                    sb.Append("] : ");
                    sb.Append((int)(100 * Axes[i]));
                    sb.AppendLine("%");
                }
            }
            else sb.AppendLine("    None");

            sb.AppendLine("  Buttons :");
            if (ButtonsCount > 0)
            {
                for (int i = 0; i < Math.Min(ButtonsCount, Buttons.Length); i++)
                {
                    sb.Append("    [");
                    sb.Append(i);
                    sb.Append("] : ");
                    sb.AppendLine(Buttons[i].ToString());
                }
            }
            else sb.AppendLine("    None");

            return sb.ToString();
        }
    }
}
