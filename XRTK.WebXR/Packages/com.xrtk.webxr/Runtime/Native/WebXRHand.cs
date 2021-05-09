namespace XRTK.WebXR.Native
{
    /// <summary>
    /// Describes the poses of hand skeleton joints
    /// </summary>
    internal class WebXRHand
    {
        #region Constants

        /// <summary>
        /// Number of tracked joints
        /// </summary>
        public const int JOINT_COUNT = 25;

        // INDEX OF EACH JOINT IN ARRAY :

        public const int WRIST = 0;

        public const int THUMB_METACARPAL = 1;
        public const int THUMB_PHALANX_PROXIMAL = 2;
        public const int THUMB_PHALANX_DISTAL = 3;
        public const int THUMB_PHALANX_TIP = 4;

        public const int INDEX_METACARPAL = 5;
        public const int INDEX_PHALANX_PROXIMAL = 6;
        public const int INDEX_PHALANX_INTERMEDIATE = 7;
        public const int INDEX_PHALANX_DISTAL = 8;
        public const int INDEX_PHALANX_TIP = 9;

        public const int MIDDLE_METACARPAL = 10;
        public const int MIDDLE_PHALANX_PROXIMAL = 11;
        public const int MIDDLE_PHALANX_INTERMEDIATE = 12;
        public const int MIDDLE_PHALANX_DISTAL = 13;
        public const int MIDDLE_PHALANX_TIP = 14;

        public const int RING_METACARPAL = 15;
        public const int RING_PHALANX_PROXIMAL = 16;
        public const int RING_PHALANX_INTERMEDIATE = 17;
        public const int RING_PHALANX_DISTAL = 18;
        public const int RING_PHALANX_TIP = 19;

        public const int LITTLE_METACARPAL = 20;
        public const int LITTLE_PHALANX_PROXIMAL = 21;
        public const int LITTLE_PHALANX_INTERMEDIATE = 22;
        public const int LITTLE_PHALANX_DISTAL = 23;
        public const int LITTLE_PHALANX_TIP = 24;

        #endregion Constants

        public WebXRHand()
        {
            for (int i = 0; i < JOINT_COUNT; i++) Joints[i] = new WebXRJoint();
        }

        /// <summary>
        /// Indicates if hand tracking is available
        /// </summary>
        public bool Available;

        /// <summary>
        /// Poses of hand skeleton joints
        /// </summary>
        public readonly WebXRJoint[] Joints = new WebXRJoint[JOINT_COUNT];
    }
}