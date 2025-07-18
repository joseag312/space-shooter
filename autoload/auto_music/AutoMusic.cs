using Godot;
using System;
using System.Collections.Generic;

public partial class AutoMusic : Node
{
    public static AutoMusic Instance { get; private set; }

    [Export] private AudioStreamPlayer _musicPlayer;
    private Timer _loopTimer;

    private const float TrackLengthSeconds = 19.0f;
    private bool _isPlaying = false;
    private string _defaultTrackPath = "res://assets/music/soundtrack.ogg";
    private float _defaultTrackLength = 19.0f;

    private Dictionary<string, TrackData> _trackRegistry = new();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;

            _musicPlayer.Bus = "Music";
            _musicPlayer.VolumeDb = LinearToDb(G.CF.MasterVolume);

            _trackRegistry["main"] = new TrackData("res://assets/music/soundtrack.ogg", 19.0f, 0.75f);
            _trackRegistry["level1"] = new TrackData("res://assets/music/level1_theme.ogg", 33.2f, 0.75f);

            SetupLoopTimer();
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

    private void SetupLoopTimer()
    {
        _loopTimer = new Timer
        {
            WaitTime = TrackLengthSeconds,
            OneShot = false,
            Autostart = false,
        };
        _loopTimer.ProcessMode = ProcessModeEnum.Always;
        AddChild(_loopTimer);

        _loopTimer.Timeout += () =>
        {
            if (!_musicPlayer.Playing)
                _musicPlayer.Play();
        };
    }

    public void Play()
    {
        if (_isPlaying || _musicPlayer.Stream == null)
            return;

        _musicPlayer.VolumeDb = LinearToDb(G.CF.MasterVolume);
        _musicPlayer.Play();
        _loopTimer.Start();
        _isPlaying = true;
    }


    public void Stop()
    {
        if (!_isPlaying)
            return;

        _musicPlayer.Stop();
        _loopTimer.Stop();
        _isPlaying = false;
    }

    public void SetVolumeDb(float volumeDb)
    {
        _musicPlayer.VolumeDb = volumeDb;
    }

    public void FadeOut(float duration = 1.0f)
    {
        if (!_isPlaying)
            return;

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_musicPlayer, "volume_db", -80, duration);
        tween.TweenCallback(Callable.From(() =>
        {
            _isPlaying = false;
            _loopTimer.Stop();
        }));
    }

    public void FadeIn(float duration = 1.0f)
    {
        if (_isPlaying)
            return;

        _isPlaying = true;
        _musicPlayer.VolumeDb = -80;
        _musicPlayer.Play();
        _loopTimer.Start();

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_musicPlayer, "volume_db", LinearToDb(G.CF.MasterVolume), duration);
    }

    public void PlayTrack(string trackKey)
    {
        if (!_trackRegistry.TryGetValue(trackKey, out var track))
        {
            GD.PrintErr($"ERROR: AutoMusic - Unknown track key '{trackKey}'");
            return;
        }

        PlayTrack(track.Path, track.Length, track.FadeDuration);
    }

    public void PlayTrack(string resourcePath, float trackLength, float fadeDuration = 0.5f)
    {
        if (_isPlaying && _musicPlayer.Stream.ResourcePath == resourcePath)
            return;

        Stop();

        var newStream = GD.Load<AudioStream>(resourcePath);
        if (newStream == null)
        {
            GD.PrintErr($"ERROR: AutoMusic - Could not load track at {resourcePath}");
            return;
        }

        _musicPlayer.Stream = newStream;
        _musicPlayer.VolumeDb = -80; // fade in from silent
        _musicPlayer.Bus = "Music";
        _musicPlayer.Play();

        _loopTimer.WaitTime = trackLength;
        _loopTimer.Start();

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_musicPlayer, "volume_db", LinearToDb(G.CF.MasterVolume), fadeDuration);

        _isPlaying = true;
    }

    private class TrackData
    {
        public string Path;
        public float Length;
        public float FadeDuration;

        public TrackData(string path, float length, float fade = 0.5f)
        {
            Path = path;
            Length = length;
            FadeDuration = fade;
        }
    }
}
