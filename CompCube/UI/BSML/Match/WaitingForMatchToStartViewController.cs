using System.Collections;
using System.Globalization;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Map;
using CompCube.Configuration;
using CompCube.UI.BSML.Components.CustomLevelBar;
using HMUI;
using CompCube.Extensions;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Match
{
    [ViewDefinition("CompCube.UI.BSML.Match.WaitingForMatchToStartView.bsml")]
    public class WaitingForMatchToStartViewController : BSMLAutomaticViewController, ITickable
    {
        [Inject] private readonly PluginConfig _config = null!;
        
        [UIValue("matchStartTimer")] private string MatchStartTimer { get; set; } = "";

        [UIValue("scoreSubmission")]
        private bool ScoreSubmission
        {
            get => _config.ScoreSubmission;
            set => _config.ScoreSubmission = value;
        }
        
        
        [UIComponent("difficultySegmentData")] private readonly TextSegmentedControl _difficultySegmentData = null!;
        [UIComponent("categorySegmentData")] private readonly TextSegmentedControl _categorySegmentData = null!;

        private CustomLevelBar? _customLevelBar = null;
         
        private DateTime? _startTime;

        [UIAction("#post-parse")]
        private void PostParse()
        {
            _customLevelBar ??= Resources.FindObjectsOfTypeAll<CustomLevelBar>()
                .First(i => i.name == "WaitingForMatchStartLevelBar");
        }
        
        [UIAction("nothing")]
        private void Nothing(SegmentedControl _, int cell){}
        
        public async Task PopulateData(VotingMap votingMap, DateTime? startTime)
        {
            // awful
            while (_customLevelBar is null)
                await Task.Delay(25);
            
            _startTime = startTime;
            
            _customLevelBar?.Setup(votingMap);

            if (startTime == null)
            {
                MatchStartTimer = "Starting soon. Please wait!";
            }

            StartCoroutine(UpdateTexts());

            return;

            IEnumerator UpdateTexts()
            {
                yield return new WaitForEndOfFrame();
                
                _difficultySegmentData.SetTexts([votingMap.GetBaseGameDifficultyType().Name()]);
                _categorySegmentData.SetTexts(["Category: " + votingMap.Category]);
            }
        }
        
        

        public void Tick()
        {
            if (!isActivated)
                return;

            if (_startTime == null)
                return;
            
            MatchStartTimer = $"Starting in {((int) (_startTime.Value - DateTime.UtcNow).TotalSeconds).ToString(CultureInfo.InvariantCulture)}...";
            
            NotifyPropertyChanged(nameof(MatchStartTimer));
        }
    }
}