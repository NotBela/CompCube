using System.Collections;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube_Models.Models.Packets.UserPackets;
using CompCube.Game;
using CompCube.Interfaces;
using CompCube.UI.BSML.Match;
using CompCube.UI.Sound;
using CompCube.UI.ViewManagers;
using HMUI;
using JetBrains.Annotations;
using CompCube.Extensions;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace CompCube.UI.FlowCoordinators.Events;

public class EventMatchFlowCoordinator : FlowCoordinator
{
    [Inject] private readonly WaitingForMatchToStartViewController _waitingForMatchToStartViewController = null;
    [Inject] private readonly GameplaySetupViewManager _gameplaySetupViewManager = null;
    [Inject] private readonly OpponentViewController _opponentViewController = null;
    [Inject] private readonly MatchResultsViewController _resultsViewController = null;
    
    [Inject] private readonly AwaitMatchEndViewController _awaitMatchEndViewController = null;
    
    [Inject] private readonly IServerListener _serverListener = null;
    
    [Inject] private readonly DisconnectHandler _disconnectHandler = null;
    
    [Inject] private readonly DisconnectFlowCoordinator _disconnectFlowCoordinator = null;
    [Inject] private  readonly MatchmakingMenuFlowCoordinator _matchmakingMenuFlowCoordinator = null;
    
    [Inject] private readonly MatchManager _matchManager = null;
    
    [Inject] private readonly SoundEffectManager _soundEffectManager = null;
    
    [Inject] private readonly IPlatformUserModel _platformUserModel = null;
    
    [CanBeNull] private Action _matchEndedCallback;
    
    [Inject] private readonly SiraLog _siraLog = null;
     
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        SetTitle("Event Room");
        showBackButton = false;
        
        ProvideInitialViewControllers(_waitingForMatchToStartViewController, _gameplaySetupViewManager.ManagedController, bottomScreenViewController: _opponentViewController);
        
        _soundEffectManager.PlayGongSoundEffect();
        
        _disconnectHandler.ShouldShowDisconnectScreen += ShouldShowDisconnectScreen;
        _serverListener.OnMatchResults += OnMatchResults;
    }

    private void OnMatchResults(MatchResultsPacket results)
    {
        this.ReplaceViewControllerSynchronously(_resultsViewController);
        _resultsViewController.PopulateData(results, _matchEndedCallback);
        
        _matchEndedCallback = null;

        if (results.WinnerScore.User.UserId !=
            _platformUserModel.GetUserInfo(CancellationToken.None).Result.platformUserId)
            return;
        
        _soundEffectManager.PlayWinningMusic();
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        _disconnectHandler.ShouldShowDisconnectScreen -= ShouldShowDisconnectScreen;
        _serverListener.OnMatchResults -= OnMatchResults;
    }

    private void ShouldShowDisconnectScreen(string reason, bool matchOnly)
    {
        var callback = () =>
        {
            _matchmakingMenuFlowCoordinator.DismissAllChildFlowCoordinators();
        };

        if (matchOnly)
            callback = () =>
            {
                _matchEndedCallback?.Invoke();
                _matchEndedCallback = null;
            };
        
        this.PresentFlowCoordinatorSynchronously(_disconnectFlowCoordinator);
        _disconnectFlowCoordinator.Setup(reason, callback);
    }

    public async void Setup(MatchStartedPacket eventMatchCreatedPacket, Action matchEndedCallback)
    {
        try
        {
            _matchEndedCallback = matchEndedCallback;
        
            _opponentViewController.PopulateData(eventMatchCreatedPacket.Opponent);

            StartCoroutine(PopulateWaitingScreen());

            await Task.Delay(eventMatchCreatedPacket.TransitionToGameWait);
            
            _matchManager.StartMatch(eventMatchCreatedPacket.MapSelected, DateTime.UtcNow.AddSeconds(eventMatchCreatedPacket.StartingWait), _gameplaySetupViewManager.ProMode, eventMatchCreatedPacket.Opponent, 
                OnLevelCompletedCallback);

            return;
            
            IEnumerator PopulateWaitingScreen()
            {
                yield return new WaitForEndOfFrame();
            
                _ = _waitingForMatchToStartViewController.PopulateData(eventMatchCreatedPacket.MapSelected, DateTime.UtcNow.AddSeconds(eventMatchCreatedPacket.TransitionToGameWait));
            }
        }
        catch (Exception e)
        {
            _siraLog.Error(e);
        }
    }
    
    private void OnLevelCompletedCallback(LevelCompletionResults levelCompletionResults, StandardLevelScenesTransitionSetupDataSO sceneTransitionSetupDataSo)
    {
        if (_disconnectHandler.WillShowDisconnectScreen) 
            return;

        _serverListener.SendPacket(new ScoreSubmissionPacket(levelCompletionResults.multipliedScore, ScoreModel.ComputeMaxMultipliedScoreForBeatmap(sceneTransitionSetupDataSo.transformedBeatmapData), levelCompletionResults.gameplayModifiers.proMode, levelCompletionResults.notGoodCount, levelCompletionResults.fullCombo));

        this.ReplaceViewControllerSynchronously(_awaitMatchEndViewController, true);
        SetLeftScreenViewController(null, ViewController.AnimationType.None);
        SetBottomScreenViewController(null, ViewController.AnimationType.None);
    }
}