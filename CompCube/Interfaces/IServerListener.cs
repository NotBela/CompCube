using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.ServerPackets.Event;

namespace CompCube.Interfaces;

public interface IServerListener
{
    public event Action<MatchCreatedPacket> OnMatchCreated;
    public event Action<OpponentVotedPacket> OnOpponentVoted;
    public event Action<MatchStartedPacket> OnMatchStarting;
    public event Action<MatchResultsPacket> OnMatchResults;
    
    public event Action OnDisconnected;
    public event Action OnConnected;
    public event Action<PrematureMatchEndPacket> OnPrematureMatchEnd;
    
    public event Action<EventStartedPacket> OnEventStarted;
    public event Action<EventMapSelected> OnEventMapSelected;
    public event Action<EventMatchStartedPacket> OnEventMatchStarted;

    public event Action<EventClosedPacket> OnEventClosed;

    public Task Connect(string queue, Action<JoinResponsePacket> onConnectedCallback);

    public Task SendPacket(UserPacket packet);

    public void Disconnect();
}