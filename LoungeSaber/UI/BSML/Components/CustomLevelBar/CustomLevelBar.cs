using CompCube_Models.Models.Map;
using LoungeSaber.Extensions;
using UnityEngine;

namespace LoungeSaber.UI.BSML.Components.CustomLevelBar;

public class CustomLevelBar : MonoBehaviour
{
    public LevelBar LevelBar { get; private set; }
    
    public void Init(Transform parent)
    {
        LevelBar = Instantiate(Resources.FindObjectsOfTypeAll<ResultsViewController>().First()._levelBar, parent, false);
        LevelBar._difficultyText.gameObject.SetActive(true);
        LevelBar._characteristicIconImageView.gameObject.SetActive(true);
    }

    public void SetWaiting()
    {
        SetLevelDetailObjectsActive(false);
        LevelBar._songNameText.text = "Waiting...";
    }

    private void SetLevelDetailObjectsActive(bool active)
    {
        LevelBar._difficultyText.gameObject.SetActive(active);
        LevelBar._songArtworkImageView.gameObject.SetActive(active);
        LevelBar._authorNameText.gameObject.SetActive(active);
        LevelBar._characteristicIconImageView.gameObject.SetActive(active);
    }

    public void Setup(VotingMap map)
    {
        SetLevelDetailObjectsActive(true);
        
        // cannot use BeatmapKey overload because it is broken ??
        LevelBar.Setup(map.GetBeatmapLevel(), map.GetBaseGameDifficultyType(), map.GetBeatmapLevel()?.GetCharacteristics().First(i => i.serializedName == "Standard"));
    }
}