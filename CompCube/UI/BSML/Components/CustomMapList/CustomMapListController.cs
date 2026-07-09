using System.Collections;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Map;
using CompCube.Extensions;
using HMUI;
using SongCore;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Components.CustomMapList;

[ViewDefinition("CompCube.UI.BSML.Components.CustomMapList.CustomMapList.bsml")]
public class CustomMapListController : BSMLAutomaticViewController
{
    [Inject] private readonly SharedCoroutineStarter _sharedCoroutineStarter = null!;
    
    [UIComponent("mapList")] private readonly CustomListTableData _mapListTableData = null!;
    private VotingListDataSource _votingListDataSource = null!;

    private Action<VotingMap>? _handleMapSelectedCallback;
    
    [UIValue("anchorPosY")] private float AnchorPosY { get; set; }
    [UIValue("anchorPosX")] private float AnchorPosX { get; set; }

    public bool Interactable { get; set; } = true;

    public VotingMap[] MapsInList => _votingListDataSource.Data.ToArray();

    public static CustomMapListController ParseOntoViewController(ViewController viewController, Action<VotingMap>? handleMapSelectedCallback, float xPos = 0f, float yPos = 0f, bool interactable = true)
    {
        var controller = BeatSaberUI.CreateViewController<CustomMapListController>();
        
        BSMLParser.Instance.Parse(
            Utilities.GetResourceContent(Assembly.GetExecutingAssembly(),
                "CompCube.UI.BSML.Components.CustomMapList.CustomMapList.bsml"), viewController.gameObject, controller);

        controller.SetPos(xPos, yPos);
        controller._handleMapSelectedCallback = handleMapSelectedCallback;
        
        controller.Interactable = interactable;
        
        return controller;
    }

    private void SetPos(float x, float y)
    {
        AnchorPosX = x;
        AnchorPosY = y;
        
        NotifyPropertyChanged(null);
    }
    
    public void SetMaps(VotingMap[] maps)
    {
        _sharedCoroutineStarter.Run(PopulateDataCoroutine());
        return;
        
        IEnumerator PopulateDataCoroutine()
        {
            yield return new WaitForEndOfFrame();

            _votingListDataSource.SetData(maps.ToList());
            _votingListDataSource.TableView.ClearSelection();
        }
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
        if (!Interactable)
        {
            ClearSelection();
            return;
        }
        
        _handleMapSelectedCallback?.Invoke(_votingListDataSource.Data[idx]);
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
        
        if (info.GetBeatmapLevel() == null)
            Plugin.Log.Info($"null");
        cell.SetDataFromLevelAsync(info.GetBeatmapLevel(), false,false, false, true);

        return cell;
    }
}