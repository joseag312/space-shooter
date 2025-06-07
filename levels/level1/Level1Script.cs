using System.Threading;
using System.Threading.Tasks;
using Godot;

public partial class Level1Script : Node
{
	[Export] public Ship Ship { get; set; }
	[Export] public LevelFlowComponent LevelFlowComponent;
	[Export] public SpawnerComponent Enemy1Spawner;
	[Export] public SpawnerComponent Enemy2Spawner;
	[Export] public SpawnerComponent Enemy3Spawner;

	private CancellationTokenSource _levelScriptCts;

	public override void _Ready()
	{
		Ship.StatsComponent.Connect("NoHealth", new Callable(this, nameof(OnShipDeath)));

		_levelScriptCts = new CancellationTokenSource();
		_ = LevelScript(_levelScriptCts.Token);
	}

	private async Task LevelScript(CancellationToken token)
	{
		try
		{
			await Task.Delay(3000, token);
			LevelFlowComponent.SpawnWave(Enemy2Spawner, 10, 15);
			await Task.Delay(3000, token);

			await Task.Delay(3000, token);
			LevelFlowComponent.StartSpawner1(300);
			await Task.Delay(20000, token);
			LevelFlowComponent.StopSpawner1();

			await Task.Delay(3000, token);
			LevelFlowComponent.SpawnWave(Enemy2Spawner, 10, 30);
			await Task.Delay(3000, token);

			await Task.Delay(500, token);
			LevelFlowComponent.StartSpawner1(300);
			await Task.Delay(20000, token);
			LevelFlowComponent.StopSpawner1();
			await Task.Delay(500, token);
		}
		catch (TaskCanceledException)
		{
			GD.Print("DEBUG: LevelScript - Canceled");
		}
	}

	private void OnShipDeath()
	{
		_levelScriptCts?.Cancel();
		_levelScriptCts?.Dispose();
		_levelScriptCts = null;
	}
}
