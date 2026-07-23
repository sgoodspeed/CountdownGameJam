namespace Countdown
{
    /// <summary>
    /// Centralized scene name constants so scene loading never relies on magic strings.
    /// These must match the .unity file names in Assets/Scenes and the entries in Build Settings.
    /// </summary>
    public static class SceneNames
    {
        public const string Initial = "InitialScene";
        public const string Loading = "LoadingScene";
        public const string Game = "GameScene";
    }
}
