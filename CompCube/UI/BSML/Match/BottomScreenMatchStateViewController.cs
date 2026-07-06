using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube.Extensions;

namespace CompCube.UI.BSML.Match;

[ViewDefinition("CompCube.UI.BSML.Match.BottomScreenMatchStateView.bsml")]
public class BottomScreenMatchStateViewController : BSMLAutomaticViewController
{
    [UIValue("roundText")] private string RoundText { get; set; }
    [UIValue("pointsText")] private string PointsText { get; set; }
    [UIValue("redText")] private string RedText { get; set; }
    [UIValue("blueText")] private string BlueText { get; set; }
    
    [UIValue("multiplierText")] private string MultiplierText { get; set; }
    
    public void PopulateData(CompCube_Models.Models.ClientData.UserInfo red, CompCube_Models.Models.ClientData.UserInfo blue)
    {
        RedText = red.GetFormattedUserName();
        BlueText = blue.GetFormattedUserName();
        
        NotifyPropertyChanged(null);
    }

    public void UpdatePoints(int redHealth, int blueHealth)
    {
        PointsText = $"{redHealth.ToString().FormatWithHtmlColor("#FF0000")} - {blueHealth.ToString().FormatWithHtmlColor("#0000FF")}";
        NotifyPropertyChanged(nameof(PointsText));
    }

    public void UpdateRound(int round)
    {
        RoundText = $"Round {round}";
        NotifyPropertyChanged(nameof(RoundText));
    }

    public void UpdateMultiplier(float multiplier)
    {
        MultiplierText = $"{multiplier:F1}x";
        NotifyPropertyChanged(nameof(MultiplierText));
    }

    public void SetStatus(string status)
    {
        RoundText = status;
        NotifyPropertyChanged(nameof(RoundText));
    }
}