using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CompCube.UI.BSML.Components.EnergyBar;

public class CustomEnergyBar(GameObject energyBarGameObject, bool depleteFromLeft)
{
    private readonly GameObject _energyBarGameObject = energyBarGameObject;
    private readonly Image _energyBarImage = energyBarGameObject.GetComponent<Image>();
    
    private readonly bool _depleteFromLeft = depleteFromLeft;

    public static CustomEnergyBar ParseOntoViewController(ViewController viewController, Vector3 position, bool depleteFromLeft, Color color)
    {
        var image = LoadEnergyBarGameObject();
        
        var energyBarGameObject = Object.Instantiate(image, viewController.transform);
        energyBarGameObject.name = "CustomEnergyBar";
        energyBarGameObject.transform.localScale = new Vector3(4f * (depleteFromLeft ? 1 : -1), .02f);
        energyBarGameObject.transform.localPosition = position;

        energyBarGameObject.GetComponent<Image>().color = color;

        var bar = new CustomEnergyBar(energyBarGameObject, depleteFromLeft);
        
        return bar;
    }

    public void SetEnergy(float energy)
    {
        _energyBarImage.rectTransform.anchorMax = new Vector2(energy, 1f);
    }

    private static GameObject LoadEnergyBarGameObject()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().First(i => i.name == "BatteryLifeSegment");
    }
}