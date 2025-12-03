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
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace CompCube.UI.FlowCoordinators
{
    public class MatchFlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly VotingScreenViewController _votingScreenViewController = null;
        [Inject] private readonly AwaitingMapDecisionViewController _awaitingMapDecisionViewController = null;
        [Inject] private readonly WaitingForMatchToStartViewController _waitingForMatchToStartViewController = null;
        [Inject] private readonly AwaitMatchEndViewController _awaitMatchEndViewController = null;
        [Inject] private readonly MatchResultsViewController _matchResultsViewController = null;
        [Inject] private readonly OpponentViewController _opponentViewController = null;
        
        [Inject] private readonly IServerListener _serverListener = null;
        [Inject] private readonly MatchManager _matchManager = null;
        
        [Inject] private readonly SiraLog _siraLog = null;
        
        [Inject] private readonly StandardLevelDetailViewManager _standardLevelDetailViewManager = null;
        [Inject] private readonly GameplaySetupViewManager _gameplaySetupViewManager = null;
        
        [Inject] private readonly DisconnectHandler _disconnectHandler = null;

        [Inject] private readonly DisconnectFlowCoordinator _disconnectFlowCoordinator = null;
        [Inject] private readonly DisconnectedViewController _disconnectedViewController = null;
        
        [Inject] private readonly IPlatformUserModel _platformUserModel = null;
        [Inject] private readonly SoundEffectManager _soundEffectManager = null;

        private NavigationController _votingScreenNavigationController;

        private Action? _onMatchFinishedCallback = null;

        public void PopulateData(MatchCreatedPacket packet, Action? onMatchFinishedCallback)
        {
            _matchManager.SetOpponents(packet.Red, packet.Blue);
            
            _onMatchFinishedCallback = onMatchFinishedCallback;
        }
        
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            SetTitle("Match Room");
            showBackButton = false;
            
            _votingScreenNavigationController = BeatSaberUI.CreateViewController<NavigationController>();
            
            ProvideInitialViewControllers(_votingScreenNavigationController, _gameplaySetupViewManager.ManagedController, bottomScreenViewController: _opponentViewController);
            _votingScreenNavigationController.PushViewController(_votingScreenViewController, null);
            
            _serverListener.OnRoundStarted += OnRoundStarted;
            _serverListener.OnBeginGameTransition += TransitionToGame;
            _serverListener.OnRoundResults += OnRoundResults;
        }

        private void OnRoundResults(RoundResultsPacket results)
        {
            _matchResultsViewController.PopulateData();
        }

        private async void TransitionToGame(BeginGameTransitionPacket packet)
        {
            try
            {
                this.ReplaceViewControllerSynchronously(_waitingForMatchToStartViewController);
                await _waitingForMatchToStartViewController.PopulateData(packet.Map, DateTime.Now.AddSeconds(packet.TransitionToGameTime));
            
                await Task.Delay(packet.TransitionToGameTime * 1000);
            
                _matchManager.StartMatch(packet.Map, DateTime.Now.AddSeconds(packet.UnpauseTime), _gameplaySetupViewManager.ProMode,
                    (results, so) =>
                    {
                        this.ReplaceViewControllerSynchronously(_awaitMatchEndViewController, true);
                    });
            }
            catch (Exception e)
            {
                _siraLog.Error(e);
            }
        }

        private void OnRoundStarted(RoundStartedPacket roundStartedPacket)
        {
            if (!_votingScreenViewController.isActivated)
            {
                _votingScreenViewController.SetActivationCallback(() =>
                {
                    _votingScreenViewController.PopulateData(roundStartedPacket.Maps, roundStartedPacket.VotingTime);
                });
                return;
            }
            
            _votingScreenViewController.PopulateData(roundStartedPacket.Maps, roundStartedPacket.VotingTime);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            
        }
    }
}