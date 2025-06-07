using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class LevelFlowComponent : Node
{
    [Export] public Ship Ship { get; set; }
    [Export] public SpawnerComponent Enemy1Spawner { get; set; }
    [Export] public SpawnerComponent Enemy2Spawner { get; set; }
    [Export] public SpawnerComponent Enemy3Spawner { get; set; }
    [Export] public CanvasLayer HUDCanvasLayer { get; set; }
    [Export] public ParallaxBackground ParallaxBackground { get; set; }

    private BackgroundFadeComponent _backgroundFadeComponent;
    private CancellationTokenSource _spawn1Cts;
    private CancellationTokenSource _spawn2Cts;
    private CancellationTokenSource _spawn3Cts;

    private double _cleanupTimer = 0.0;
    private bool _shouldSpawn1;
    private bool _shouldSpawn2;
    private bool _shouldSpawn3;
    private int _margin;
    private int _leftBorder;
    private int _rightBorder;
    private bool _complete;

    public override void _Ready()
    {
        G.BG.UnblockInput();
        G.GF.ResetTransition();
        G.GF.LastPlayedScene = GetTree().CurrentScene.SceneFilePath;

        _backgroundFadeComponent = ParallaxBackground.GetNodeOrNull<BackgroundFadeComponent>("BackgroundFadeComponent");

        _margin = 8;
        _leftBorder = _margin;
        _rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width") - _margin;

        Ship.StatsComponent.Connect("NoHealth", new Callable(this, nameof(OnShipDeath)));
    }

    public override void _Process(double delta)
    {
        _cleanupTimer += delta;
        if (_cleanupTimer >= 1.0)
        {
            _cleanupTimer = 0;
            CleanupOffscreenEnemies();
        }
    }

    public void SpawnWave(SpawnerComponent spawner, int count, int margin)
    {
        PackedScene enemyScene = spawner.Scene;
        int spriteWidth = GetSpriteWidth(enemyScene);
        int screenCenter = (_leftBorder + _rightBorder) / 2;
        int totalSpacing = spriteWidth + margin;

        for (int i = 0; i < count; i++)
        {
            int offset = ((i - count / 2) * totalSpacing);
            Vector2 spawnPosition = new Vector2(screenCenter + offset, -120);
            Node2D enemy = enemyScene.Instantiate<Node2D>();
            enemy.GlobalPosition = spawnPosition;
            enemy.AddToGroup("spawn_wave");
            AddChild(enemy);
        }
    }

    public void StartSpawner1(int delay) => StartRecurrentSpawn(ref _shouldSpawn1, ref _spawn1Cts, Enemy1Spawner, delay, RecurrentSpawn1);
    public void StartSpawner2(int delay) => StartRecurrentSpawn(ref _shouldSpawn2, ref _spawn2Cts, Enemy2Spawner, delay, RecurrentSpawn2);
    public void StartSpawner3(int delay) => StartRecurrentSpawn(ref _shouldSpawn3, ref _spawn3Cts, Enemy3Spawner, delay, RecurrentSpawn3);

    public void StopSpawner1() { _shouldSpawn1 = false; _spawn1Cts?.Cancel(); }
    public void StopSpawner2() { _shouldSpawn2 = false; _spawn2Cts?.Cancel(); }
    public void StopSpawner3() { _shouldSpawn3 = false; _spawn3Cts?.Cancel(); }

    private void StartRecurrentSpawn(ref bool spawnFlag, ref CancellationTokenSource cts, SpawnerComponent spawner, int delay, Func<SpawnerComponent, int, CancellationToken, Task> spawnMethod)
    {
        spawnFlag = true;
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
        _ = spawnMethod.Invoke(spawner, delay, cts.Token);
    }

    public async Task RecurrentSpawn1(SpawnerComponent spawner, int time, CancellationToken token)
    {
        PackedScene enemyScene = spawner.Scene;
        if (enemyScene == null) return;

        int spawnCount = 0;

        while (_shouldSpawn1 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            if (!_shouldSpawn1 || token.IsCancellationRequested || !IsInsideTree()) break;

            int x = (int)GD.RandRange(_leftBorder + _margin, _rightBorder - _margin);
            Vector2 position = new Vector2(x, -120);

            Node2D enemy = enemyScene.Instantiate<Node2D>();
            enemy.GlobalPosition = position;
            enemy.AddToGroup("despawnable");
            enemy.Name = $"Enemy_{spawnCount}";

            GetParent()?.AddChild(enemy);
            spawnCount++;
        }
    }

    public async Task RecurrentSpawn2(SpawnerComponent spawner, int time, CancellationToken token)
    {
        while (_shouldSpawn2 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            if (!_shouldSpawn2 || token.IsCancellationRequested || !IsInsideTree()) break;

            Vector2 position = new Vector2(GD.RandRange(_leftBorder + _margin, _rightBorder - _margin), -120);
            spawner.Spawn(position, GetParent());
        }
    }

    public async Task RecurrentSpawn3(SpawnerComponent spawner, int time, CancellationToken token)
    {
        while (_shouldSpawn3 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            if (!_shouldSpawn3 || token.IsCancellationRequested || !IsInsideTree()) break;

            Vector2 position = new Vector2(GD.RandRange(_leftBorder + _margin, _rightBorder - _margin), -120);
            spawner.Spawn(position, GetParent());
        }
    }

    private void StopAllRecurrentSpawning()
    {
        _shouldSpawn1 = false;
        _shouldSpawn2 = false;
        _shouldSpawn3 = false;

        _spawn1Cts?.Cancel();
        _spawn2Cts?.Cancel();
        _spawn3Cts?.Cancel();

        _spawn1Cts?.Dispose();
        _spawn2Cts?.Dispose();
        _spawn3Cts?.Dispose();
    }

    private int GetSpriteWidth(PackedScene enemyScene)
    {
        int spriteWidth = 0;
        Node2D tempEnemy = enemyScene.Instantiate<Node2D>();
        AnimatedSprite2D animatedSprite = tempEnemy.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

        if (animatedSprite?.SpriteFrames is SpriteFrames frames && frames.HasAnimation("move"))
        {
            spriteWidth = (int)frames.GetFrameTexture("move", 0).GetWidth();
        }

        tempEnemy.QueueFree();
        return spriteWidth;
    }

    private void CleanupOffscreenEnemies()
    {
        var viewportRect = GetViewport().GetVisibleRect();

        foreach (var child in GetTree().GetNodesInGroup("despawnable"))
        {
            if (child is Node2D node2D && !viewportRect.HasPoint(node2D.GlobalPosition))
                node2D.QueueFree();
        }
    }

    private async void OnShipDeath()
    {
        StopAllRecurrentSpawning();

        var fadeBlackTask = G.BG.FadeInBlack(0.3f);
        var despawnTask = FadeOutAllDespawnables();
        var hudTask = FadeOutHUD();
        var backgroundTask = _backgroundFadeComponent.FadeOutAll(0.3f);

        await Task.WhenAll(despawnTask, hudTask, backgroundTask, fadeBlackTask);
        await G.GF.FadeToSceneWithBGFast(G.GF.GameOverScene);
    }

    private async Task FadeOutAllDespawnables()
    {
        var awaiters = new List<SignalAwaiter>();
        var toFade = new HashSet<Node2D>();

        foreach (var node in GetTree().GetNodesInGroup("despawnable"))
            if (node is Node2D n2d) toFade.Add(n2d);

        foreach (var node in GetTree().GetNodesInGroup("spawn_wave"))
            if (node is Node2D n2d) toFade.Add(n2d);

        foreach (var n2d in toFade)
        {
            n2d.Visible = true;
            var tween = CreateTween();
            tween.TweenProperty(n2d, "modulate:a", 0.0f, 0.4f);
            var nodeToFree = n2d;

            tween.TweenCallback(Callable.From(() =>
            {
                if (GodotObject.IsInstanceValid(nodeToFree))
                    nodeToFree.QueueFree();
            }));

            awaiters.Add(ToSignal(tween, "finished"));
        }

        foreach (var awaiter in awaiters)
            await awaiter;
    }

    private async Task FadeOutHUD()
    {
        var awaiters = new List<SignalAwaiter>();

        foreach (var child in HUDCanvasLayer.GetChildren())
        {
            if (child is Control control && control.Visible)
            {
                var tween = CreateTween();
                tween.TweenProperty(control, "modulate:a", 0.0f, 0.4f);
                awaiters.Add(ToSignal(tween, "finished"));
            }
        }

        foreach (var awaiter in awaiters)
            await awaiter;
    }
}
