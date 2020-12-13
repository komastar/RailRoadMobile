namespace Assets.Foundation.Constant
{
    public static class UrlTable
    {
        public static string GameServer => IsRemote ? GameServerRemote : GameServerLocal;
        public static string GameServerRemote => "http://rpi.komastar.kr";
        public static string GameServerLocal => "https://localhost:44377";
        public static bool IsRemote { get; set; } = true;
    }

    public static class GameCode
    {
        public static string SoloPlay => "****";
    }
}
