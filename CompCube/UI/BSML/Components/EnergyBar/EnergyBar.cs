using BeatSaberMarkupLanguage;
using HarmonyLib;
using HMUI;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace CompCube.UI.BSML.Components.EnergyBar;

public class EnergyBar
{
    private static Image _energyBarImage;
    
    public static void ParseOntoViewController(ViewController viewController)
    {
        if (!_energyBarImage)
        {
            Plugin.Log.Info("energy bar is bad");
            return;
        }
        
        Object.Instantiate(_energyBarImage, viewController.transform);
    }
    
    public static void SetEnergyBarImage(Image image) => _energyBarImage = image;
}