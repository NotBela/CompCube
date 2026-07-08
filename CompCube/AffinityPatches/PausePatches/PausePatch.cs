using System.Reflection;
using CompCube.Game;
using SiraUtil.Affinity;
using Zenject;

namespace CompCube.AffinityPatches.PausePatches
{
    public class PausePatch : IAffinity
    {
        [Inject] private readonly LevelStartUnpauseController _levelStartUnpauseController = null!;

        [AffinityPrefix]
        [AffinityPatch(typeof(PauseController), nameof(PauseController.Pause))]
        private bool Prefix(PauseController __instance)
        {
            if (_levelStartUnpauseController.StillInStartingPauseMenu)
                return true;
            
            __instance._pauseMenuManager.ShowMenu();
            return false;
        }
        
        [AffinityPostfix]
        [AffinityPatch(typeof(PauseController), nameof(PauseController.Pause))]
        private void PostFix(PauseController __instance)
        {
            if (_levelStartUnpauseController.StillInStartingPauseMenu)
                return;
            
            __instance._pauseMenuManager._continueButton.gameObject.SetActive(true);
            __instance._pauseMenuManager._backButton.gameObject.SetActive(true);
            __instance._pauseMenuManager._restartButton.gameObject.SetActive(false);
        }
    }
}