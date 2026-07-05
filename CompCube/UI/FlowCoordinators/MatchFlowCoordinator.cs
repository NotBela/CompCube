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
using CompCube.UI.BSML.Match.Modal;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace CompCube.UI.FlowCoordinators
{
    public class MatchFlowCoordinator : FlowCoordinator
    {
        [Inject] private readonly VotingScreenViewController _votingScreenViewController = null!;
        [Inject] private readonly WaitingForMatchToStartViewController _waitingForMatchToStartViewController = null!;
        [Inject] private readonly AwaitMatchEndViewController _awaitMatchEndViewController = null!;
        [Inject] private readonly RoundResultsViewController _roundResultsViewController = null!;
        [Inject] private readonly OpponentViewController _opponentViewController = null!;
        [Inject] private readonly MatchResultsViewController _matchResultsViewController = null!;
        [Inject] private readonly EarlyLeaveWarningModalViewController _earlyLeaveWarningModalViewController = null!;
        [Inject] private readonly WaitingForOpponentsPickViewController _waitingForOpponentsPickViewController = null!;
        
        [Inject] private readonly PhasePopupViewController _phasePopupViewController = null!;
         
        [Inject] private readonly IServerListener _serverListener = null!;
        [Inject] private readonly TransitionToLevelManager _transitionToLevelManager = null!;
        [Inject] private readonly MatchStateManager _matchStateManager = null!;
        
        [Inject] private readonly SiraLog _siraLog = null!;
        
        [Inject] private readonly StandardLevelDetailViewManager _standardLevelDetailViewManager = null!;
        [Inject] private readonly GameplaySetupViewManager _gameplaySetupViewManager = null!;
        
        [Inject] private readonly DisconnectHandler _disconnectHandler = null!;

        [Inject] private readonly DisconnectFlowCoordinator _disconnectFlowCoordinator = null!;
        [Inject] private readonly DisconnectedViewController _disconnectedViewController = null!;
        
        [Inject] private readonly SoundEffectManager _soundEffectManager = null!;
        
        [Inject] private readonly PlatformLeaderboardViewController _platformLeaderboardViewController = null!;

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
                StartCoroutine(ShowPhaseChangeModal("Discard Phase", "Discard maps that you don't want to play!"));
            }
        }

        private IEnumerator ShowPhaseChangeModal(string topText, string subText)
        {
            yield return new WaitUntil(() => !isInTransition && !topViewController.isInTransition);
                
            _phasePopupViewController.ParseOntoObject(topViewController.gameObject, topText, subText);
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
            _serverListener.OnPickPhaseStarted += OnPickPhaseStarted;
        }

        private void OnPickPhaseStarted(StartPickPhasePacket packet)
        {
            // make it so this swaps based on round count
            if (_matchStateManager.IsRedTeam)
            {
                StartCoroutine(PickMapCoroutine());
                StartCoroutine(ShowPhaseChangeModal("Pick Phase", $"{_matchStateManager.RedPlayer.GetFormattedUserName()}'s Pick"));
                return;
            }

            StartCoroutine(WaitForMapToBePickedCoroutine());
            StartCoroutine(ShowPhaseChangeModal("Pick Phase", $"{_matchStateManager.BluePlayer.GetFormattedUserName()}'s Pick"));
            return;
            
            IEnumerator PickMapCoroutine()
            {
                this.ReplaceViewControllerSynchronously(_votingScreenNavigationController);
                
                yield return new WaitUntil(() => _votingScreenViewController.isActivated);
                
                _votingScreenViewController.PopulateData(packet.AvailableMaps, 30);
            }

            IEnumerator WaitForMapToBePickedCoroutine()
            {
                yield return new WaitUntil(() => !_standardLevelDetailViewManager.ManagedController.isActivated);
                
                this.ReplaceViewControllerSynchronously(_waitingForOpponentsPickViewController);
                _waitingForOpponentsPickViewController.PopulateData(_matchStateManager.Opponent.GetFormattedUserName());
            }
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
            
            _standardLevelDetailViewManager.SetData(votingMap, HandleStandardLevelDetailButtonPressed, _matchStateManager.InDiscardPhase ? "Discard" : "Select", _matchStateManager.CanDiscardMaps || !_matchStateManager.InDiscardPhase);
            ShowLeaderboard(votingMap);
            _soundEffectManager.PlayBeatmapLevelPreview(votingMap.GetBeatmapLevel()!);
        }

        private async void HandleStandardLevelDetailButtonPressed(VotingMap votingMap)
        {
            try
            {
                _votingScreenNavigationController.PopViewController(() => {}, true);
                _soundEffectManager.CrossfadeToDefault();
                _votingScreenViewController.ClearSelection();
                _votingScreenViewController.DiscardMap(votingMap);
                HideLeaderboard();
                
                if (_matchStateManager.InDiscardPhase)
                {
                    await _serverListener.SendPacket(new DiscardMapPacket(votingMap));
                    return;
                }
                
                await _serverListener.SendPacket(new MapSelectionPacket(votingMap));
                ShowMapPreviewViewAndStartMatch(votingMap);
            }
            catch (Exception e)
            {
                _siraLog.Error(e);
            }
        }

        # region Leaderboard Functions
        private void ShowLeaderboard(VotingMap votingMap)
        {
            _platformLeaderboardViewController.SetData(votingMap.GetBeatmapKey());
            SetRightScreenViewController(_platformLeaderboardViewController, ViewController.AnimationType.In);
        }

        private void HideLeaderboard()
        {
            SetRightScreenViewController(null, ViewController.AnimationType.Out);
        }

        #endregion
        private void ShowMapPreviewViewAndStartMatch(VotingMap map)
        {
            StartCoroutine(WaitForSecondsAndStartMatch());
            return;
            
            IEnumerator WaitForSecondsAndStartMatch()
            {
                ShowLeaderboard(map);
                _soundEffectManager.PlayGongSoundEffect();
                this.ReplaceViewControllerSynchronously(_waitingForMatchToStartViewController);
                
                yield return new WaitUntil(() => _waitingForMatchToStartViewController.isActivated);
                
                _waitingForMatchToStartViewController.PopulateData(map, DateTime.Now.AddSeconds(15));
                
                yield return new WaitForSeconds(15);
                
                _transitionToLevelManager.StartLevel(map, DateTime.Now.AddSeconds(25), _gameplaySetupViewManager.ProMode, async void (results, transitionSetupDataSo) =>
                {
                    try
                    {
                        this.ReplaceViewControllerSynchronously(_awaitMatchEndViewController, true);
                        await _serverListener.SendPacket(new ScoreSubmissionPacket(results.multipliedScore,
                            ScoreModel.ComputeMaxMultipliedScoreForBeatmap(transitionSetupDataSo.transformedBeatmapData),
                            results.gameplayModifiers.proMode, results.notGoodCount, results.fullCombo));
                    }
                    catch (Exception e)
                    {
                        _siraLog.Error(e);
                    }
                });
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