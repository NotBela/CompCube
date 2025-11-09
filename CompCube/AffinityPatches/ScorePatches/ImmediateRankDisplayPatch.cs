using CompCube.Game;
using SiraUtil.Affinity;

namespace CompCube.AffinityPatches.ScorePatches
{
    public class ImmediateRankDisplayPatch : IAffinity
    {
        [AffinityPatch(typeof(RelativeScoreAndImmediateRankCounter),
            nameof(RelativeScoreAndImmediateRankCounter.UpdateRelativeScoreAndImmediateRank))]
        [AffinityPrefix]
        private void Prefix(ref int score, ref int modifiedScore, ref int maxPossibleScore,
            ref int maxPossibleModifiedScore)
        {
            modifiedScore = score;
            maxPossibleModifiedScore = maxPossibleScore;
        }
    }
}