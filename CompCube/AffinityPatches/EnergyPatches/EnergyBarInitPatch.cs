using SiraUtil.Affinity;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CompCube.AffinityPatches.EnergyPatches
{
    public class EnergyBarInitPatch : IAffinity
    {
        [Inject] private readonly SiraLog _siraLog = null!;
        
        [AffinityPatch(typeof(GameEnergyUIPanel), nameof(GameEnergyUIPanel.Init))]
        [AffinityPostfix]
        private void Postfix(GameEnergyUIPanel __instance)
        {
            _siraLog.Info(__instance._batteryLifeSegmentPrefab.name);
            __instance.gameObject.SetActive(false);
        }
    }
}