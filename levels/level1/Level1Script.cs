using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class Level1Script : Node
{
	[Export] public Ship Ship { get; set; }
	[Export] public LevelFlowComponent LevelFlowComponent { get; set; }
	[Export] public SpawnerComponent Enemy1Spawner { get; set; }
	[Export] public SpawnerComponent Enemy2Spawner { get; set; }
	[Export] public SpawnerComponent Enemy3Spawner { get; set; }

	public override void _Ready()
	{
		G.CR.Run("LevelScript", RunLevelScript);
		Ship.StatsComponent.Connect("NoHealth", new Callable(this, nameof(OnShipDeath)));
	}

	private async Task RunLevelScript(CancellationToken token)
	{
		try
		{
			await Task.Delay(3000, token);
			await LevelFlowComponent.SpawnerWave.SpawnWaveUntilCleared(Enemy2Spawner, 10, 15);
			await Task.Delay(3000, token);


			// LevelFlowComponent.SpawnerRecurrent.StartSpawner1(300);
			// await Task.Delay(20000, token);
			// LevelFlowComponent.SpawnerRecurrent.StopSpawner1();

			// await Task.Delay(3000, token);
			// LevelFlowComponent.SpawnerWave.SpawnWave(Enemy2Spawner, 10, 30);
			// await Task.Delay(3000, token);

			// LevelFlowComponent.SpawnerRecurrent.StartSpawner1(300);
			// await Task.Delay(20000, token);
			// LevelFlowComponent.SpawnerRecurrent.StopSpawner1();
			// await Task.Delay(500, token);

			await HandleLevelClear();
		}
		catch (TaskCanceledException)
		{
			GD.Print("DEBUG: Level1Script - Script canceled");
		}
	}

	private async Task HandleLevelClear()
	{
		G.BG.BlockInput();
		G.GF.BlockInput();

		await LevelFlowComponent.LevelCleanup.FadeOutAll();

		Ship.StopFiring();
		Ship.PositionClampComponent.Enabled = false;
		Ship.MoveComponent.Velocity = new Vector2(0, -250);
		await Task.Delay(2000);

		await G.BG.FadeInBlack(0.3f);

		G.GF.UnblockInput();
		await G.GF.FadeToSceneWithBG(G.GF.LevelClearScene);
	}

	private void OnShipDeath()
	{
		G.CR.Stop("LevelScript");
	}
}
