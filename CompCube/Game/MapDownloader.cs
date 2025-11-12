using System.IO.Compression;
using BeatSaverSharp;
using IPA.Utilities;
using SiraUtil.Logging;
using SongCore;
using Zenject;

namespace CompCube.Game;

public class MapDownloader
{
    [Inject] private readonly SiraLog _siraLog = null!;
    
    private readonly BeatSaver _beatSaver = new("CompCube", Version.Parse(IPA.Loader.PluginManager.GetPlugin("CompCube").HVersion.ToString()));
    
    public event Action<int, int>? OnMapDownloaded;

    public async Task DownloadMaps(string[] mapHashes)
    {
        var mapsDownloaded = 0;
        
        foreach (var mapHash in mapHashes)
        {
            if (Loader.GetLevelByHash(mapHash) != null)
                continue;
            
            var map = await _beatSaver.BeatmapByHash(mapHash);
            if (map == null)
            {
                _siraLog.Error($"Could not find map {mapHash}!");
                continue;
            }

            var beatmapData = await map.LatestVersion.DownloadZIP();
            
            var zippedBeatmap = new ZipArchive(new MemoryStream(beatmapData ?? throw new Exception("Beatmap data is null!")), ZipArchiveMode.Read);
            zippedBeatmap.ExtractToDirectory(Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels", Path.GetInvalidFileNameChars().Aggregate($"{map.Name} ({map.ID})", (current, c) => current.Replace(c.ToString(), string.Empty))));
            
            mapsDownloaded++;
            OnMapDownloaded?.Invoke(mapsDownloaded, mapHashes.Length);
        }
    }
}