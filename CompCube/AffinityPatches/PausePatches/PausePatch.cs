using CompCube.Game;
using SiraUtil.Affinity;
using Zenject;

namespace CompCube.AffinityPatches.PausePatches
{
    public class PausePatch : IAffinity
    {
        [Inject] private readonly MatchStartUnpauseController _matchStartUnpauseController = null;
        
        [AffinityPrefix]
        [AffinityPatch(typeof(PauseController), nameof(PauseController.Pause))]
        private bool Prefix() => _matchStartUnpauseController.StillInStartingPauseMenu;
    }
}