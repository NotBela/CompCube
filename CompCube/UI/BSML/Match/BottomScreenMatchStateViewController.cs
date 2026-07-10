using System.Collections;
using System.Globalization;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube.Extensions;
using CompCube.UI.BSML.Components.EnergyBar;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Match;

[ViewDefinition("CompCube.UI.BSML.Match.BottomScreenMatchStateView.bsml")]
public class BottomScreenMatchStateViewController : BSMLAutomaticViewController
{
    [Inject] private readonly SharedCoroutineStarter _sharedCoroutineStarter = null!;
    
    private CustomEnergyBar _redEnergyBar;
    private CustomEnergyBar _blueEnergyBar;
    
    [UIValue("roundText")] private string RoundText { get; set; }
    [UIValue("redText")] private string RedText { get; set; }
    [UIValue("blueText")] private string BlueText { get; set; }
    
    [UIValue("multiplierText")] private string MultiplierText { get; set; }

    [UIAction("#post-parse")]
    void PostParse()
    {
        _redEnergyBar = CustomEnergyBar.ParseOntoViewController(this, new Vector3(-10f, -25f), false, new Color(1f, .4549f, .4235f));
        _blueEnergyBar = CustomEnergyBar.ParseOntoViewController(this, new Vector3(10f, -25f), true, new Color(.565f, .835f, 1f));
    }
    
    public void PopulateData(CompCube_Models.Models.ClientData.UserInfo red, CompCube_Models.Models.ClientData.UserInfo blue)
    {
        // RedText = red.GetFormattedUserName();
        // BlueText = blue.GetFormattedUserName();
        
        NotifyPropertyChanged(null);
    }

    public void UpdateHealth(int redHealth, int blueHealth)
    {
        _sharedCoroutineStarter.Run(UpdateHealthCoroutine());
        return;
        
        IEnumerator UpdateHealthCoroutine()
        {
            yield return new WaitUntil(() => _redEnergyBar != null);
            
            RedText = redHealth.ToString("N0", CultureInfo.InvariantCulture);
            BlueText = blueHealth.ToString("N0", CultureInfo.InvariantCulture);
        
            _redEnergyBar.SetEnergy((float) redHealth / 1000000);
            _blueEnergyBar.SetEnergy((float) blueHealth / 1000000);
        
            NotifyPropertyChanged(null);
        }
    }

    public void UpdateRound(int round)
    {
        RoundText = $"Round {round}";
        NotifyPropertyChanged(nameof(RoundText));
    }

    public void UpdateMultiplier(float multiplier)
    {
        MultiplierText = $"{multiplier:F1}x";
        NotifyPropertyChanged(nameof(MultiplierText));
    }

    public void SetStatus(string status)
    {
        RoundText = status;
        NotifyPropertyChanged(nameof(RoundText));
    }
}