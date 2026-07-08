using CompCube_Models.Models.Match;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube.Interfaces;
using SiraUtil.Logging;
using Zenject;

namespace CompCube.Server.Debug;

public class DebugServerListener : IServerListener
{
    [Inject] private readonly SiraLog _siraLog = null!;

    private bool _isConnected;

    public event Action<MatchCreatedPacket>? OnMatchCreated;
    public event Action<PlayerSelectedMapPacket>? OnPlayerSelectedMap;
    public event Action<RoundResultsPacket>? OnRoundResults;
    public event Action<StartPickPhasePacket>? OnPickPhaseStarted;
    public event Action<MatchFinishedPacket>? OnMatchFinished;
    
    public event Action? OnConnected;
    public event Action? OnDisconnected;
    public event Action<string>? OnAbruptDisconnect;
    public bool Connected => _isConnected;

    public async Task Connect(string queue, Action<JoinResponsePacket> onConnectedCallback)
    {
        await Task.Delay(1000);

        _isConnected = true;
        
        onConnectedCallback?.Invoke(new JoinResponsePacket(true, ""));
        OnConnected?.Invoke();
        _siraLog.Info("connected");

        await Task.Delay(1000);
        await SendPacket(new JoinRequestPacket(DebugApi.Self.Username, DebugApi.Self.UserId, queue));
    }

    public async Task SendPacket(UserPacket packet)
    {
        _siraLog.Info($"sent packet {packet.PacketType.ToString()}");
        
        if (!_isConnected)
        {
            _siraLog.Info("tried to send packet when not connected!");
            return;
        }

        switch (packet.PacketType)
        {
            case UserPacket.UserPacketTypes.JoinRequest:
                OnMatchCreated?.Invoke(new MatchCreatedPacket(DebugApi.Self, DebugApi.DebugOpponent, DebugApi.Maps));

                // await Task.Delay(2000);
                //OnAbruptDisconnect?.Invoke("test");
                break;
            case UserPacket.UserPacketTypes.DiscardMaps:
                OnPickPhaseStarted?.Invoke(new StartPickPhasePacket(DebugApi.Maps, true, 10f));
                break;
            case UserPacket.UserPacketTypes.MapSelection:
                
                break;
            case UserPacket.UserPacketTypes.ScoreSubmission:
                OnRoundResults?.Invoke(new RoundResultsPacket(Score.Empty, Score.Empty, 1000, 10));

                await Task.Delay(500);
                
                OnMatchFinished?.Invoke(new MatchFinishedPacket(100, false));
                break;
        }
    }

    public void DisconnectAbruptly(string reason)
    {
        if (!_isConnected) return;
        _isConnected = false;
        
        OnAbruptDisconnect?.Invoke(reason);
    }

    public void Disconnect()
    {
        if (!_isConnected) return;

        _isConnected = false;
        OnDisconnected?.Invoke();
    }
}