using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CompCube_Models.Models.Map;
using CompCube_Models.Models.Packets.ServerPackets;
using CompCube.Interfaces;
using CompCube.UI.BSML.Components.CustomLevelBar;
using UnityEngine;
using Zenject;

namespace CompCube.UI.BSML.Match
{
    [ViewDefinition("CompCube.UI.BSML.Match.AwaitingMapDecisionView.bsml")]
    public class AwaitingMapDecisionViewController : BSMLAutomaticViewController, IInitializable, IDisposable
    {
        [Inject] private readonly IServerListener _serverListener = null;
        
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
        
        public void PopulateData(VotingMap vote, List<VotingMap> votingMaps)
        {
            _opponentLevelBar.SetWaiting();
            
            _votingMaps = votingMaps;
            
            _ownLevelBar.Setup(vote);
        }

        private void OnOpponentVoted(OpponentVotedPacket opponentVoted)
        {
            while (_votingMaps.Count == 0);
            
            _opponentLevelBar.Setup(_votingMaps[opponentVoted.VoteIndex]);
        }

        public void Dispose()
        {
            _serverListener.OnOpponentVoted -= OnOpponentVoted;
        }

        public void Initialize()
        {
            _serverListener.OnOpponentVoted += OnOpponentVoted;
        }
    }
}