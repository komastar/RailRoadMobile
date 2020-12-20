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
        private static string ApiUrl => $"{GameServer}/api/ApiGame";

        public static string GetCreateGameUrl(int maxUserCount) => $"{ApiUrl}/Create/{maxUserCount}";
        public static string GetJoinGameUrl(string gameCode) => $"{ApiUrl}/Join/{gameCode}";
        public static string GetFindGameUrl(string gameCode) => $"{ApiUrl}/Find/{gameCode}";
        public static string GetExitGameUrl(string game, string userId) => $"{ApiUrl}/Exit/{game}/{userId}";
        public static string GetStartGameUrl(string gameCode) => $"{ApiUrl}/Start/{gameCode}";
        public static string GetRoundGameUrl(string gameCode, int round) => $"{ApiUrl}/Round/{gameCode}/{round}";
    }

    public static class GameCode
    {
        public static string SoloPlay => "****";
    }
}
