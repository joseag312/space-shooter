using Godot;
using System;

public partial class AutoMusic : Node
{
    public static AutoMusic Instance { get; private set; }

    [Export] private AudioStreamPlayer _musicPlayer;
    private Timer _loopTimer;

    private const float TrackLengthSeconds = 19.0f;
    private bool _isPlaying = false;
    private string _defaultTrackPath = "res://assets/music/soundtrack.ogg";
    private float _defaultTrackLength = 19.0f;


    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            ProcessMode = ProcessModeEnum.Always;

            _musicPlayer.Stream = GD.Load<AudioStream>("res://assets/music/soundtrack.ogg");
            _musicPlayer.VolumeDb = -6;
            _musicPlayer.Bus = "Music";

            SetupLoopTimer();
        }
        else
        {
            QueueFree();
        }
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
        if (_isPlaying)
            return;

        if (_musicPlayer.Stream == null)
            return;

        // Ensure volume is audible if previously faded out
        if (_musicPlayer.VolumeDb <= -79.9f)
            _musicPlayer.VolumeDb = -6;

        _isPlaying = true;
        _musicPlayer.Play();
        _loopTimer.Start();
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

    public void FadeIn(float duration = 1.0f, float targetDb = -6)
    {
        if (_isPlaying)
            return;

        _isPlaying = true;
        _musicPlayer.VolumeDb = -80;
        _musicPlayer.Play();
        _loopTimer.Start();

        var tween = GetTree().CreateTween();
        tween.TweenProperty(_musicPlayer, "volume_db", targetDb, duration);
    }

    public void PlayTrack(string resourcePath, float trackLength, float fadeDuration = 0.5f, float volumeDb = -6)
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
        tween.TweenProperty(_musicPlayer, "volume_db", volumeDb, fadeDuration);

        _isPlaying = true;
    }
}
