using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CompCube.UI.BSML.Components.EnergyBar;

public class CustomEnergyBar(GameObject energyBarGameObject, bool depleteFromLeft)
{
    private readonly Image _energyBarImage = energyBarGameObject.GetComponent<Image>();

    public static CustomEnergyBar InstantiateOntoViewController(ViewController viewController, Vector3 position, bool depleteFromLeft, Color color)
    {
        var image = LoadEnergyBarGameObject();
        
        var energyBarBackgroundGameObject = Object.Instantiate(image, viewController.transform);
        energyBarBackgroundGameObject.transform.localScale = new Vector3(4.08f * (depleteFromLeft ? -1 : 1), .04f);
        energyBarBackgroundGameObject.transform.localPosition = position;
        energyBarBackgroundGameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, .6f);
        
        var energyBarGameObject = Object.Instantiate(image, viewController.transform);
        energyBarGameObject.name = "CustomEnergyBar";
        energyBarGameObject.transform.localScale = new Vector3(4f * (depleteFromLeft ? -1 : 1), .02f);
        energyBarGameObject.transform.localPosition = position;

        energyBarGameObject.GetComponent<Image>().color = color;

        var bar = new CustomEnergyBar(energyBarGameObject, depleteFromLeft);
        
        return bar;
    }

    public void SetEnergy(float energy)
    {
        if (!_energyBarImage)
            return;
        
        energyBarGameObject.transform.localScale = new Vector3(4f * (depleteFromLeft ? -1 : 1) * energy, .02f);
    }

    private static GameObject LoadEnergyBarGameObject()
    {
        return Resources.FindObjectsOfTypeAll<GameObject>().First(i => i.name == "BatteryLifeSegment");
    }
}