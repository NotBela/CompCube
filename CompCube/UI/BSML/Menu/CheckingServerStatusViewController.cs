using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube.Game;
using CompCube.Server;
using Zenject;

namespace CompCube.UI.BSML.Menu;

[ViewDefinition("CompCube.UI.BSML.Menu.CheckingServerStatusView.bsml")]
public class CheckingServerStatusViewController : BSMLAutomaticViewController, IInitializable, IDisposable
{
    [Inject] private readonly MapDownloader _mapDownloader = null;
    [Inject] private readonly InitialServerChecker _initialServerChecker = null;
    
    [UIValue("stateText")] private string StateText { get; set; } = "placeholder";

    public void SetControllerState(InitialServerChecker.ServerCheckingStates state)
    {
        switch (state)
        {
            case InitialServerChecker.ServerCheckingStates.ServerStatus:
                StateText = "Connecting to server...";
                break;
            case InitialServerChecker.ServerCheckingStates.Maps:
                StateText = "Fetching maps...";
                break;
            case InitialServerChecker.ServerCheckingStates.UserData:
                StateText = "Fetching user data...";
                break;
            case InitialServerChecker.ServerCheckingStates.DownloadingMaps:
                StateText = "Downloading maps...";
                _mapDownloader.OnMapDownloaded += OnMapDownloaded;
                break;
        };
        NotifyPropertyChanged(nameof(StateText));
    }

    private void OnMapDownloaded(int mapsDownloaded, int totalMaps)
    {
        if (mapsDownloaded == totalMaps)
            _mapDownloader.OnMapDownloaded -= OnMapDownloaded;
        
        StateText = $"Downloading maps... ({mapsDownloaded}/{totalMaps})";
        
        NotifyPropertyChanged(nameof(StateText));
    }

    public void Initialize()
    {
        _initialServerChecker.ServerCheckingStateUpdated += SetControllerState;
    }

    public void Dispose()
    {
        _initialServerChecker.ServerCheckingStateUpdated -= SetControllerState;
    }
}