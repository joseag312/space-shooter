using Godot;
using System.Collections.Generic;

public static class LevelCatalog
{
    private static Dictionary<string, LevelDataResource> _levels;

    public static void LoadAll()
    {
        _levels = new Dictionary<string, LevelDataResource>();

        var dir = DirAccess.Open("res://resources/levels/");
        if (dir == null)
        {
            GD.PrintErr("ERROR: LevelCatalog - Could not open levels/data directory");
            return;
        }

        dir.ListDirBegin();
        string fileName;
        while ((fileName = dir.GetNext()) != "")
        {
            string fullPath = $"res://resources/levels/{fileName}";

            var level = GD.Load<LevelDataResource>(fullPath);

            if (level == null)
            {
                GD.PrintErr($"ERROR: Failed to load level resource: {fullPath}");
                continue;
            }

            if (string.IsNullOrEmpty(level.Key))
            {
                GD.PrintErr($"ERROR: Level resource missing Key: {fullPath}");
                continue;
            }

            _levels[level.Key] = level;
        }
        dir.ListDirEnd();
    }


    public static LevelDataResource Get(string levelId)
    {
        if (_levels == null)
        {
            GD.PrintErr("ERROR: LevelCatalog - LoadAll() was not called before Get()");
            return null;
        }

        return _levels.TryGetValue(levelId, out var level) ? level : null;
    }

    public static IEnumerable<LevelDataResource> GetAll() => _levels?.Values;
}
