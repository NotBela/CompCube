using System.Globalization;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Match;
using CompCube_Models.Models.Packets.ServerPackets;
using JetBrains.Annotations;
using LoungeSaber.Extensions;
using Zenject;

namespace LoungeSaber.UI.BSML.Match
{
    [ViewDefinition("LoungeSaber.UI.BSML.Match.MatchResultsView.bsml")]
    public class MatchResultsViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly IPlatformUserModel _platformUserModel = null;
        
        [CanBeNull] private Action _onContinueButtonPressedCallback = null;
        
        [UIValue("titleBgColor")] private string TitleBgColor { get; set; } = "#0000FF";
        [UIValue("titleText")] private string TitleText { get; set; } = "You Win";
        
        [UIValue("winnerScoreText")] private string WinnerScoreText { get; set; }
        [UIValue("loserScoreText")] private string LoserScoreText { get; set; }
        
        
        [UIValue("mmrChangeText")] private string MmrChangeText { get; set; }
        
        public void PopulateData(MatchResultsPacket results, Action onContinueButtonPressedCallback)
        {
            _onContinueButtonPressedCallback = onContinueButtonPressedCallback;
            
            WinnerScoreText = FormatScore(results.WinnerScore, true);
            LoserScoreText = FormatScore(results.LoserScore, false);

            var won = results.WinnerScore.User.UserId ==
                       _platformUserModel.GetUserInfo(CancellationToken.None).Result.platformUserId;
            
            TitleText = won ? "You Win!" : "You Lose!";
            TitleBgColor = won ? "#0000FF" : "#FF0000";

            MmrChangeText =
                $"You {(won ? "gained" : "lost")}: {results.MmrChange.ToString().FormatWithHtmlColor(won ? "#90EE90" : "#FF7F7F")} MMR";
            
            NotifyPropertyChanged(null);
        }

        private string FormatScore(MatchScore score, bool winner) => 
            $"{(winner ? "1" : "2")}. {score.User.GetFormattedUserName()} - " +
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