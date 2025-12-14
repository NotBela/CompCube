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
    public class MatchmakingMenuViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [Inject] private readonly PluginConfig _config = null!;
        [Inject] private readonly IServerListener _serverListener = null!;
        [Inject] private readonly SiraLog _siraLog = null!;

        [UIParams] private readonly BSMLParserParams _parserParams = null!;

        public event Action? AboutButtonClicked;

        public event Action? EventsButtonClicked;

        private bool _isQueued = false;

        [UIValue("is-queued")]
        private bool IsInMatchmakingQueue
        {
            get => _isQueued;
            set
            {
                _isQueued = value;
                NotifyPropertyChanged(null);
            }
        }

        [UIValue("is-not-queued")]
        private bool IsNotInMatchmakingQueue => !IsInMatchmakingQueue;

        [UIValue("failedToConnectReason")] private string FailedToConnectReason { get; set; } = "";

        [UIAction("joinMatchmakingPoolButtonOnClick")]
        private void HandleJoinMatchmakingPoolClicked()
        {
            IsInMatchmakingQueue = true;
            
            _serverListener.Connect("standard", (response) =>
            {
                if (response.Successful) 
                    return;
                
                _parserParams.EmitEvent("failedToConnectModalShow");
                FailedToConnectReason = $"Reason: {response.Message}";
                NotifyPropertyChanged(nameof(FailedToConnectReason));
            });
        }

        [UIAction("leaveMatchmakingPoolButtonOnClick")]
        private void HandleLeaveMatchmakingPoolButtonClicked() => _parserParams.EmitEvent("disconnectModalShowEvent");
        
        [UIAction("leaveMatchmakingPoolDenyButtonOnClick")] 
        private void HandleLeaveMatchmakingPoolDenied() => _parserParams.EmitEvent("disconnectModalHideEvent");

        [UIAction("leaveMatchmakingPoolAllowButtonOnClick")]
        private void LeaveMatchmakingPoolAllowButton()
        {
            _parserParams.EmitEvent("disconnectModalHideEvent");
            _serverListener.Disconnect();
        }

        [UIAction("failedToConnectModalOkButtonOnClick")]
        private void HandleFailedToConnectModalOkButtonClicked() => _parserParams.EmitEvent("failedToConnectModalHide");

        public void Initialize()
        {
            _serverListener.OnDisconnected += HandleDisconnected;
        }

        private void HandleDisconnected() => IsInMatchmakingQueue = false;

        public void Dispose()
        {
            _serverListener.OnDisconnected -= HandleDisconnected;
        }
    }
}