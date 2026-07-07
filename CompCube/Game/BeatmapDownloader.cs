using System.IO.Compression;
using BeatSaverSharp;
using CompCube.Interfaces;
using IPA.Utilities;
using SiraUtil.Logging;
using SongCore;
using Zenject;

namespace CompCube.Game;

public class BeatmapDownloader
{
    [Inject] private readonly SiraLog _siraLog = null!;
    [Inject] private readonly IApi _api = null!;
    
    public event Action<int, int>? OnMapDownloaded;

    public async Task DownloadMaps(string[] mapHashes)
    {
        var mapsDownloaded = 0;

        var hashesWithoutDuplicates = mapHashes.Distinct().ToArray();
        
        foreach (var mapHash in hashesWithoutDuplicates)
        {
            if (Collections.songWithHashPresent(mapHash))
                continue;

            _siraLog.Notice($"Attempting to download {mapHash}...");
            
            var beatmapData = await _api.DownloadBeatmap(mapHash);

            if (beatmapData == null)
                throw new Exception($"Failed to download {mapHash}!");
            
            var zippedBeatmap = new ZipArchive(new MemoryStream(beatmapData ?? throw new Exception("Beatmap data is null!")), ZipArchiveMode.Read);
            zippedBeatmap.ExtractToDirectory(Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels", mapHash));
            
            mapsDownloaded++;
            OnMapDownloaded?.Invoke(mapsDownloaded, mapHashes.Length);
        }
        
        Loader.Instance.RefreshSongs();
    }
}