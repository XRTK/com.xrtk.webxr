namespace XRTK.WebXR.Native
{
    /// <summary>
    /// Describes the method used to produce the target ray, and indicates how the application should present the target ray to the user if desired.
    /// </summary>
    internal enum WebXRTargetRayModes
    {
        /// <summary>
        /// No event has yet identified the target ray mode.
        /// </summary>
        None = 0,

        /// <summary>
        /// The target ray originates from either a handheld device or other hand-tracking mechanism and represents that the user is using their hands or the held device for pointing. The orientation of the target ray relative to the tracked object MUST follow platform-specific ergonomics guidelines when available. In the absence of platform-specific guidance, the target ray SHOULD point in the same direction as the user�s index finger if it was outstretched.
        /// </summary>
        TrackedPointer = 1,

        /// <summary>
        /// The input source was an interaction with the canvas element associated with an inline session�s output context, such as a mouse click or touch event.
        /// </summary>
        Screen = 2,

        /// <summary>
        /// The target ray will originate at the viewer and follow the direction it is facing. (This is commonly referred to as a "gaze input" device in the context of head-mounted displays.)
        /// </summary>
        Gaze = 3,
    }
}