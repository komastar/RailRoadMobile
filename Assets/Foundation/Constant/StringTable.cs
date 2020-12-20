namespace Assets.Foundation.Constant
{
    public static class UrlTable
    {
#if UNITY_EDITOR
        public static string GameServer => IsRemote ? GameServerRemote : GameServerLocal;
#else
        public static string GameServer => GameServerRemote;
#endif
        public static string GameServerRemote => "http://rpi.komastar.kr";
        public static string GameServerLocal => "https://localhost:44377";
        public static bool IsRemote { get; set; } = true;
    }

    public static class GameCode
    {
        public static string SoloPlay => "****";
    }
}
