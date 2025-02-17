
using assert;
namespace UtilityIO;

public static class Constants
{
    // parent directory that contains object lib/kcg map/ kcg data, etc
    private static string _kcgRootDirectory;
    public static string KcgRootDirectory
    {
        get
        {
            Utils.Assert(_kcgRootDirectory != null, "Root directory not set!");
            return _kcgRootDirectory;
        }
        private set
        {
            Utils.Assert(!IsRootDirectorySet, "Absolute path already set!");
            Utils.Assert(!PathUtils.IsAbsolutePath($"Only absolute paths are accepted. provided path: {value}"));
            _kcgRootDirectory = PathUtils.ReplaceBackSlashesWithForwardSlashes(value);
        }
    }
    
    private static readonly object _lock = new object();
    
    // current project being used directory path. kcg-lib or kcg-map ir kcg-game
    // May be a temporary solution but it allow getting cash data
    private static string _projectDirectory;
    public static string ProjectDirectory
    {
        get
        {
            Utils.Assert(IsRootDirectorySet, "Root directory not set!");
            return _projectDirectory;
        }
        set
        {
            lock (_lock)
            {
                if (!IsRootDirectorySet)
                {
                    _projectDirectory = value;
                    KcgRootDirectory = PathUtils.GetParentPath(_projectDirectory);
                }
            }
        }
    }


    public static bool IsRootDirectorySet => _kcgRootDirectory != null;
    
    public const string CacheFolderDirectoryTilesets = "kcg-data/tilesets_v01";
    public const string CacheFolderDirectoryRegions = "kcg-data/regions_v01";
    public const string CacheFolderDirectoryMechs = "kcg-data/mechs_v01";
    public const string CacheFolderDirectoryMaps = "kcg-data/maps_v01";
    
    // TODO: delete
    public const string CacheFolderName = "kcg-data";
    // TODO: delete
    public const string TilesetFileName = "tileset.json";
    // TODO: delete
    public const string ManifestFileName = "tileset-manifest.json";
    public const string MapsFolderName = "/assets/maps/";
    
    public const string PlanetGridImageDumpFolder = "output/planetGridImages";
    public const string PlanetGridImageFileName = "outputPlanetGridImg";

    public const string ChunkPreviewImageDumpFolder = "output/ChunkPreviewImages";
    public const string ChunkPreviewImageFileName = "outputChunkPreviewImg";
}