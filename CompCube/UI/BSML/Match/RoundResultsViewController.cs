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
        
        [UIValue("titleBgColor")] private string TitleBgColor { get; set; } = "#0000FF";
        [UIValue("titleText")] private string TitleText { get; set; } = String.Empty;
        
        [UIValue("winnerScoreText")] private string WinnerScoreText { get; set; }
        [UIValue("loserScoreText")] private string LoserScoreText { get; set; }
        
        public void PopulateData(RoundResultsPacket results)
        {
            var scores = results.Scores.OrderByDescending(i => i.Value.Points)
                .Select(i => new MatchScore(_matchStateManager.Players.First(j => j.Key.UserId == i.Key).Key, i.Value)).ToArray();

            TitleText = $"{scores[0].User.Username} wins!";
            
            WinnerScoreText = FormatScore(scores[0], 1);
            LoserScoreText = FormatScore(scores[1], 2);
            
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