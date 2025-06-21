using Godot;
using System;
using System.Collections.Generic;

public partial class AutoSFX : Node
{
    public static AutoSFX Instance { get; private set; }

    [Export] private AudioStreamPlayer _sfxPlayer;

    private Dictionary<string, AudioStream> _sounds = new();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;

            _sounds["oiia_slow"] = GD.Load<AudioStream>("res://assets/sounds/oiia_slow.ogg");
            _sounds["oiia_fast"] = GD.Load<AudioStream>("res://assets/sounds/oiia_fast.ogg");
            _sounds["oiia_death"] = GD.Load<AudioStream>("res://assets/sounds/oiia_death.ogg");
            _sounds["meow"] = GD.Load<AudioStream>("res://assets/sounds/meow.ogg");
        }
        else
        {
            QueueFree();
        }
    }

    public void Play(string name, float volumeDb = -6)
    {
        if (_sounds.TryGetValue(name, out var stream))
        {
            _sfxPlayer.VolumeDb = volumeDb;
            _sfxPlayer.Stream = stream;
            _sfxPlayer.Play();
        }
        else
        {
            GD.PrintErr($"ERROR: AutoSFX - Sound '{name}' not found");
        }
    }

    public void SetVolume(float db)
    {
        _sfxPlayer.VolumeDb = db;
    }
}
