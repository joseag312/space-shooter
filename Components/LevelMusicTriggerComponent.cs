using Godot;
using System;

[GlobalClass]
public partial class LevelMusicTriggerComponent : Node
{
    [Export] public string TrackPath = "res://assets/music/level1_theme.ogg";
    [Export] public float TrackLengthSeconds = 33.2f;
    [Export] public float VolumeDb = -6;

    public override void _Ready()
    {
        G.MS.PlayTrack(TrackPath, TrackLengthSeconds, 0.75f, VolumeDb);
    }
}
