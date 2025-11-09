using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace CompCube.UI.BSML.Events;

[ViewDefinition("CompCube.UI.BSML.Events.EventWaitingOnNextMatchView.bsml")]
public class EventWaitingOnNextMatchViewController : BSMLAutomaticViewController
{
    [UIValue("centerText")] private string CenterText { get; set; } = "Waiting on event host...";

    public void SetText(string text)
    {
        CenterText = text;
        NotifyPropertyChanged(nameof(CenterText));
    }
}