using CompCube_Models.Models.Map;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube.Interfaces;
using Zenject;

namespace CompCube.Game;

public class MatchBeatmapManager() : IInitializable, IDisposable
{
    [Inject] private readonly IServerListener _serverListener = null!;
    
    public List<VotingMap?> DiscardedMaps { get; private set; } = [];

    public bool InDiscardPhase { get; private set; } = true;

    public List<VotingMap> Maps = [];
    
    public bool CanDiscardMaps => InDiscardPhase && DiscardedMaps.Count < 2;

    public event Action? CanNoLongerDiscardMaps;

    public void DiscardMap(VotingMap? map)
    {
        if (map != null)
            Maps.Remove(map);
        
        DiscardedMaps.Add(map);
        
        if (!CanDiscardMaps)
            CanNoLongerDiscardMaps?.Invoke();
    }

    public void SkipDiscardingMaps()
    {
        while (DiscardedMaps.Count < 2)
            DiscardMap(null);
    }

    public void Initialize()
    {
        _serverListener.OnMatchCreated += HandleMatchCreated;
        _serverListener.OnPickPhaseStarted += HandlePickPhaseStarted;
    }

    private void HandlePickPhaseStarted(StartPickPhasePacket packet)
    {
        Maps = packet.AvailableMaps.ToList();
        InDiscardPhase = false;
    }
    
    private void HandleMatchCreated(MatchCreatedPacket packet)
    {
        Maps = [];
        DiscardedMaps = [];
        
        InDiscardPhase = true;
    }

    public void Dispose()
    {
        _serverListener.OnMatchCreated -= HandleMatchCreated;
        _serverListener.OnPickPhaseStarted -= HandlePickPhaseStarted;
    }
}