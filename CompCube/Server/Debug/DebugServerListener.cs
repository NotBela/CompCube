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
    
    public event Action<MatchCreatedPacket>? OnMatchCreated;
    public event Action<BeginGameTransitionPacket>? OnShouldBeginGameTransition;
    public event Action<PlayerSelectedMapPacket>? OnPlayerSelectedMap;
    public event Action<MapSelectionPacket>? OnMapSelection;
    public event Action<RoundResultsPacket>? OnRoundResults;
    public event Action<StartPickPhasePacket>? OnPickPhaseStarted;
    public event Action? OnConnected;
    public event Action? OnDisconnected;


    private bool _isConnected;

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
                break;
            case UserPacket.UserPacketTypes.DiscardMap:
                OnPickPhaseStarted?.Invoke(new StartPickPhasePacket(DebugApi.Maps));
                break;
            case UserPacket.UserPacketTypes.MapSelection:
                
                break;
        }
    }

    public void Disconnect()
    {
        if (!_isConnected) return;

        _isConnected = false;
        OnDisconnected?.Invoke();
    }
}