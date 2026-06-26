using System.Collections;
using System.Globalization;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Match;
using CompCube_Models.Models.Packets.ServerPackets;
using JetBrains.Annotations;
using CompCube.Extensions;
using CompCube.Game;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Match
{
    [ViewDefinition("CompCube.UI.BSML.Match.RoundResultsView.bsml")]
    public class RoundResultsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly MatchStateManager _matchStateManager = null!;
        
        private Action? _onContinueButtonPressedCallback = null;
        
        [UIValue("redPlayerName")] private string RedPlayerName { get; set; } = "";
        [UIValue("bluePlayerName")] private string BluePlayerName { get; set; } = "";
        
        
        public void PopulateData(RoundResultsPacket results)
        {
            RedPlayerName = _matchStateManager.RedPlayer.GetFormattedUserName();
            BluePlayerName = _matchStateManager.BluePlayer.GetFormattedUserName();
            
            NotifyPropertyChanged(null);
        }

        public void SetContinueButtonCallback(Action? onContinueButtonPressedCallback)
        {
            _onContinueButtonPressedCallback = onContinueButtonPressedCallback;
        }

        private string FormatScore(MatchScore score, int placement) => 
            $"{(placement)}. {score.User.GetFormattedUserName()} - " +
            $"{(score.Score?.RelativeScore * 100):F}% " +
            $"{(score.Score.FullCombo ? "FC".FormatWithHtmlColor("#90EE90") : $"{score.Score.Misses}x".FormatWithHtmlColor("#FF7F7F"))}" +
            $"{(score.Score.ProMode ? " (PM)" : "")}";

        [UIAction("continueButtonClicked")]
        private void ContinueButtonClicked()
        {
            _onContinueButtonPressedCallback?.Invoke();
            _onContinueButtonPressedCallback = null;
        }
    }
}