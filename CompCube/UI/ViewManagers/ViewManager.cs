using BeatSaberMarkupLanguage;
using CompCube.UI.FlowCoordinators;
using HMUI;
using CompCube.UI.FlowCoordinators.Events;
using UnityEngine;
using Zenject;

namespace CompCube.UI.ViewManagers;

public abstract class ViewManager : MonoBehaviour, IInitializable, IDisposable
{
    public abstract ViewController ManagedController { get; }
    
    [Inject] private readonly MatchmakingMenuFlowCoordinator _matchmakingMenuFlowCoordinator = null;
    [Inject] private readonly MainFlowCoordinator _mainFlowCoordinator = null;

    public void Initialize()
    {
        ManagedController.didActivateEvent += OnManagedControllerActivated;
        ManagedController.didDeactivateEvent += OnManagedControllerDeactivated;
    }

    public void Dispose()
    {
        ManagedController.didActivateEvent -= OnManagedControllerActivated;
        ManagedController.didDeactivateEvent -= OnManagedControllerDeactivated;
    }

    private void OnManagedControllerActivated(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (!_mainFlowCoordinator.IsFlowCoordinatorInHierarchy(_matchmakingMenuFlowCoordinator))
            return;
        
        SetupManagedController();
    }

    private void OnManagedControllerDeactivated(bool removedFromHierarchy, bool screenSystemDisabling) => ResetManagedController();
    
    protected virtual void SetupManagedController(){}
    protected virtual void ResetManagedController(){}
}