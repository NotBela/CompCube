using System.Diagnostics;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube.Configuration;
using CompCube.Interfaces;
using SiraUtil.Logging;
using UnityEngine.UI;
using Zenject;

namespace CompCube.UI.BSML.Menu
{
    [ViewDefinition("CompCube.UI.BSML.Menu.MatchmakingMenuView.bsml")]
    public class MatchmakingMenuViewController : BSMLAutomaticViewController, ITickable
    {
        [Inject] private readonly PluginConfig _config = null;
        [Inject] private readonly IServerListener _serverListener = null;
        [Inject] private readonly SiraLog _siraLog = null;

        [UIParams] private readonly BSMLParserParams _parserParams = null;

        public event Action AboutButtonClicked;

        public event Action EventsButtonClicked;

        public event Action<JoinResponsePacket> OnJoinFailed; 
        
        protected override void DidDeactivate(bool firstActivation, bool addedToHierarchy)
        {
            _leaveMatchmakingPoolButton.gameObject.SetActive(false);
            ResetMatchmakingTimer();
            ChangeButtonState(false);
        }

        [UIAction("aboutButtonOnClick")]
        private void AboutButtonOnClick() => AboutButtonClicked?.Invoke();

        [UIComponent("aboutButton")] private readonly Button _aboutButton = null;

        [UIComponent("eventsButton")] private readonly Button _eventsButton = null;

        [UIAction("eventsButtonOnClick")]
        private void EventsButtonOnClick() => EventsButtonClicked?.Invoke();

        private void ChangeButtonState(bool inMatch)
        {
            _aboutButton.interactable = !inMatch;
            _leaveMatchmakingPoolButton.gameObject.SetActive(inMatch);
            _joinMatchmakingPoolButton.interactable = !inMatch;
            _leaveMatchmakingPoolButton.gameObject.SetActive(inMatch);
            _eventsButton.interactable = !inMatch;
        }

        #region Queue Control
        private readonly Stopwatch _matchmakingTimeStopwatch = new();
        
        [UIComponent("joinMatchmakingPoolButton")] private readonly Button _joinMatchmakingPoolButton = null;
        [UIComponent("leaveMatchmakingPoolButton")] private readonly Button _leaveMatchmakingPoolButton = null;
        
        private void ResetMatchmakingTimer()
        {
            _matchmakingTimeStopwatch.Stop();
            _joinMatchmakingPoolButton.interactable = true;
            _joinMatchmakingPoolButton.SetButtonText("Find Match");
        }
        

        [UIAction("leaveMatchmakingPoolButtonOnClick")]
        private void LeaveMatchmakingPoolButtonOnClick() => _parserParams.EmitEvent("disconnectModalShowEvent");

        [UIAction("leaveMatchmakingPoolDenyButtonOnClick")]
        private void LeaveMatchmakingPoolDenyButtonOnClick() =>
            _parserParams.EmitEvent("disconnectModalHideEvent");

        [UIAction("leaveMatchmakingPoolAllowButtonOnClick")]
        private void LeaveMatchmakingPoolAllowButton()
        {
            ChangeButtonState(false);
            
            _parserParams.EmitEvent("disconnectModalHideEvent");
            ResetMatchmakingTimer();
            _serverListener.Disconnect();
        }
        
        [UIAction("joinMatchmakingPoolButtonOnClick")]
        private async void JoinMatchmakingPoolButtonOnClick()
        {
            try
            {
                ChangeButtonState(true);
                _matchmakingTimeStopwatch.Stop();
                _joinMatchmakingPoolButton.SetButtonText("Finding Match (Joining Pool...)");

                await _serverListener.Connect(_config.ConnectToDebugQueue ? "debug" : "standard", OnConnectedCallback);
            }
            catch (Exception e)
            {
                _siraLog.Error(e);
            }
        }

        private void OnConnectedCallback(JoinResponsePacket joinResponse)
        {
            if (joinResponse.Successful)
            {
                _matchmakingTimeStopwatch.Restart();
                return;
            }
            
            OnJoinFailed?.Invoke(joinResponse);
            
            _siraLog.Warn($"Failed to connect to matchmaking pool: {joinResponse.Message}");
        }

        public void Tick()
        {
            if (isActiveAndEnabled)
                UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            if (_matchmakingTimeStopwatch.IsRunning)
                _joinMatchmakingPoolButton.SetButtonText($"Finding Match\n({_matchmakingTimeStopwatch.Elapsed.Minutes:00}:{_matchmakingTimeStopwatch.Elapsed.Seconds % 60:00} elapsed)");
        }
        #endregion
    }
}