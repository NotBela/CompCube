using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;

namespace CompCube.Interfaces;

public interface IServerListener
{
    public event Action<MatchCreatedPacket> OnMatchCreated;
    
    public event Action<PlayerSelectedMapPacket> OnPlayerSelectedMap;
    
    public event Action<RoundResultsPacket> OnRoundResults;
    
    public event Action<StartPickPhasePacket> OnPickPhaseStarted;

    public event Action<MatchFinishedPacket> OnMatchFinished;

    public event Action OnConnected;
    
    public event Action OnDisconnected;
    
    public bool Connected { get; }

    public Task Connect(string queue, Action<JoinResponsePacket> onConnectedCallback);

    public Task SendPacket(UserPacket packet);

    public void Disconnect();
}