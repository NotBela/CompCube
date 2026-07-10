using BeatSaberMarkupLanguage;
using BGLib.UnityExtension;
using HarmonyLib;
using HMUI;
using IPA.Utilities;
using SiraUtil.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace CompCube.UI.BSML.Components.EnergyBar;

public class EnergyBar
{
    private bool _isLoadingImage;
    
    private static GameObject _image;
    
    public static void ParseOntoViewController(ViewController viewController)
    {
        LoadImageIfNotLoadedAlready();
        
        Object.Instantiate(_image, viewController.transform);
    }

    private static void LoadImageIfNotLoadedAlready()
    {
        if (_image != null) 
            return;
        
        _image = Resources.FindObjectsOfTypeAll<GameObject>().First(i => i.name == "BatteryLifeSegment");
    }
}