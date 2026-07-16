using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube.Configuration;
using CompCube.Game;
using CompCube.Game.MatchState;
using CompCube.Interfaces;
using SiraUtil.Logging;
using Zenject;

namespace CompCube.Server
{
    public class ServerListener : IServerListener, IDisposable
    {
        [Inject] private readonly PluginConfig _config = null!;
        [Inject] private readonly SiraLog _siraLog = null!;

        private ClientWebSocket _client = new();
        
        public event Action<MatchCreatedPacket>? OnMatchCreated;
        public event Action<PlayerSelectedMapPacket>? OnPlayerSelectedMap;
        public event Action<RoundResultsPacket>? OnRoundResults;
        public event Action<StartPickPhasePacket>? OnPickPhaseStarted;
        public event Action<MatchFinishedPacket>? OnMatchFinished;
        
        public event Action<UpdateCardsPacket>? OnCardsUpdated; 
        public event Action? OnConnected;
        public event Action? OnDisconnected;
        public event Action<string>? OnAbruptDisconnect;

        private bool _shouldListenToServer;


        [Inject] private readonly UserModelWrapper _userModelWrapper = null!;

        public bool Connected => _client.State == WebSocketState.Open;
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        public async Task Connect(string queue, Action<JoinResponsePacket>? onConnectedCallback)
        {
            if (Connected)
            {
                _siraLog.Error("Tried to connect to server while already connected!");
                return;
            }

            try
            {
                _client = new ClientWebSocket();
                await _client.ConnectAsync(new Uri($"{_config.WebsocketIp}", UriKind.Absolute), _cancellationTokenSource.Token);

                await SendPacket(new JoinRequestPacket(_userModelWrapper.UserName, _userModelWrapper.UserId, queue));

                var bytes = new byte[1024];
                var result = await _client.ReceiveAsync(new ArraySegment<byte>(bytes), _cancellationTokenSource.Token);
                Array.Resize(ref bytes, result.Count);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Normal Closure", _cancellationTokenSource.Token);
                    HandleAbruptDisconnection("Disconnected");
                    return;
                }
                
                var json = Encoding.UTF8.GetString(bytes);

                if (ServerPacket.Deserialize(json) is not JoinResponsePacket joinResponsePacket)
                {
                    HandleAbruptDisconnection("Failed to get server response!");
                    return;
                }
                
                onConnectedCallback?.Invoke(joinResponsePacket);

                if (!joinResponsePacket.Successful)
                {
                    HandleAbruptDisconnection(joinResponsePacket.Message);
                    return;
                }
                
                _shouldListenToServer = true;
                
                while (_shouldListenToServer)
                    await ListenToServer();
            }
            catch (OperationCanceledException)
            {
                // do nothing
            }
        }

        private async Task ListenToServer()
        {
            try
            {
                var data = new byte[4096];

                var result = await _client.ReceiveAsync(new ArraySegment<byte>(data), _cancellationTokenSource.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    HandleAbruptDisconnection("Disconnected");
                    return;
                }

                var json = Encoding.UTF8.GetString(data);

                if (json == "")
                    return;

                var packet = ServerPacket.Deserialize(json);

                switch (packet.PacketType)
                {
                    case ServerPacket.ServerPacketTypes.MatchCreated:
                        OnMatchCreated?.Invoke(packet as MatchCreatedPacket);
                        break;
                    case ServerPacket.ServerPacketTypes.PlayerSelectedMap:
                        OnPlayerSelectedMap?.Invoke(packet as PlayerSelectedMapPacket);
                        break;
                    case ServerPacket.ServerPacketTypes.RoundResults:
                        OnRoundResults?.Invoke(packet as RoundResultsPacket);
                        break;
                    case ServerPacket.ServerPacketTypes.StartPickPhase:
                        OnPickPhaseStarted?.Invoke(packet as StartPickPhasePacket);
                        break;
                    case ServerPacket.ServerPacketTypes.MatchFinished:
                        OnMatchFinished?.Invoke(packet as MatchFinishedPacket);
                        break;
                    case ServerPacket.ServerPacketTypes.UpdateCards:
                        OnCardsUpdated?.Invoke(packet as UpdateCardsPacket);
                        break;
                    case ServerPacket.ServerPacketTypes.AbruptDisconnection:
                        var disconnectPacket = packet as AbruptDisconnectionPacket;

                        HandleAbruptDisconnection(disconnectPacket!.Reason);
                        break;
                    default:
                        throw new Exception("Could not get packet type!");
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                _siraLog.Error(e);
                HandleAbruptDisconnection("Unhandled exception, please check your logs!");
            }
        }

        public async Task SendPacket(UserPacket packet)
        {
            try
            {
                var buffer = packet.SerializeToBytes();
                await _client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        private void StopListeningToServer()
        {
            _shouldListenToServer = false;
            _client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", _cancellationTokenSource.Token);
        }

        public void Disconnect()
        {
            StopListeningToServer();
            OnDisconnected?.Invoke();
        }

        public void HandleAbruptDisconnection(string reason)
        {
            StopListeningToServer();
            OnAbruptDisconnect?.Invoke(reason);
        }

        public void Dispose() => Disconnect();
    }
}