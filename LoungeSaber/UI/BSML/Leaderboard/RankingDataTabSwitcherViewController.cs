using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HarmonyLib;
using HMUI;
using LoungeSaber.UI.BSML.Components;
using LoungeSaber.UI.BSML.Info;
using UnityEngine;
using Zenject;

namespace LoungeSaber.UI.BSML.Leaderboard;

[ViewDefinition("LoungeSaber.UI.BSML.Leaderboard.RankingDataTabSwitcherView.bsml")]
public class RankingDataTabSwitcherViewController : BSMLAutomaticViewController
{
    [Inject] private readonly LoungeSaberLeaderboardViewController _loungeSaberLeaderboardViewController = null;
    [Inject] private readonly InfoViewController _infoViewController = null;
    
    [UIComponent("rankingDataTabSelector")]
    private readonly TabSelector _rankingDataTabSelector = null;
    
    [UIValue("rankingDataTabItems")]
    private readonly List<RankingDataTab> _rankingDataTabItems = [];
    
    [UIObject("rankingsTab")]
    private readonly GameObject _rankingsTab = null;
    
    [UIObject("selfTab")]
    private readonly GameObject _selfTab = null;

    [UIAction("#post-parse")]
    void PostParse()
    {
        BSMLParser.Instance.Parse(_loungeSaberLeaderboardViewController.Content, _rankingsTab, _loungeSaberLeaderboardViewController);
        BSMLParser.Instance.Parse(_infoViewController.Content, _selfTab, _infoViewController);
        
        _rankingDataTabSelector.TextSegmentedControl.ReloadData();
        _rankingDataTabSelector.Refresh();
    }
}