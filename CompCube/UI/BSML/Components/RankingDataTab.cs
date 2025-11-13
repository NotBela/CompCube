using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;

namespace CompCube.UI.BSML.Components;

public class RankingDataTab
{
    private readonly BSMLAutomaticViewController _host;
    
    public RankingDataTab(BSMLAutomaticViewController host, GameObject parent)
    {
        _host = host;
        
        BSMLParser.Instance.Parse(host.Content, parent, host);
    }

    public void Refresh()
    {
        if (_host is not Interfaces.IRefreshable refreshableHost)
            return;
        
        refreshableHost.Refresh();
    }
}