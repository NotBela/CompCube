using BeatSaberMarkupLanguage.Attributes;
using CompCube_Models.Models.Events;
using CompCube_Models.Models.Packets.ServerPackets.Event;
using CompCube.Interfaces;
using CompCube.UI.BSML.Disconnect;
using CompCube.UI.BSML.Events;
using HMUI;
using CompCube.Extensions;
using SiraUtil.Logging;
using Zenject;

namespace CompCube.UI.FlowCoordinators.Events;

public class EventsFlowCoordinator : FlowCoordinator, IInitializable, IDisposable
{
    [Inject] private readonly EventsListViewController _eventsListViewController = null;
    [Inject] private readonly SiraLog _siraLog = null;
    
    [Inject] private readonly IServerListener _serverListener = null;
    
    [Inject] private readonly DisconnectFlowCoordinator _disconnectFlowCoordinator = null;
    [Inject] private readonly DisconnectedViewController _disconnectedViewController = null;
    
    [Inject] private readonly WaitingForEventMatchFlowCoordinator _waitingForEventMatchFlowCoordinator = null;
    
    public event Action OnBackButtonPressed;
    
    protected override async void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        try
        {
            SetTitle("Events");
            showBackButton = true;
            ProvideInitialViewControllers(_eventsListViewController);

            while (!_eventsListViewController.Parsed)
                await Task.Delay(25);

            await _eventsListViewController.RefreshData();
        }
        catch (Exception e)
        {
            _siraLog.Error(e);
        }
    }

    private void WaitingForEventFlowCoordinatorOnBackButtonPressed() => DismissFlowCoordinator(_waitingForEventMatchFlowCoordinator);

    private async void OnEventJoinRequested(EventData data)
    {
        try
        {
            await _serverListener.Connect(data.EventName, response =>
            {
                if (response.Successful)
                {
                    this.PresentFlowCoordinatorSynchronously(_waitingForEventMatchFlowCoordinator);
                    return;
                }
            
                this.PresentFlowCoordinatorSynchronously(_disconnectFlowCoordinator);
            
                _disconnectedViewController.SetReason(response.Message, () =>
                {
                    DismissFlowCoordinator(_disconnectFlowCoordinator);
                });
            });
        }
        catch (Exception e)
        {
            _siraLog.Error(e);
        }
    }

    protected override void BackButtonWasPressed(ViewController _) => OnBackButtonPressed?.Invoke();
    public void Initialize()
    {
        _eventsListViewController.OnEventJoinRequested += OnEventJoinRequested;
        _waitingForEventMatchFlowCoordinator.OnBackButtonPressed += WaitingForEventFlowCoordinatorOnBackButtonPressed;
        _serverListener.OnOutOfEvent += OnOutOfEvent;
    }

    private void OnOutOfEvent(OutOfEventPacket outOfEvent)
    {
        
    }

    public void Dispose()
    {
        _eventsListViewController.OnEventJoinRequested -= OnEventJoinRequested;
        _waitingForEventMatchFlowCoordinator.OnBackButtonPressed -= WaitingForEventFlowCoordinatorOnBackButtonPressed;
    }
}