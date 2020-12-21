namespace Assets.Foundation.Constant
{
    public static class UrlTable
    {
        public static bool IsRemote { get; set; } = true;
#if UNITY_EDITOR
        public static string GameServer => IsRemote ? GameServerRemote : GameServerLocal;
#else
        public static string GameServer => GameServerRemote;
#endif
        public static string GameServerRemote => "http://rpi.komastar.kr";
        public static string GameServerLocal => "https://localhost:44377";
        private static string ApiGameUrl => $"{GameServer}/api/ApiGame";
        private static string ApiContentUrl => $"{GameServer}/api/ApiContentLock";

        public static string GetCreateGameUrl(int maxUserCount) => $"{ApiGameUrl}/Create/{maxUserCount}";
        public static string GetJoinGameUrl(string gameCode) => $"{ApiGameUrl}/Join/{gameCode}";
        public static string GetFindGameUrl(string gameCode) => $"{ApiGameUrl}/Find/{gameCode}";
        public static string GetExitGameUrl(string game, string userId) => $"{ApiGameUrl}/Exit/{game}/{userId}";
        public static string GetStartGameUrl(string gameCode) => $"{ApiGameUrl}/Start/{gameCode}";
        public static string GetRoundGameUrl(string gameCode, int round) => $"{ApiGameUrl}/Round/{gameCode}/{round}";
        public static string GetContentLockCheckUrl(string appName, string version, string contentName) => $"{ApiContentUrl}/Check/{appName}/{version}/{contentName}";
    }

    public static class GameCode
    {
        public static string SoloPlay => "****";
    }
}
