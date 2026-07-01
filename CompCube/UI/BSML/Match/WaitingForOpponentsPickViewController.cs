using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace CompCube.UI.BSML.Match;

[ViewDefinition("CompCube.UI.BSML.Match.WaitingForOpponentsPickView.bsml")]
public class WaitingForOpponentsPickViewController : BSMLAutomaticViewController
{
    [UIValue("mainText")] private string mainText { get; set; } = "";

    public void PopulateData(string opponentName)
    {
        mainText = $"Waiting for {opponentName} to select a map...";
        NotifyPropertyChanged(nameof(mainText));
    }
}