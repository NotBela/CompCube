using System.IO.Compression;
using BeatSaverSharp;
using IPA.Utilities;
using SiraUtil.Logging;
using SongCore;
using Zenject;

namespace CompCube.Game;

public class BeatmapDownloader
{
    [Inject] private readonly SiraLog _siraLog = null!;
    
    private readonly BeatSaver _beatSaver = new("CompCube", Version.Parse(IPA.Loader.PluginManager.GetPlugin("CompCube").HVersion.ToString()));
    
    public event Action<int, int>? OnMapDownloaded;

    public async Task DownloadMaps(string[] mapHashes)
    {
        var mapsDownloaded = 0;

        var hashesWithoutDuplicates = mapHashes.Distinct().ToArray();
        
        foreach (var mapHash in hashesWithoutDuplicates)
        {
            if (SongCore.Collections.songWithHashPresent(mapHash))
                continue;
            
            var map = await _beatSaver.BeatmapByHash(mapHash);
            if (map == null)
            {
                _siraLog.Error($"Could not find map {mapHash}!");
                continue;
            }
            
            var version =
                map.Versions.FirstOrDefault(i => string.Equals(i.Hash, mapHash, StringComparison.OrdinalIgnoreCase));

            if (version == null)
            {
                _siraLog.Error("Map not found in beatsaver versions!");
                continue;
            }

            _siraLog.Notice($"Downloading...");
            var beatmapData = await version.DownloadZIP();
            
            var zippedBeatmap = new ZipArchive(new MemoryStream(beatmapData ?? throw new Exception("Beatmap data is null!")), ZipArchiveMode.Read);
            zippedBeatmap.ExtractToDirectory(Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels", Path.GetInvalidFileNameChars().Aggregate($"{map.Name} ({map.ID})", (current, c) => current.Replace(c.ToString(), string.Empty))));
            
            mapsDownloaded++;
            OnMapDownloaded?.Invoke(mapsDownloaded, mapHashes.Length);
        }
        
        Loader.Instance.RefreshSongs();
    }
}