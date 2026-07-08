using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Map;
using CompCube.UI.BSML.Components.CustomMapList;

namespace CompCube.UI.BSML.Match;

[ViewDefinition("CompCube.UI.BSML.Match.WaitingForDiscardPhaseToFinishView.bsml")]
public class WaitingForDiscardPhaseToFinishViewController : BSMLAutomaticViewController
{
    private CustomMapListController _customMapListController;
    
    [UIAction("#post-parse")]
    private void PostParse()
    {
        _customMapListController = CustomMapListController.ParseOntoViewController(this, null, 0f, 10f, false);
    }

    public void PopulateData(VotingMap[] votingMaps)
    {
        _customMapListController.SetMaps(votingMaps);
    }
}