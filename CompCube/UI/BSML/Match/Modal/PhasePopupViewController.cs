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
    [UIParams] private readonly BSMLParserParams _parserParams = null!;
    [Inject] private readonly SharedCoroutineStarter _sharedCoroutineStarter = null!;
    
    [UIValue("mainText")] private string MainText { get; set; } = "";
    [UIValue("subText")] private string SubText { get; set; } = "";
    
    public void ParseOntoObject(GameObject go, string mainText, string subText)
    {
        MainText = mainText;
        SubText = subText;
        NotifyPropertyChanged(null);

        BSMLParser.Instance.Parse(
            Utilities.GetResourceContent(Assembly.GetExecutingAssembly(),
                "CompCube.UI.BSML.Match.Modal.PhasePopupView.bsml"), go, this);
        _sharedCoroutineStarter.Run(Show());
    }

    private IEnumerator Show()
    {
        _parserParams.EmitEvent("show-event");

        yield return new WaitForSeconds(2f);
        
        _parserParams.EmitEvent("hide-event");
    }
}