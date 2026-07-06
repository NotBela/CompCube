using System.Collections;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Map;
using CompCube_Models.Models.Packets.ServerPackets;
using HMUI;
using JetBrains.Annotations;
using CompCube.Extensions;
using CompCube.Game;
using CompCube.Game.MatchState;
using SiraUtil.Logging;
using TMPro;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Match;

[ViewDefinition("CompCube.UI.BSML.Match.VotingScreenView.bsml")]
public class VotingScreenViewController : BSMLAutomaticViewController
{
    [Inject] private readonly SiraLog _log = null!;
    [Inject] private readonly MatchStateManager _matchStateManager = null!;
    
    public event Action<VotingMap>? MapSelected;
    private Action? _ranOutOfTimeCallback = null;

    [UIComponent("mapList")] private readonly CustomListTableData _mapListTableData = null!;
    private VotingListDataSource _votingListDataSource = null!;

    [UIComponent("voteStatusText")] private readonly TextMeshProUGUI _voteStatusText = null!;
    
    private Action? _activationCallback = null;
    
    private Action? _skipButtonPressedCallback = null;
    
    [UIValue("showSkipButton")] private bool ShowSkipButton => _skipButtonPressedCallback != null;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        
        _activationCallback?.Invoke();
        _activationCallback = null;
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        
        StopCountdown();
    }

    [UIAction("#post-parse")]
    void PostParse()
    {
        _votingListDataSource = gameObject.AddComponent<VotingListDataSource>();
        _mapListTableData.TableView.SetDataSource(_votingListDataSource, true);
        
        _votingListDataSource.Init(_mapListTableData.TableView);
        
        Destroy(_mapListTableData);
        
        _votingListDataSource.TableView.didSelectCellWithIdxEvent += DidSelectCellWithIdxEvent;
    }

    private void DidSelectCellWithIdxEvent(TableView tableView, int idx)
    {
        MapSelected?.Invoke(_votingListDataSource.Data[idx]);
    }

    public void ClearSelection()
    {
        _mapListTableData.TableView.ClearSelection();
    }
    public void RemoveMapFromList(VotingMap map)
    {
        _votingListDataSource.Data.Remove(map);
        _votingListDataSource.TableView.ReloadData();
    }

    public void StopCountdown() => _ranOutOfTimeCallback = null;

    [UIAction("skipButtonClicked")]
    private void SkipButtonClicked()
    {
        _skipButtonPressedCallback?.Invoke();
        _skipButtonPressedCallback = null;
    }

    public void PopulateData(VotingMap[] maps, int waitTime, Action? skipButtonPressedCallback = null, Action? timerRanOutCallback = null)
    {
        _log.Notice("Populating maps");
        StartCoroutine(PopulateDataCoroutine());
        
        _ranOutOfTimeCallback = timerRanOutCallback;
        _skipButtonPressedCallback = skipButtonPressedCallback;
        
        NotifyPropertyChanged(nameof(ShowSkipButton));
        
        return;
        
        IEnumerator PopulateDataCoroutine()
        {
            yield return new WaitForEndOfFrame();
            
            _votingListDataSource.SetData(maps.ToList());
            _votingListDataSource.TableView.ClearSelection();

            yield return CountDown();
        }

        IEnumerator CountDown()
        {
            var countdownFinishTime = DateTime.Now.AddSeconds(waitTime);
            
            _log.Notice("Counting down");
            while (true)
            {
                if (_ranOutOfTimeCallback == null)
                    yield return null;
                
                var remaining = countdownFinishTime - DateTime.Now;
                if (remaining.TotalSeconds <= 0)
                    break;

                _voteStatusText.text =
                    $"Discard Phase\nDiscard up to two maps that you don't want to play!\nTime left: {Mathf.CeilToInt((float)remaining.TotalSeconds)}";

                yield return null;
            }
            _ranOutOfTimeCallback?.Invoke();
            _ranOutOfTimeCallback = null;
        }
    }
}
    
public class VotingListDataSource : MonoBehaviour, TableView.IDataSource
{
    public TableView TableView { get; private set; }
        
    public List<VotingMap> Data { get; private set; } = new();

    private LevelListTableCell _tableCellPrefab;

    private LevelListTableCell CreateTableCellPrefab()
    {
        var gameObj = Instantiate(
            Resources.FindObjectsOfTypeAll<LevelCollectionViewController>()
                .First()
                .transform
                .Find("LevelsTableView/TableView/Viewport/Content/LevelListTableCell")
                .gameObject);
            
        gameObj.name = "MyListCell";

        var cell = gameObj.GetComponent<LevelListTableCell>();
        return cell;
    }
        
    public void Init(TableView tableView) => TableView = tableView;

    public void SetData(List<VotingMap> maps)
    {
        Data = maps;
        TableView.ReloadData();
    }

    public float CellSize(int idx) => 8.5f;

    public int NumberOfCells() => Data.Count;

    public TableCell CellForIdx(TableView tableView, int idx)
    {
        var cell = (LevelListTableCell) tableView.DequeueReusableCellForIdentifier("VotingListTableCell");

        if (cell is null)
        {
            _tableCellPrefab ??= CreateTableCellPrefab();
            cell = Instantiate(_tableCellPrefab);
            cell.reuseIdentifier = "VotingListTableCell";
        }

        var info = Data[idx];
        cell.SetDataFromLevelAsync(info.GetBeatmapLevel(), false,false, false, true);

        return cell;
    }
}