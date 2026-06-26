using System.Collections;
using BeatSaberMarkupLanguage;
using CompCube_Models.Models.Map;
using CompCube_Models.Models.Packets;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube.Game;
using CompCube.Interfaces;
using CompCube.UI.BSML.Disconnect;
using CompCube.UI.BSML.Match;
using CompCube.UI.Sound;
using CompCube.UI.ViewManagers;
using HMUI;
using CompCube.Extensions;
using CompCube.UI.BSML.EarlyLeaveWarning;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace CompCube.UI.FlowCoordinators
{
    public class MatchFlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly VotingScreenViewController _votingScreenViewController = null!;
        [Inject] private readonly AwaitingMapDecisionViewController _awaitingMapDecisionViewController = null!;
        [Inject] private readonly WaitingForMatchToStartViewController _waitingForMatchToStartViewController = null!;
        [Inject] private readonly AwaitMatchEndViewController _awaitMatchEndViewController = null!;
        [Inject] private readonly RoundResultsViewController _roundResultsViewController = null!;
        [Inject] private readonly OpponentViewController _opponentViewController = null!;
        [Inject] private readonly MatchResultsViewController _matchResultsViewController = null!;
        [Inject] private readonly EarlyLeaveWarningModalViewController _earlyLeaveWarningModalViewController = null!;
         
        [Inject] private readonly IServerListener _serverListener = null!;
        [Inject] private readonly MatchManager _matchManager = null!;
        [Inject] private readonly MatchStateManager _matchStateManager = null!;
        
        [Inject] private readonly SiraLog _siraLog = null!;
        
        [Inject] private readonly StandardLevelDetailViewManager _standardLevelDetailViewManager = null!;
        [Inject] private readonly GameplaySetupViewManager _gameplaySetupViewManager = null!;
        
        [Inject] private readonly DisconnectHandler _disconnectHandler = null!;

        [Inject] private readonly DisconnectFlowCoordinator _disconnectFlowCoordinator = null!;
        [Inject] private readonly DisconnectedViewController _disconnectedViewController = null!;
        
        [Inject] private readonly SoundEffectManager _soundEffectManager = null!;

        private NavigationController _votingScreenNavigationController;

        private Action? _onMatchFinishedCallback = null;

        public void PopulateData(MatchCreatedPacket packet, Action? onMatchFinishedCallback)
        {
            _opponentViewController.PopulateData(packet.Red, packet.Blue);
            _opponentViewController.UpdateRound(1);
            _opponentViewController.UpdatePoints(0, 0);

            StartCoroutine(WaitForVotingScreenToPresent());
            
            _onMatchFinishedCallback = onMatchFinishedCallback;
            return;
            
            IEnumerator WaitForVotingScreenToPresent()
            {
                yield return new WaitUntil(() => _votingScreenViewController.isActivated);
                
                _votingScreenViewController.PopulateData(packet.InitialMaps, 30);
            }
        }
        
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            SetTitle("Match Room");
            showBackButton = true;
            
            _votingScreenNavigationController = BeatSaberUI.CreateViewController<NavigationController>();
            
            ProvideInitialViewControllers(_votingScreenNavigationController, _gameplaySetupViewManager.ManagedController, bottomScreenViewController: _opponentViewController);
            _votingScreenNavigationController.PushViewController(_votingScreenViewController, null);
            
            _disconnectHandler.ShouldShowDisconnectScreen += HandleShouldShowDisconnectScreen;
            _votingScreenViewController.MapSelected += HandleVotingScreenMapSelected;
        }

        private void HandleShouldShowDisconnectScreen(string reason, bool matchOnly)
        {
            while (!isActivated);
            
            this.PresentFlowCoordinatorSynchronously(_disconnectFlowCoordinator);
            
            _disconnectedViewController.SetReason(reason, async void () =>
            {
                try
                {
                    await DismissChildFlowCoordinatorsRecursively();
                }
                catch(Exception e)
                {
                    _siraLog.Error(e);
                }
            });
        }
        
        private void HandleVotingScreenMapSelected(VotingMap votingMap)
        {
            if (!_standardLevelDetailViewManager.ManagedController.isActivated)
                _votingScreenNavigationController.PushViewController(_standardLevelDetailViewManager.ManagedController,
                    () =>
                    {
                        _standardLevelDetailViewManager.ManagedController.transform.position = new Vector3(1.4f,
                            _standardLevelDetailViewManager.ManagedController.transform.position.y, 
                            _standardLevelDetailViewManager.ManagedController.transform.position.z);
                    });
            
            _standardLevelDetailViewManager.SetData(votingMap, HandleButtonPressed, "Discard", _matchStateManager.CanDiscardMaps);
            
            _soundEffectManager.PlayBeatmapLevelPreview(votingMap.GetBeatmapLevel()!);
        }

        private async void HandleButtonPressed(VotingMap votingMap)
        {
            try
            {
                _votingScreenNavigationController.PopViewController(() => {}, false);_votingScreenNavigationController.PopViewController(() => {}, false);
                _soundEffectManager.CrossfadeToDefault();
                _votingScreenViewController.ClearSelection();
                
                if (_matchStateManager.InDiscardPhase)
                {
                    _votingScreenViewController.DiscardMap(votingMap);
                    await _serverListener.SendPacket(new DiscardMapPacket(votingMap));
                    return;
                }
                
                
            }
            catch (Exception e)
            {
                _siraLog.Error(e);
            }
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            _disconnectHandler.ShouldShowDisconnectScreen -= HandleShouldShowDisconnectScreen;
        }

        private void ResetNavigationController()
        {
            if (_votingScreenNavigationController)
                Destroy(_votingScreenNavigationController);
            _votingScreenNavigationController = BeatSaberUI.CreateViewController<NavigationController>();
            _votingScreenNavigationController.PushViewController(
                                                    _votingScreenViewController, null);
        }

        protected override void BackButtonWasPressed(ViewController viewController)
        {
            _earlyLeaveWarningModalViewController.ParseOntoGameObject(viewController, "Are you sure you want to leave the match early?\nLeaving the match early could result in penalties!", () =>
            {
                _onMatchFinishedCallback?.Invoke();
            });
        }
    }
}