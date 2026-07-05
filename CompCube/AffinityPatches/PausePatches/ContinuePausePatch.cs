using CompCube.Game;
using SiraUtil.Affinity;
using Zenject;

namespace CompCube.AffinityPatches.PausePatches;

public class ContinuePausePatch : IAffinity
{
    [Inject] private readonly LevelStartUnpauseController _levelStartUnpauseController = null!;

    [AffinityPatch(typeof(PauseController), nameof(PauseController.HandlePauseMenuManagerDidPressContinueButton))]
    [AffinityPrefix]
    private bool Prefix() => !_levelStartUnpauseController.StillInStartingPauseMenu;
}