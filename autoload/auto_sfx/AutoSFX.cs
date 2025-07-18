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

            _sfxPlayer.Bus = "SFX";
            _sfxPlayer.VolumeDb = LinearToDb(G.CF.SfxVolume);

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

    private static float LinearToDb(float linear)
    {
        if (linear <= 0.001f)
            return -80f;
        return 20f * (float)Math.Log10(linear);
    }

    public void Play(string name)
    {
        if (_sounds.TryGetValue(name, out var stream))
        {
            _sfxPlayer.Stream = stream;
            _sfxPlayer.Play();
        }
        else
        {
            GD.PrintErr($"ERROR: AutoSFX - Sound '{name}' not found");
        }
    }

    public void SetVolumeDb(float db)
    {
        _sfxPlayer.VolumeDb = db;
    }
}
