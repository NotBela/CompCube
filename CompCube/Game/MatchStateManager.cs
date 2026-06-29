using CompCube_Models.Models.Map;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube.Interfaces;
using HarmonyLib;
using SiraUtil.Logging;
using Zenject;

namespace CompCube.Game;

public class MatchStateManager : IInitializable, IDisposable
{
    [Inject] private readonly IServerListener _serverListener = null!;
    [Inject] private readonly SiraLog _siraLog = null!;
    [Inject] private readonly UserModelWrapper _userModelWrapper = null!;

    public CompCube_Models.Models.ClientData.UserInfo RedPlayer { get; private set; }
    public CompCube_Models.Models.ClientData.UserInfo BluePlayer { get; private set; }

    public int RedHealth { get; private set; } = 1000000;
    public int BlueHealth { get; private set; } = 1000000;

    public float DamageMultiplier { get; private set; }= 1f;
    
    public int DiscardedMapCount { get; private set; } = 0;

    public bool InDiscardPhase { get; private set; } = true;

    public List<VotingMap> Maps = [];

    public bool IsRedTeam => RedPlayer.UserId == _userModelWrapper.UserId;
    
    public bool CanDiscardMaps => InDiscardPhase && DiscardedMapCount < 2;

    public void DiscardMap(VotingMap map)
    {
        Maps.Remove(map);
        DiscardedMapCount++;
    }
    
    public void Initialize()
    {
        _serverListener.OnMatchCreated += HandleMatchCreated;
        _serverListener.OnRoundResults += HandleRoundResults;
        _serverListener.OnPickPhaseStarted += HandlePickPhaseStarted;
    }

    private void HandlePickPhaseStarted(StartPickPhasePacket packet)
    {
        Maps = packet.AvailableMaps.ToList();
        InDiscardPhase = false;
    }

    private void HandleMatchCreated(MatchCreatedPacket matchCreated)
    {
        RedPlayer = matchCreated.Red;
        BluePlayer = matchCreated.Blue;
        
        RedHealth = 1000000;
        BlueHealth = 1000000;

        DamageMultiplier = 1.0f;

        Maps = matchCreated.InitialMaps.ToList();

        DiscardedMapCount = 0;
        
        InDiscardPhase = true;
    }
    
    private void HandleRoundResults(RoundResultsPacket results)
    {
        RedHealth = results.RedHealth;
        BlueHealth = results.BlueHealth;

        DamageMultiplier = results.DamageMultiplier;
    }

    public void Dispose()
    {
        _serverListener.OnMatchCreated -= HandleMatchCreated;
        _serverListener.OnRoundResults -= HandleRoundResults;
        _serverListener.OnPickPhaseStarted -= HandlePickPhaseStarted;
    }
}