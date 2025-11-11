using System.Collections;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.ServerPackets.Event;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube.Game;
using CompCube.Interfaces;
using CompCube.UI.BSML.Match;
using CompCube.UI.Sound;
using CompCube.UI.ViewManagers;
using HMUI;
using JetBrains.Annotations;
using CompCube.Extensions;
using CompCube.UI.BSML.Events;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace CompCube.UI.FlowCoordinators.Events;

public class InEventFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly IServerListener _serverListener = null!;
    
    [Inject] private readonly EventWaitingOnNextMatchViewController _eventWaitingOnNextMatchViewController = null!;
    [Inject] private readonly GameplaySetupViewManager _gameplaySetupViewManager = null!;
    [Inject] private readonly WaitingForMatchToStartViewController _waitingForMatchToStartViewController = null!;

    [Inject] private readonly SoundEffectManager _soundEffectManager = null!;

    private Action? _backButtonPressedCallback;
    
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        showBackButton = true;
        SetTitle("Event");
        ProvideInitialViewControllers(_eventWaitingOnNextMatchViewController, leftScreenViewController: _gameplaySetupViewManager.ManagedController);
        
        _serverListener.OnEventStarted += ServerListenerOnOnEventStarted;
        _serverListener.OnEventMapSelected += OnEventMapSelected;
    }

    private void OnEventMapSelected(EventMapSelected eventMap)
    {
        this.ReplaceViewControllerSynchronously(_waitingForMatchToStartViewController);
        _waitingForMatchToStartViewController.PopulateData(eventMap.Map, null);
        
        _soundEffectManager.PlayGongSoundEffect();
    }

    private void ServerListenerOnOnEventStarted(EventStartedPacket eventStartedPacket)
    {
        this.SetBackButtonInteractivity(false);
        
        _eventWaitingOnNextMatchViewController.SetText("Event in progress!\nWaiting for host...");
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        _serverListener.Disconnect();
    }

    public void Setup(Action? backButtonPressedCallback) => _backButtonPressedCallback = backButtonPressedCallback;

    protected override void BackButtonWasPressed(ViewController _) => _backButtonPressedCallback?.Invoke();
}