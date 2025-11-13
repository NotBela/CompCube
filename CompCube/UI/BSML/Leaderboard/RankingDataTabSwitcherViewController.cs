using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube.UI.BSML.Components;
using CompCube.UI.BSML.Info;
using HarmonyLib;
using HMUI;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Leaderboard;

[ViewDefinition("CompCube.UI.BSML.Leaderboard.RankingDataTabSwitcherView.bsml")]
public class RankingDataTabSwitcherViewController : BSMLAutomaticViewController
{
    [Inject] private readonly LeaderboardViewController _leaderboardViewController = null!;
    [Inject] private readonly OwnRankingViewController _ownRankingViewController = null!;
    
    [UIComponent("rankingDataTabSelector")]
    private readonly TabSelector _rankingDataTabSelector = null!;
    
    [UIValue("rankingDataTabItems")]
    private readonly List<RankingDataTab> _rankingDataTabItems = [];
    
    [UIObject("rankingsTab")]
    private readonly GameObject _rankingsTab = null!;
    
    [UIObject("selfTab")]
    private readonly GameObject _selfTab = null!;

    [UIAction("#post-parse")]
    void PostParse()
    {
        _rankingDataTabItems.Add(new RankingDataTab("Ranking", _ownRankingViewController.ContentFilePath, _ownRankingViewController));
        _rankingDataTabItems.Add(new RankingDataTab("Leaderboard", _leaderboardViewController.ContentFilePath, _leaderboardViewController));
        
        _rankingDataTabSelector.TextSegmentedControl.ReloadData();
        _rankingDataTabSelector.Refresh();
    }

    [UIAction("onCellSelected")]
    private void OnCellSelected(object _, int index)
    {
        var cell = _rankingDataTabItems[index];
        cell.Host.Refresh();
    }
}