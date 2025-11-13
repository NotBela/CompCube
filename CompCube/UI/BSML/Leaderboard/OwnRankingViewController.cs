using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube.Extensions;
using CompCube.Interfaces;
using Zenject;

namespace CompCube.UI.BSML.Leaderboard;

[ViewDefinition("CompCube.UI.BSML.Leaderboard.OwnRankingDataView.bsml")]
public class OwnRankingViewController : BSMLAutomaticViewController, Interfaces.IRefreshable
{
    [Inject] private readonly IApi _api = null!;
    [Inject] private readonly IPlatformUserModel _platformUserModel = null!;
    
    [UIValue("nameText")] private string NameText { get; set; } = "";
    [UIValue("mmrText")] private string MmrText { get; set; } = "";
    [UIValue("rankText")] private string RankText { get; set; } = "";
    [UIValue("winRateText")] private string WinRateText { get; set; } = "";
    [UIValue("winstreakText")] private string WinStreakText { get; set; } = "";

    private bool _loading;

    [UIValue("loading")]
    private bool Loading
    {
        get => _loading;
        set
        {
            _loading = value;
            NotifyPropertyChanged();
        }
    }
    
    [UIValue("notLoading")] private bool NotLoading => !Loading;

    private async Task UpdateDataAsync()
    {
        Loading = true;

        var selfData = await _api.GetUserInfo((await _platformUserModel.GetUserInfo(CancellationToken.None)).platformUserId);

        if (selfData == null)
        {
            return;
        }

        NameText = selfData.GetFormattedUserName();
        MmrText = $"MMR: {selfData.Mmr} ({selfData.Division.GetFormattedDivision()})";
        WinRateText = $"Win rate: {selfData.Wins}/{selfData.Wins + selfData.Losses} ({(float) selfData.Wins / selfData.Wins + selfData.Losses:F}%)";
        WinStreakText = $"Winstreak: {selfData.Winstreak} (Highest: {selfData.HighestWinstreak})";

        Loading = false;
    }

    public void UpdateData() => UpdateDataAsync();

    [UIAction("#post-parse")]
    private void PostParse()
    {
        UpdateData();
    }

    public void Refresh()
    {
        
    }
}