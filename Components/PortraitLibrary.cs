using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class PortraitLibrary : Resource
{
    [Export] public Godot.Collections.Array<PortraitEntry> Entries { get; set; } = new();

    private Dictionary<string, Texture2D> _portraitMap;

    private void BuildPortraitMap()
    {
        _portraitMap = new Dictionary<string, Texture2D>();

        foreach (var entry in Entries)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.Name) && entry.Portrait != null)
                _portraitMap[entry.Name] = entry.Portrait;
        }
    }

    public Texture2D GetPortrait(string name)
    {
        if (_portraitMap == null)
            BuildPortraitMap();

        if (_portraitMap.TryGetValue(name, out var tex))
            return tex;

        GD.PrintErr($"ERROR: PortraitLibrary - No portrait found for '{name}'");
        return null;
    }
}
