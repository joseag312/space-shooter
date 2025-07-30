using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public abstract partial class BaseLevelScript : Node
{
	[Export] public Ship Ship { get; set; }
	[Export] public LevelFlowComponent LevelFlowComponent { get; set; }
	[Export] public SpawnerComponent Enemy1Spawner { get; set; }
	[Export] public SpawnerComponent Enemy2Spawner { get; set; }
	[Export] public SpawnerComponent Enemy3Spawner { get; set; }
	[Export] public HUDMain HUD { get; set; }

	public override void _Ready()
	{
		Ship.StatsComponent.Connect("NoHealth", new Callable(this, nameof(OnShipDeath)));
		HUD.Powers.SetWeaponManager(Ship.WeaponManager);

		G.MS.PlayTrack(GetLevelMusic());
		G.CR.Run("LevelScript", RunLevelScript);
	}

	private async Task RunLevelScript(CancellationToken token)
	{
		try
		{
			G.GS.CurrentLevel = GetLevelId();
			await RunLevel(token);
		}
		catch (TaskCanceledException)
		{
			GD.Print("DEBUG: BaseLevelScript - Script canceled");
		}
	}

	protected abstract String GetLevelMusic();
	protected abstract int GetLevelId();
	protected abstract Task RunLevel(CancellationToken token);

	protected void StartDialog()
	{
		G.GF.StartDialog();
		Ship.StopFiring();
	}

	protected void StopDialog()
	{
		G.GF.StopDialog();
		Ship.StartFiring();
	}

	protected async Task HandleLevelClear()
	{
		G.BG.BlockInput();
		G.GF.BlockInput();
		G.GS.Save();
		await LevelFlowComponent.FadeOutAll();

		Ship.StopFiring();
		Ship.PositionClampComponent.Enabled = false;
		Ship.MoveComponent.Velocity = new Vector2(0, -250);

		await Task.Delay(3000);
		G.MS.FadeOut();

		await G.BG.FadeInBlack(0.3f);
		G.GF.UnblockInput();

		await G.GF.FadeToSceneWithBG(G.GF.LevelClearScene);
	}

	private void OnShipDeath()
	{
		G.MS.FadeOut();
		G.SFX.Play(SFX.OIIA_DEATH);
		G.CR.Stop("LevelScript");
	}
}
