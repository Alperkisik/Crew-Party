namespace Game.GlobalVariables
{
    /// <summary>
    /// Add custom managers events here.
    /// <example> <code> public const string SomeEvent = nameof(SomeEvent); </code> </example>
    /// </summary>
    public static partial class CustomManagerEvents
    {
        #region FeverBar

        public const string FeverState = nameof(FeverState);
        public const string FeverIncrease = nameof(FeverIncrease);

        #endregion

        #region ProgressBar

        public const string SendFinishlineTransform = nameof(SendFinishlineTransform);
        public const string SendPlayerTransform = nameof(SendPlayerTransform);
        public const string PlayerPositionUpdated = nameof(PlayerPositionUpdated);

        #endregion

        #region Joystick
        public const string SendJoystick = nameof(SendJoystick);
        #endregion

        #region Camera

        public const string SendCameraTarget = nameof(SendCameraTarget);

        #endregion

        #region Path System
        public const string PlayerCanFollowPath = nameof(PlayerCanFollowPath);
        public const string PlayerReachedEndOfPath = nameof(PlayerReachedEndOfPath);
        public const string SendPathCreatorRef = nameof(SendPathCreatorRef);
        public const string UpdatePathedProgressBar = nameof(UpdatePathedProgressBar);
        #endregion

    }
}
