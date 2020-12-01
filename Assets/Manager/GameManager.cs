using System;

public class GameManager : Singleton<GameManager>
{
    private string authCode;

    public void SetAuthCode(string authCode)
    {
        this.authCode = authCode;
    }
}
