using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube.Configuration;
using JetBrains.Annotations;
using Zenject;

namespace CompCube.UI.BSML.Menu;

[ViewDefinition("CompCube.UI.BSML.Menu.MissingMapsView.bsml")]
public class MissingMapsViewController : BSMLAutomaticViewController
{
    [Inject] private readonly PluginConfig _config = null;
    
    [CanBeNull] private Action<bool> _userChoseToDownloadMapsCallback;
    
    [UIValue("missingMapText")] private string MissingMapText { get; set; } = "placeholder";

    public void SetMissingMapCount(int missingMapCount, Action<bool> userChoseToDownloadMapsCallback)
    {
        _userChoseToDownloadMapsCallback = userChoseToDownloadMapsCallback;
        
        MissingMapText = $"It looks like you are missing {missingMapCount} map(s).";
        NotifyPropertyChanged(nameof(MissingMapText));
    }

    [UIAction("yesButtonOnClick")]
    private void YesButtonOnClick()
    {
        _userChoseToDownloadMapsCallback?.Invoke(true);
        _userChoseToDownloadMapsCallback = null;
    }

    [UIAction("noButtonOnClick")]
    private void NoButtonOnClick()
    {
        _userChoseToDownloadMapsCallback?.Invoke(false);
        _userChoseToDownloadMapsCallback = null;
    }

    [UIValue("automaticallyDownloadNewMaps")]
    private bool AutomaticallyDownloadNewMaps
    {
        get => _config.DownloadMapsAutomatically;
        set => _config.DownloadMapsAutomatically = value;
    }
}