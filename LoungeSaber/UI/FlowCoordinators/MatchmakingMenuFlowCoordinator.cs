using System.Collections;
using System.Reflection;
using HarmonyLib;
using HMUI;
using IPA.Utilities;
using LoungeSaber.Extensions;
using LoungeSaber.Game;
using LoungeSaber.Interfaces;
using LoungeSaber.Models.Packets.ServerPackets;
using LoungeSaber.Models.Server;
using LoungeSaber.Server;
using LoungeSaber.UI.BSML.Disconnect;
using LoungeSaber.UI.BSML.Leaderboard;
using LoungeSaber.UI.BSML.Match;
using LoungeSaber.UI.BSML.Menu;
using LoungeSaber.UI.FlowCoordinators.Events;
using SiraUtil.Logging;
using SongCore;
using Zenject;
using UnityEngine;

namespace LoungeSaber.UI.FlowCoordinators
{
    public class MatchmakingMenuFlowCoordinator : FlowCoordinator, IInitializable, IDisposable
    {
        [Inject] private readonly MainFlowCoordinator _mainFlowCoordinator = null;
        [Inject] private readonly MatchFlowCoordinator _matchFlowCoordinator = null;
        [Inject] private readonly InfoFlowCoordinator _infoFlowCoordinator = null;
        
        [Inject] private readonly IServerListener _serverListener = null;
        [Inject] private readonly MatchmakingMenuViewController _matchmakingMenuViewController = null;
        
        // [Inject] private readonly LoungeSaberLeaderboardViewController _leaderboardViewController = null;
        [Inject] private readonly RankingDataTabSwitcherViewController _rankingDataTabSwitcherViewController = null;
        [Inject] private readonly DisconnectFlowCoordinator _disconnectFlowCoordinator = null;
        
        [Inject] private readonly EventsFlowCoordinator _eventsFlowCoordinator = null;
        
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            showBackButton = true;
            SetTitle("LoungeSaber");
            ProvideInitialViewControllers(_matchmakingMenuViewController, rightScreenViewController: _rankingDataTabSwitcherViewController);
        }

        private void OnMatchCreated(MatchCreatedPacket packet)
        {
            this.PresentFlowCoordinatorSynchronously(_matchFlowCoordinator);

            _matchFlowCoordinator.StartMatch(packet, () =>
            {
                DismissFlowCoordinator(_matchFlowCoordinator);
            });
        }

        public void Dispose()
        {
            _serverListener.OnMatchCreated -= OnMatchCreated;
            _matchmakingMenuViewController.AboutButtonClicked -= OnAboutButtonClicked;
            _infoFlowCoordinator.OnBackButtonPressed -= OnInfoFlowCoordinatorBackButtonPressed;
            _matchmakingMenuViewController.EventsButtonClicked -= OnEventsButtonClicked;
            _eventsFlowCoordinator.OnBackButtonPressed -= EventsFlowCoordinatorOnBackButtonPressed;
            _matchmakingMenuViewController.OnJoinFailed -= OnJoinFailed;
        }
        
        public void Initialize()
        {
            _serverListener.OnMatchCreated += OnMatchCreated;
            _matchmakingMenuViewController.AboutButtonClicked += OnAboutButtonClicked;
            _infoFlowCoordinator.OnBackButtonPressed += OnInfoFlowCoordinatorBackButtonPressed;
            _matchmakingMenuViewController.EventsButtonClicked += OnEventsButtonClicked;
            _eventsFlowCoordinator.OnBackButtonPressed += EventsFlowCoordinatorOnBackButtonPressed;
            _matchmakingMenuViewController.OnJoinFailed += OnJoinFailed;
        }

        private void OnJoinFailed(JoinResponse response)
        {
            this.PresentFlowCoordinatorSynchronously(_disconnectFlowCoordinator);
            
            _disconnectFlowCoordinator.Setup(response.Message, () =>
            {
                DismissFlowCoordinator(_disconnectFlowCoordinator);
            });
        }

        private void EventsFlowCoordinatorOnBackButtonPressed() => DismissFlowCoordinator(_eventsFlowCoordinator);

        private void OnEventsButtonClicked() => this.PresentFlowCoordinatorSynchronously(_eventsFlowCoordinator);

        private void OnInfoFlowCoordinatorBackButtonPressed() => DismissFlowCoordinator(_infoFlowCoordinator);

        private void OnAboutButtonClicked()
        {
            this.PresentFlowCoordinatorSynchronously(_infoFlowCoordinator);
        }

        protected override void BackButtonWasPressed(ViewController _)
        {
            _serverListener.Disconnect();
            _mainFlowCoordinator.DismissAllChildFlowCoordinators();
            
            // _mainFlowCoordinator.GetType().GetMethod("DismissChildFlowCoordinatorsRecursively", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(_mainFlowCoordinator,
            //     [false]);
        }
    }
}  