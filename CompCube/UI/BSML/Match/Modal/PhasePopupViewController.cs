using System.Collections;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using Tweening;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Match.Modal;

[ViewDefinition("CompCube.UI.BSML.Match.Modal.PhasePopupView.bsml")]
public class PhasePopupViewController : BSMLAutomaticViewController
{
    [Inject] private readonly BSMLParserParams _parserParams = null!;
    [Inject] private readonly BSMLParser _parser = null!;
    
    [UIValue("mainText")] private string MainText { get; set; } = "";
    [UIValue("subText")] private string SubText { get; set; } = "";
    
    public void ParseOntoObject(GameObject go, string mainText, string subText)
    {
        MainText = mainText;
        SubText = subText;
        NotifyPropertyChanged(null);

        _parser.Parse(
            Utilities.GetResourceContent(Assembly.GetExecutingAssembly(),
                "CompCube.UI.BSML.Match.Modal.PhasePopupView.bsml"), go, this);
        Show();
    }

    private void Show()
    {
        _parserParams.EmitEvent("show-event");
    }
}