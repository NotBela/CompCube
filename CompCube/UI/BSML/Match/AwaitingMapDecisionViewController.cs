using System.Collections;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Map;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube.Interfaces;
using CompCube.UI.BSML.Components.CustomLevelBar;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Match
{
    [ViewDefinition("CompCube.UI.BSML.Match.AwaitingMapDecisionView.bsml")]
    public class AwaitingMapDecisionViewController : BSMLAutomaticViewController
    {
        [Inject] private readonly IServerListener _serverListener = null!;
        [Inject] private readonly SiraLog _siraLog = null!;
        
        private List<VotingMap> _votingMaps = [];

        private CustomLevelBar _ownLevelBar;
        private CustomLevelBar _opponentLevelBar;
        
        [UIAction("#post-parse")]
        private void PostParse()
        {
            var allLevelBars = Resources.FindObjectsOfTypeAll<CustomLevelBar>();
            
            _opponentLevelBar =
                allLevelBars.First(i => i.name == "OpponentVoteBar");
            _ownLevelBar = allLevelBars.First(i => i.name == "OwnVoteBar");
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            _opponentLevelBar.SetWaiting();
        }
        
        public void PopulateData(VotingMap vote, List<VotingMap> votingMaps)
        {
            // _opponentLevelBar.SetWaiting();
            
            _votingMaps = votingMaps;
            
            _ownLevelBar.Setup(vote);
        }

        public void PopulateOpponentVote(PlayerVotedPacket opponentVoted)
        {
            _opponentLevelBar.Setup(opponentVoted.Vote);
        }
    }
}