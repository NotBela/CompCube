using CompCube.Interfaces;
using UnityEngine.SceneManagement;
using Zenject;

namespace CompCube.Game.MatchState;

public class DisconnectHandler : IInitializable, IDisposable
{
    [Inject] private readonly IServerListener _serverListener = null!;
    [Inject] private readonly TransitionToLevelManager _transitionToLevelManager = null!;
    
    public event Action<string>? ShouldShowDisconnectScreen;
    
    public void Initialize()
    {
        _serverListener.OnDisconnected += OnDisconnect;
    }

    private void EndLevelAndShowDisconnectScreen(string reason)
    {

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ShouldShowDisconnectScreen?.Invoke(reason);
            return;
        }
        
        _transitionToLevelManager.StopLevel((levelDetails, sceneTransitionSetupData) =>
        {
            ShouldShowDisconnectScreen?.Invoke(reason);
        });
    }

    private void OnDisconnect()
    {
        EndLevelAndShowDisconnectScreen("Disconnected");
    }

    public void Dispose()
    {
        _serverListener.OnDisconnected -= OnDisconnect;
    }
}