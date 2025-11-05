using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;

namespace LoungeSaber.UI.BSML.Components;

public class RankingDataTab
{
    [UIValue("tab-name")]
    private string _tabName { set; get; }

    [UIObject("root-tab")]
    private readonly GameObject _tabObject = null;

    private readonly object _host;

    private readonly string _resource;
    
    public RankingDataTab(string name, string resource, object host)
    {
        _tabName = name;
        _host = host;
        _resource = resource;
    }

    [UIAction("#post-parse")]
    private void PostParse()
    {
        BSMLParser.Instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), _resource), _tabObject, _host);
    }
}