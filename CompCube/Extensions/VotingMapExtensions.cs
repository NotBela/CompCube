using CompCube_Models.Models.Map;
using JetBrains.Annotations;
using SongCore;

namespace CompCube.Extensions;

public static class VotingMapExtensions
{
    public static IBeatmapLevel? GetBeatmapLevel(this VotingMap votingMap)
    {
        return Loader.GetLevelByHash(votingMap.Hash) as IBeatmapLevel;
    }

    public static BeatmapDifficulty GetBaseGameDifficultyType(this VotingMap votingMap) => votingMap.Difficulty switch
    {
        VotingMap.DifficultyType.Easy => BeatmapDifficulty.Easy,
        VotingMap.DifficultyType.Normal => BeatmapDifficulty.Normal,
        VotingMap.DifficultyType.Hard => BeatmapDifficulty.Hard,
        VotingMap.DifficultyType.Expert => BeatmapDifficulty.Expert,
        VotingMap.DifficultyType.ExpertPlus => BeatmapDifficulty.ExpertPlus,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public static BeatmapDifficultyMask GetBaseGameDifficultyTypeMask(this VotingMap votingMap) => votingMap.GetBaseGameDifficultyType() switch
    {
        BeatmapDifficulty.Easy => BeatmapDifficultyMask.Easy,
        BeatmapDifficulty.Normal => BeatmapDifficultyMask.Normal,
        BeatmapDifficulty.Hard => BeatmapDifficultyMask.Hard,
        BeatmapDifficulty.Expert => BeatmapDifficultyMask.Expert,
        BeatmapDifficulty.ExpertPlus => BeatmapDifficultyMask.ExpertPlus,
        _ => BeatmapDifficultyMask.All
    };

    public static IDifficultyBeatmap? GetBeatmapKey(this VotingMap votingMap) => votingMap.GetBeatmapLevel()
        ?.beatmapLevelData.difficultyBeatmapSets.First(i => i.beatmapCharacteristic.serializedName == "Standard")
        .difficultyBeatmaps.First(i => i.difficulty == votingMap.GetBaseGameDifficultyType());
}