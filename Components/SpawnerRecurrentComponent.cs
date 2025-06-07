using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public partial class SpawnerRecurrentComponent : Node
{
    [Export] public SpawnerComponent Enemy1Spawner { get; set; }
    [Export] public SpawnerComponent Enemy2Spawner { get; set; }
    [Export] public SpawnerComponent Enemy3Spawner { get; set; }

    [Export] public int Margin = 8;

    private int _leftBorder;
    private int _rightBorder;

    private CancellationTokenSource _spawn1Cts;
    private CancellationTokenSource _spawn2Cts;
    private CancellationTokenSource _spawn3Cts;

    private bool _shouldSpawn1;
    private bool _shouldSpawn2;
    private bool _shouldSpawn3;

    public override void _Ready()
    {
        _leftBorder = Margin;
        _rightBorder = (int)ProjectSettings.GetSetting("display/window/size/viewport_width") - Margin;
    }

    public void StartSpawner1(int delay) => StartRecurrentSpawn(ref _shouldSpawn1, ref _spawn1Cts, delay, RecurrentSpawn1);
    public void StartSpawner2(int delay) => StartRecurrentSpawn(ref _shouldSpawn2, ref _spawn2Cts, delay, RecurrentSpawn2);
    public void StartSpawner3(int delay) => StartRecurrentSpawn(ref _shouldSpawn3, ref _spawn3Cts, delay, RecurrentSpawn3);

    public void StopSpawner1() => StopSpawner(ref _shouldSpawn1, ref _spawn1Cts);
    public void StopSpawner2() => StopSpawner(ref _shouldSpawn2, ref _spawn2Cts);
    public void StopSpawner3() => StopSpawner(ref _shouldSpawn3, ref _spawn3Cts);

    public void StopAllSpawning()
    {
        StopSpawner1();
        StopSpawner2();
        StopSpawner3();
    }

    private void StopSpawner(ref bool flag, ref CancellationTokenSource cts)
    {
        flag = false;
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private void StartRecurrentSpawn(ref bool flag, ref CancellationTokenSource cts, int delay, Func<int, CancellationToken, Task> method)
    {
        flag = true;
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
        _ = method.Invoke(delay, cts.Token);
    }

    private async Task RecurrentSpawn1(int time, CancellationToken token)
    {
        if (Enemy1Spawner.Scene == null) return;

        int spawnCount = 0;
        while (_shouldSpawn1 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            int x = (int)GD.RandRange(_leftBorder + Margin, _rightBorder - Margin);
            Vector2 position = new Vector2(x, -120);

            Node2D enemy = Enemy1Spawner.Scene.Instantiate<Node2D>();
            enemy.GlobalPosition = position;
            enemy.AddToGroup("despawnable");
            enemy.Name = $"Enemy_{spawnCount++}";

            GetParent()?.AddChild(enemy);
        }
    }

    private async Task RecurrentSpawn2(int time, CancellationToken token)
    {
        if (Enemy2Spawner.Scene == null) return;

        int spawnCount = 0;
        while (_shouldSpawn2 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            int x = (int)GD.RandRange(_leftBorder + Margin, _rightBorder - Margin);
            Vector2 position = new Vector2(x, -120);

            Node2D enemy = Enemy2Spawner.Scene.Instantiate<Node2D>();
            enemy.GlobalPosition = position;
            enemy.AddToGroup("despawnable");
            enemy.Name = $"Enemy_{spawnCount++}";

            GetParent()?.AddChild(enemy);
        }
    }

    private async Task RecurrentSpawn3(int time, CancellationToken token)
    {
        if (Enemy3Spawner.Scene == null) return;

        int spawnCount = 0;
        while (_shouldSpawn3 && !token.IsCancellationRequested && IsInsideTree())
        {
            try { await Task.Delay(time, token); }
            catch (TaskCanceledException) { break; }

            int x = (int)GD.RandRange(_leftBorder + Margin, _rightBorder - Margin);
            Vector2 position = new Vector2(x, -120);

            Node2D enemy = Enemy3Spawner.Scene.Instantiate<Node2D>();
            enemy.GlobalPosition = position;
            enemy.AddToGroup("despawnable");
            enemy.Name = $"Enemy_{spawnCount++}";

            GetParent()?.AddChild(enemy);
        }
    }
}
