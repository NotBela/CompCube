using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using LoungeSaber.Extensions;

namespace LoungeSaber.UI.BSML.Match;

[ViewDefinition("LoungeSaber.UI.BSML.Match.OpponentView.bsml")]
public class OpponentViewController : BSMLAutomaticViewController
{
    [UIValue("opponentText")] private string OpponentText { get; set; }
    
    public void PopulateData(CompCube_Models.Models.ClientData.UserInfo opponent)
    {
        OpponentText = $"{opponent.GetFormattedUserName()} - {opponent.Mmr} MMR";
        NotifyPropertyChanged(nameof(OpponentText));
    }
}