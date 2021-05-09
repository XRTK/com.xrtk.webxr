namespace XRTK.WebXR.Native
{
    /// <summary>
    /// Describes a button, trigger, thumbstick, or touchpad data
    /// </summary>
    internal class WebXRGamepadButton
    {
        /// <summary>
        /// The amount which the button has been pressed, between 0.0 and 1.0, for buttons that have an analog sensor
        /// </summary>
        public float Value { get; internal set; }

        /// <summary>
        /// The touched state of the button
        /// </summary>
        public bool Touched { get; internal set; }

        /// <summary>
        /// The pressed state of the button
        /// </summary>
        public bool Pressed { get; internal set; }

        /// <summary>
        /// Stringify the button, trigger, thumbstick, or touchpad data
        /// </summary>
        /// <returns>String that describes the button, trigger, thumbstick, or touchpad data</returns>
        public override string ToString()
        {
            return $"{(Touched ? (Pressed ? "Touched and pressed" : "Touched") : (Pressed ? "Pressed" : "Released"))} - {(int)(100 * Value)}%";
        }
    }
}