using Godot;
using System;

[GlobalClass]
public partial class LevelMusicTriggerComponent : Node
{
    [Export] public string TrackName = "level1";

    public override void _Ready()
    {
        G.MS.PlayTrack(TrackName);
    }
}
