// edgegap relay transport.
// reuses KcpTransport with custom KcpServer/Client.

//#if MIRROR <- commented out because MIRROR isn't defined on first import yet
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Mirror;
using kcp2k;

namespace Edgegap
{
    [DisallowMultipleComponent]
    public class EdgegapTransport : KcpTransport
    {
        private EdgegapTransport() { }
        public static EdgegapTransport GetInstance()
        {
            if (instance == null)
            {
                instance = new EdgegapTransport();
            }
            return instance;
        }
        [Header("Relay")]
        public static string relayAddress = "127.0.0.1";
        public static ushort relayGameServerPort = 8888;
        public static ushort relayGameClientPort = 9999;

        // mtu for kcp transport. respects relay overhead.
        public const int MaxPayload = Kcp.MTU_DEF - Protocol.Overhead;

        [Header("Relay")]
        public bool relayGUI = true;
        public static uint userId = 11111111;
        public static uint sessionId = 22222222;
        public static string session_ID;
        private static EdgegapTransport instance;
        

        // helper
        internal static String ReParse(String cmd, String pattern, String defaultValue)
        {
            Match match = Regex.Match(cmd, pattern);
            return match.Success ? match.Groups[1].Value : defaultValue;
        }
        public void ChangeValue(string _relayAdress, ushort _relayGameClientPort, ushort _relayGameServerPort, string _session_ID, uint _sessionId, uint _userId)
        {
            relayAddress = _relayAdress;
            relayGameServerPort = _relayGameServerPort;
            relayGameClientPort = _relayGameClientPort;
            userId = _userId;
            sessionId = _sessionId;
            session_ID = _session_ID;
            return ;
        }
        protected override void Awake()
        {
            // logging
            //   Log.Info should use Debug.Log if enabled, or nothing otherwise
            //   (don't want to spam the console on headless servers)
            if (debugLog)
                Log.Info = Debug.Log;
            else
                Log.Info = _ => {};
            Log.Warning = Debug.LogWarning;
            Log.Error = Debug.LogError;

            // create config from serialized settings.
            // with MaxPayload as max size to respect relay overhead.
            config = new KcpConfig(DualMode, RecvBufferSize, SendBufferSize, MaxPayload, NoDelay, Interval, FastResend, false, SendWindowSize, ReceiveWindowSize, Timeout, MaxRetransmit);

            // client (NonAlloc version is not necessary anymore)
            client = new EdgegapClient(
                () => OnClientConnected.Invoke(),
                (message, channel) => OnClientDataReceived.Invoke(message, FromKcpChannel(channel)),
                () => OnClientDisconnected.Invoke(),
                (error, reason) => OnClientError.Invoke(ToTransportError(error), reason),
                config
            );

            // server
            server = new EdgegapServer(
                (connectionId) => OnServerConnected.Invoke(connectionId),
                (connectionId, message, channel) => OnServerDataReceived.Invoke(connectionId, message, FromKcpChannel(channel)),
                (connectionId) => OnServerDisconnected.Invoke(connectionId),
                (connectionId, error, reason) => OnServerError.Invoke(connectionId, ToTransportError(error), reason),
                config);
            if (statisticsLog)
                InvokeRepeating(nameof(OnLogStatistics), 1, 1);

            Debug.Log("EdgegapTransport initialized!");
        }

        protected override void OnValidate()
        {
            // show max message sizes in inspector for convenience.
            // 'config' isn't available in edit mode yet, so use MTU define.
            ReliableMaxMessageSize = KcpPeer.ReliableMaxMessageSize(MaxPayload, ReceiveWindowSize);
            UnreliableMaxMessageSize = KcpPeer.UnreliableMaxMessageSize(MaxPayload);
        }

        // client overwrites to use EdgegapClient instead of KcpClient
        public override void ClientConnect(string address)
        {
            // connect to relay address:port instead of the expected server address
            EdgegapClient client = (EdgegapClient)this.client;
            client.userId = userId;
            client.sessionId = sessionId;
            client.state = ConnectionState.Checking; // reset from last time
            client.Connect(relayAddress, relayGameClientPort);
        }
        
        public override void ClientConnect(Uri uri)
        {
            if (uri.Scheme != Scheme)
                throw new ArgumentException($"Invalid url {uri}, use {Scheme}://host:port instead", nameof(uri));

            // connect to relay address:port instead of the expected server address
            EdgegapClient client = (EdgegapClient)this.client;
            client.Connect(relayAddress, relayGameClientPort, userId, sessionId);
        }

        // server overwrites to use EdgegapServer instead of KcpServer
        public override void ServerStart()
        {
            // start the server
            EdgegapServer server = (EdgegapServer)this.server;
            server.Start(relayAddress, relayGameServerPort, userId, sessionId);
        }

        void OnGUIRelay()
        {
            // if (server.IsActive()) return;

            GUILayout.BeginArea(new Rect(300, 30, 200, 100));

            GUILayout.BeginHorizontal();
            GUILayout.Label("SessionId:");
            sessionId = Convert.ToUInt32(GUILayout.TextField(sessionId.ToString()));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("UserId:");
            userId = Convert.ToUInt32(GUILayout.TextField(userId.ToString()));
            GUILayout.EndHorizontal();

            if (NetworkServer.active)
            {
                EdgegapServer server = (EdgegapServer)this.server;
                GUILayout.BeginHorizontal();
                GUILayout.Label("State:");
                GUILayout.Label(server.state.ToString());
                GUILayout.EndHorizontal();
            }
            else if (NetworkClient.active)
            {
                EdgegapClient client = (EdgegapClient)this.client;
                GUILayout.BeginHorizontal();
                GUILayout.Label("State:");
                GUILayout.Label(client.state.ToString());
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
        }

        // base OnGUI only shows in editor & development builds.
        // here we always show it because we need the sessionid & userid buttons.
        new void OnGUI()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            base.OnGUI();
#endif
            if (relayGUI) OnGUIRelay();
        }

        public override string ToString() => "Edgegap Transport";
    }
}
//#endif MIRROR <- commented out because MIRROR isn't defined on first import yet
