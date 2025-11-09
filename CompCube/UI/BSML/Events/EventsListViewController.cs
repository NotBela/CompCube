using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Events;
using CompCube.Interfaces;
using CompCube.UI.BSML.Components;
using HarmonyLib;
using Zenject;

namespace CompCube.UI.BSML.Events;

[ViewDefinition("CompCube.UI.BSML.Events.EventsListView.bsml")]
public class EventsListViewController : BSMLAutomaticViewController
{
    [Inject] private readonly IApi _api = null;
    
    [UIParams] private readonly BSMLParserParams _parserParams = null;
    
    [UIComponent("eventsList")] private readonly CustomCellListTableData _eventsList = null;
    
    public event Action<EventData> OnEventJoinRequested; 
    
    public bool Parsed { get; private set; }
    
    [UIAction("#post-parse")]
    private void PostParse() => Parsed = true;

    public async Task RefreshData()
    {
        _parserParams.EmitEvent("loadingModalShow");
        
        var events = await _api.GetEvents();

        _eventsList.Data = events.Select(i =>
        {
            var slot = new EventSlot(i);
            slot.OnJoinButtonClicked += OnSlotJoinButtonClicked;
            return slot;
        }).ToList();
        _eventsList.TableView.ReloadData();
        
        _parserParams.EmitEvent("loadingModalHide");
    }

    private void OnSlotJoinButtonClicked(EventData data)
    {
        _parserParams.EmitEvent("loadingModalShow");
        
        OnEventJoinRequested?.Invoke(data);
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);

        _eventsList.Data.Cast<EventSlot>().Do(i => i.OnJoinButtonClicked -= OnSlotJoinButtonClicked);
    }
}