using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube.Extensions;

namespace CompCube.UI.BSML.Match;

[ViewDefinition("CompCube.UI.BSML.Match.OpponentView.bsml")]
public class OpponentViewController : BSMLAutomaticViewController
{
    [UIValue("opponentText")] private string OpponentText { get; set; }
    
    public void PopulateData(CompCube_Models.Models.ClientData.UserInfo opponent)
    {
        OpponentText = $"{opponent.GetFormattedUserName()} - {opponent.Mmr} MMR";
        NotifyPropertyChanged(nameof(OpponentText));
    }
}