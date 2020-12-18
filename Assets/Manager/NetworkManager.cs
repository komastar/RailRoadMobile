using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Assets.Foundation.Model;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Assets.Foundation.Constant;

namespace Manager
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        private const int port = 11000;
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static String responseString = String.Empty;

        public static GameRoomModel CreateGame(int maxUserCount)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Create/{maxUserCount}";
            var response = GetRequest(url);

            return GameRoomModel.Parse(response.Data);
        }

        public static GameRoomModel JoinGame(string game)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Join/{game}";
            var response = GetRequest(url);

            return response.ProcessResult ? GameRoomModel.Parse(response.Data) : null;
        }

        public static GameRoomModel GetGame(string game)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Get/{game}";
            var response = GetRequest(url);

            return GameRoomModel.Parse(response.Data);
        }

        public static bool ReadyGame(string game, int time)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Round/{game}/{time}";
            var response = GetRequest(url);

            return response != null ? response.ProcessResult : false;
        }

        public static GameRoomModel ExitGame(string game, string userId)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Exit/{game}/{userId}";
            var response = GetRequest(url);
            if (null == response
                || null == response.Data)
            {
                return null;
            }

            return response.ProcessResult ? GameRoomModel.Parse(response.Data) : null;
        }

        public static GameRoomModel StartGame(string gameCode)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Start/{gameCode}";
            var response = GetRequest(url);
            if (null == response
                || null == response.Data)
            {
                return null;
            }

            return response.ProcessResult ? GameRoomModel.Parse(response.Data) : null;
        }

        public static string CreateUser()
        {
            string url = $"{UrlTable.GameServer}/api/ApiGameUser/Create";
            var response = GetRequest2(url);

            return response;
        }

        public static bool DeleteGame(string gameCode)
        {
            string url = $"{UrlTable.GameServer}/api/ApiGame/Delete/{gameCode}";
            var response = GetRequest2(url);

            return bool.Parse(response);
        }

        public static int ClearUser()
        {
            string url = $"{UrlTable.GameServer}/api/ApiGameUser/Clear";
            var response = GetRequest2(url);

            return int.Parse(response);
        }

        public static ResponseModel GetRequest(string url)
        {
            return ResponseModel.Parse(GetRequest2(url));
        }

        public static string GetRequest2(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var send = request.SendWebRequest();
                while (!send.isDone) { }
                if (null == send.webRequest.downloadHandler
                    || string.IsNullOrEmpty(send.webRequest.downloadHandler.text))
                {
                    Log.Error("GetRequest FAIL. downloadHandler is null / text is null or empty");
                    return null;
                }

                return send.webRequest.downloadHandler.text;
            }
        }

        public async static Task StartWebSocket()
        {
            try
            {
                var client = new ClientWebSocket();
                Log.Info("Open");
                //await client.ConnectAsync(new Uri("ws://localhost:7001/ws"), CancellationToken.None);
                await client.ConnectAsync(new Uri("ws://rpi.komastar.kr/ws"), CancellationToken.None);

                Log.Info("Send");
                CommModel data = new CommModel();
                byte[] bufferSend = data.GetBytes();
                await client.SendAsync(new ArraySegment<byte>(bufferSend), WebSocketMessageType.Text, true, CancellationToken.None);

                Log.Info("Receive");
                byte[] bufferReceive = new byte[256];
                while (client.State == WebSocketState.Open)
                {
                    var r = await client.ReceiveAsync(new ArraySegment<byte>(bufferReceive), CancellationToken.None);
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                Log.Info($"{Encoding.ASCII.GetString(bufferReceive)}");
            }
            catch (Exception e)
            {
                Log.Info(e.Message);
            }
        }

        public static void StartClient()
        {
            try
            {
                responseString = String.Empty;
                connectDone.Reset();
                sendDone.Reset();
                receiveDone.Reset();

                IPHostEntry ipHostInfo = Dns.GetHostEntry("dev.komastar.kr");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                Send(client, "This is a test<EOF>");
                sendDone.WaitOne();

                Receive(client);
                receiveDone.WaitOne();

                Log.Info($"Response received : {responseString}");

                client.Shutdown(SocketShutdown.Both);
                client.Close();
                client.Dispose();

            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Log.Info($"Socket connected to {client.RemoteEndPoint}");
                connectDone.Set();
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject
                {
                    workSocket = client
                };
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.sb.Length > 1)
                    {
                        responseString = state.sb.ToString();
                    }
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                Log.Info($"Sent {bytesSent} bytes to server.");

                sendDone.Set();
            }
            catch (Exception e)
            {
                Log.Info(e.ToString());
            }
        }
    }

    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 256;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
