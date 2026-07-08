using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace CompCube.UI.BSML.Match
{
    [ViewDefinition("CompCube.UI.BSML.Match.WaitingView.bsml")]
    public class WaitingViewController : BSMLAutomaticViewController
    {
        [UIValue("waitingText")] private string WaitingText { get; set; } = "";
        
        public void SetText(string text)
        {
            WaitingText = text;
            NotifyPropertyChanged(nameof(WaitingText));
        }
    }
}